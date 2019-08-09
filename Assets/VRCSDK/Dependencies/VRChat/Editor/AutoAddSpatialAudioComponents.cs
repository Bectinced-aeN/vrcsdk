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

        TryToAddSpatializationToAllAudioSources(true, false);
    }

	static void OnProjectWindowChanged()
	{
		RegisterCallbacks();
	}

	static void RegisterCallbacks()
	{
		VRC_SdkControlPanel._EnableSpatialization = VRCSDKControlPanel_EnableSpatialization;
	}

	// callback from VrcSdkControlPanel in dll
	public static void VRCSDKControlPanel_EnableSpatialization()
	{
		Debug.Log("Enabling spatialization on AudioSources...");
		TryToAddSpatializationToAllAudioSources(false, true);
	}

    // this duplicates the same function in VRCAudioManager, but alas, it's not available in the SDK
    static bool ApplyDefaultSpatializationToAudioSource(AudioSource audioSrc, bool force = false)
    {
        if (audioSrc == null)
            return false;

        // don't spatialize non-full 3D sounds
        if (!force && audioSrc.spatialBlend < 0.99f)
            return false;

        var vrcsp = audioSrc.gameObject.GetComponent<VRCSDK2.VRC_SpatialAudioSource>();

        bool setValues = force;

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
            }

            Component.DestroyImmediate(onsp);
        }
#endif
        if (vrcsp == null)
        {
            vrcsp = audioSrc.gameObject.AddComponent<VRCSDK2.VRC_SpatialAudioSource>();
            setValues = true;
        }

        audioSrc.spatialize = true;
        vrcsp.enabled = true;

        if (setValues)
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

                src.spatialBlend = 1.0f;  // set 3D mode

                UnityEngine.Audio.AudioMixerGroup mixer = AssetDatabase.LoadAssetAtPath<UnityEngine.Audio.AudioMixerGroup>("Assets/VRCSDK/Dependencies/OSPNative/scenes/mixers/SpatializerMixer.mixer");
                if (mixer != null)
                {
                    src.outputAudioMixerGroup = mixer;
                }
            }

            if (ApplyDefaultSpatializationToAudioSource(src, false))
            {
                Debug.Log("Automatically added VRC_SpatialAudioSource component and enabled spatialized audio to " + GetGameObjectPath(src.gameObject) + "!");
            }
        }
    }

    static bool IsNewAudioSource(AudioSource src)
    {
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

}
