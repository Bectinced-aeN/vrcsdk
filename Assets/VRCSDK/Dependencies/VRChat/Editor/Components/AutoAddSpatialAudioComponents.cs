#define SUPPORT_DEPRECATED_ONSP

using UnityEngine;
using System.Collections;
using UnityEditor;
using VRCSDK2;

[InitializeOnLoad]
public class AutoAddSpatialAudioComponents
{

    public static bool Enabled = true;

    static AutoAddSpatialAudioComponents()
    {
        EditorApplication.hierarchyWindowChanged += OnHierarchyWindowChanged;
		EditorApplication.projectWindowChanged += OnProjectWindowChanged;
		RegisterCallbacks();
    }

    static void OnHierarchyWindowChanged()
    {
        if (!Enabled)
        {
            EditorApplication.hierarchyWindowChanged -= OnHierarchyWindowChanged;
            return;
        }

        // check for proper use of VRCSP, and warn
        //TryToAddSpatializationToAllAudioSources(true, false);
    }

	static void OnProjectWindowChanged()
	{
		RegisterCallbacks();
	}

	static void RegisterCallbacks()
	{
        VRCSdkControlPanel._EnableSpatialization = VRCSDKControlPanel_EnableSpatialization;
	}

	// callback from VrcSdkControlPanel in dll
	public static void VRCSDKControlPanel_EnableSpatialization()
	{
		Debug.Log("Enabling spatialization on 3D AudioSources...");
		TryToAddSpatializationToAllAudioSources(false, true);
	}

    static bool ApplyDefaultSpatializationToAudioSource(AudioSource audioSrc, bool force = false)
    {
        if (audioSrc == null)
            return false;

        var vrcsp = audioSrc.gameObject.GetComponent<VRCSDK2.VRC_SpatialAudioSource>();

        // don't make changes if we already have a vrcsp and we aren't forcing
        if (vrcsp != null && !force)
            return false;

        if (force)
            audioSrc.spatialBlend = 1;

        bool initValues = force;

        // is audio source set to be 2D?
        bool is2D = audioSrc.spatialBlend == 0;

#if SUPPORT_DEPRECATED_ONSP
        var onsp = audioSrc.GetComponent<ONSPAudioSource>();
        if (onsp != null)
        {
            if (vrcsp == null)
            {
                // copy the values from deprecated component
                vrcsp = audioSrc.gameObject.AddComponent<VRCSDK2.VRC_SpatialAudioSource>();
                vrcsp.Gain = onsp.Gain;
                vrcsp.Near = onsp.Near;
                vrcsp.Far = onsp.Far;
                vrcsp.UseAudioSourceVolumeCurve = !onsp.UseInvSqr;
                vrcsp.EnableSpatialization = onsp.EnableSpatialization;
            }

            // remove deprecated component
            Component.DestroyImmediate(onsp);
        }
#endif
        if (vrcsp == null)
        {
            // no onsp and no vrcsp, so add
            vrcsp = audioSrc.gameObject.AddComponent<VRCSDK2.VRC_SpatialAudioSource>();
            if (is2D)
            {
                // this audio source was marked as 2D, leave the vrcsp disabled
                vrcsp.EnableSpatialization = false;
            }
            initValues = true;
        }

        audioSrc.spatialize = vrcsp.EnableSpatialization;
        vrcsp.enabled = true;

        if (initValues)
        {
            bool isAvatar = audioSrc.GetComponentInParent<VRCSDK2.VRC_AvatarDescriptor>();

            vrcsp.Gain = isAvatar ? VRCSDK2.AudioManagerSettings.AvatarAudioMaxGain : VRCSDK2.AudioManagerSettings.RoomAudioGain;
            vrcsp.Near = 0;
            vrcsp.Far = isAvatar ? VRCSDK2.AudioManagerSettings.AvatarAudioMaxRange : VRCSDK2.AudioManagerSettings.RoomAudioMaxRange;
            vrcsp.UseAudioSourceVolumeCurve = false;
        }

        return true;
    }

    public static void TryToAddSpatializationToAllAudioSources(bool newAudioSourcesOnly, bool includeInactive)
    {
        AudioSource[] allAudioSources = includeInactive ? Resources.FindObjectsOfTypeAll<AudioSource>() : Object.FindObjectsOfType<AudioSource>();
        foreach (AudioSource src in allAudioSources)
        {
            if (src == null || src.gameObject == null || src.gameObject.scene != UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene())
            {
                continue;
            }

            if (newAudioSourcesOnly)
            {
                if (!IsNewAudioSource(src))
                    continue;

                UnityEngine.Audio.AudioMixerGroup mixer = AssetDatabase.LoadAssetAtPath<UnityEngine.Audio.AudioMixerGroup>("Assets/VRCSDK/Dependencies/OSPNative/scenes/mixers/SpatializerMixer.mixer");
                if (mixer != null)
                {
                    src.outputAudioMixerGroup = mixer;
                }
            }

            if (ApplyDefaultSpatializationToAudioSource(src, false))
            {
                Debug.Log("Automatically added VRC_SpatialAudioSource component to " + GetGameObjectPath(src.gameObject) + "!");
            }
        }
    }

    static bool IsNewAudioSource(AudioSource src)
    {
        var vrcsp = src.GetComponent<VRC_SpatialAudioSource>();
        if (vrcsp != null)
            return false;

        if (src.clip != null)
            return false;
        if (src.outputAudioMixerGroup != null)
            return false;

        if (src.mute || src.bypassEffects || src.bypassReverbZones || !src.playOnAwake || src.loop)
            return false;

        if (src.priority != 128 ||
            !Mathf.Approximately(src.volume, 1.0f) ||
            !Mathf.Approximately(src.pitch, 1.0f) ||
            !Mathf.Approximately(src.panStereo, 0.0f) ||
            !Mathf.Approximately(src.spatialBlend, 0.0f) ||
            !Mathf.Approximately(src.reverbZoneMix, 1.0f))
        {
            return false;
        }

        if (!Mathf.Approximately(src.dopplerLevel, 1.0f) ||
            !Mathf.Approximately(src.spread, 0.0f) ||
            src.rolloffMode != AudioRolloffMode.Logarithmic ||
            !Mathf.Approximately(src.minDistance, 1.0f) ||
            !Mathf.Approximately(src.maxDistance, 500.0f))
        {
            return false;
        }

        return true;
    }

    static string GetGameObjectPath(GameObject obj)
    {
        string path = "/" + obj.name;
        while (obj.transform.parent != null)
        {
            obj = obj.transform.parent.gameObject;
            path = "/" + obj.name + path;
        }
        return path;
    }

    public static void ConvertONSPAudioSource(AudioSource src)
    {
        if (src == null) return;

        var onsp = src.GetComponent<ONSPAudioSource>();
        if (onsp != null)
        {
            var vrcsp = src.gameObject.GetComponent<VRCSDK2.VRC_SpatialAudioSource>();
            if (vrcsp == null)
            {
                // copy the values from deprecated component
                vrcsp = src.gameObject.AddComponent<VRCSDK2.VRC_SpatialAudioSource>();
                vrcsp.Gain = onsp.Gain;
                vrcsp.Near = onsp.Near;
                vrcsp.Far = onsp.Far;
                vrcsp.UseAudioSourceVolumeCurve = !onsp.UseInvSqr;
                vrcsp.EnableSpatialization = onsp.EnableSpatialization;
            }
            // remove deprecated component
            Component.DestroyImmediate(onsp);
        }
    }

    public static void AddVRCSpatialToBareAudioSource(AudioSource src)
    {
        if (src == null) return;

        var vrcsp = src.gameObject.GetComponent<VRCSDK2.VRC_SpatialAudioSource>();
        if (vrcsp != null) return;

        vrcsp = src.gameObject.AddComponent<VRCSDK2.VRC_SpatialAudioSource>();

        // add default values
        bool isAvatar = src.gameObject.GetComponentInParent<VRCSDK2.VRC_AvatarDescriptor>();

        vrcsp.Gain = isAvatar ? VRCSDK2.AudioManagerSettings.AvatarAudioMaxGain : VRCSDK2.AudioManagerSettings.RoomAudioGain;
        vrcsp.Near = 0;
        vrcsp.Far = isAvatar ? VRCSDK2.AudioManagerSettings.AvatarAudioMaxRange : VRCSDK2.AudioManagerSettings.RoomAudioMaxRange;
        vrcsp.UseAudioSourceVolumeCurve = false;

        // enable spatialization if src is not 2D
        AnimationCurve curve = src.GetCustomCurve(AudioSourceCurveType.SpatialBlend);
        if (src.spatialBlend == 0 || (curve == null || curve.keys.Length <= 1))
            vrcsp.EnableSpatialization = false;
        else
            vrcsp.EnableSpatialization = true;
    }
}
