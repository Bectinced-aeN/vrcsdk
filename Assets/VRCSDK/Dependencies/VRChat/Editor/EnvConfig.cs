//#define ENV_SET_INCLUDED_SHADERS

using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using System.Reflection;
using System.Linq;

/// <summary>
/// Setup up SDK env on editor launch
/// </summary>
[InitializeOnLoad]
public class EnvConfig
{
    static BuildTarget[] relevantBuildTargets = new BuildTarget[] {
        BuildTarget.Android, BuildTarget.iOS,
        BuildTarget.StandaloneLinux, BuildTarget.StandaloneLinux64, BuildTarget.StandaloneLinuxUniversal,
        BuildTarget.StandaloneWindows, BuildTarget.StandaloneWindows64,
        BuildTarget.StandaloneOSXIntel, BuildTarget.StandaloneOSXIntel64, BuildTarget.StandaloneOSXUniversal
    };

    static System.Collections.Generic.Dictionary<BuildTarget, UnityEngine.Rendering.GraphicsDeviceType[]> allowedGraphicsAPIs = new System.Collections.Generic.Dictionary<BuildTarget, UnityEngine.Rendering.GraphicsDeviceType[]>()
    {
        { BuildTarget.Android, null },
        { BuildTarget.iOS, null },
        { BuildTarget.StandaloneLinux, null },
        { BuildTarget.StandaloneLinux64, null },
        { BuildTarget.StandaloneLinuxUniversal, null },
        { BuildTarget.StandaloneWindows, new UnityEngine.Rendering.GraphicsDeviceType[] { UnityEngine.Rendering.GraphicsDeviceType.Direct3D11 } },
        { BuildTarget.StandaloneWindows64, new UnityEngine.Rendering.GraphicsDeviceType[] { UnityEngine.Rendering.GraphicsDeviceType.Direct3D11 } },
        { BuildTarget.StandaloneOSXIntel, null },
        { BuildTarget.StandaloneOSXIntel64, null },
        { BuildTarget.StandaloneOSXUniversal, null }
    };

    static BuildTarget[] allowedTargets = new BuildTarget[] {
        BuildTarget.StandaloneWindows64, BuildTarget.StandaloneWindows
    };

    private static bool _requestConfigureSettings = true;

#if ENV_SET_INCLUDED_SHADERS
    static string[] minimalShaders = new string[]
    {
        "Standard",
        "Standard (Specular setup)"
    };
#endif

    static EnvConfig()
    {
        EditorApplication.update += EditorUpdate;
    }

    static void EditorUpdate()
    {
        if (_requestConfigureSettings)
        {
            if (ConfigureSettings())
            {
                _requestConfigureSettings = false; 
            }
        }
    }

    public static void RequestConfigureSettings()
    {
        _requestConfigureSettings = true;
    }

    [UnityEditor.Callbacks.DidReloadScripts(int.MaxValue)]
    static void DidReloadScripts()
    {
        RequestConfigureSettings();
    }

    public static bool ConfigureSettings()
    {
        CustomDLLMaker.CreateDirectories();
        if (CheckForFirstInit())
            VRC.AssetExporter.CleanupTmpFiles();

        if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isUpdating)
            return false;

        ConfigurePlayerSettings();
        return true;
    }

    public static void ConfigurePlayerSettings()
	{
		Debug.Log("Setting required PlayerSettings...");

        SetBuildTarget();

		// Needed for Microsoft.CSharp namespace in DLLMaker
		// Doesn't seem to work though
		if(PlayerSettings.GetApiCompatibilityLevel(EditorUserBuildSettings.selectedBuildTargetGroup) != ApiCompatibilityLevel.NET_2_0)
			PlayerSettings.SetApiCompatibilityLevel(EditorUserBuildSettings.selectedBuildTargetGroup, ApiCompatibilityLevel.NET_2_0);

		if(!PlayerSettings.runInBackground)
			PlayerSettings.runInBackground = true;

        SetDefaultGraphicsAPIs();
        SetGraphicsSettings();
        SetPlayerSettings();
        
#if VRC_CLIENT
        RefreshClientVRSDKs();
#else
		// SDK

		// default to steam runtime in sdk (shouldn't matter)
		SetVRSDKs(new string[] { "None", "OpenVR", "Oculus" });
#endif
    }

    static void EnableBatching(bool enable)
	{
		PlayerSettings[] playerSettings = Resources.FindObjectsOfTypeAll<PlayerSettings>();
		if (playerSettings == null)
			return;

		SerializedObject playerSettingsSerializedObject = new SerializedObject(playerSettings);
		SerializedProperty batchingSettings = playerSettingsSerializedObject.FindProperty("m_BuildTargetBatching");
		if (batchingSettings == null)
			return;

		for (int i = 0;i < batchingSettings.arraySize;i++)
		{
			SerializedProperty batchingArrayValue = batchingSettings.GetArrayElementAtIndex(i);
			if (batchingArrayValue == null)
				continue;
			
			IEnumerator batchingEnumerator = batchingArrayValue.GetEnumerator();
			if (batchingEnumerator == null)
				continue;

			while(batchingEnumerator.MoveNext())
			{
				SerializedProperty property = (SerializedProperty)batchingEnumerator.Current;

				if (property != null && property.name == "m_BuildTarget")
				{
					// only change setting on "Standalone" entry
					if (property.stringValue != "Standalone")
						break;
				}


				if (property != null && property.name == "m_StaticBatching")
				{
					property.boolValue = enable;
				}

				if (property != null && property.name == "m_DynamicBatching")
				{
					property.boolValue = enable;
				}
			}
		}

		playerSettingsSerializedObject.ApplyModifiedProperties();
	}

	public static void SetVRSDKs(string[] sdkNames)
	{
		Debug.Log("Setting virtual reality SDKs in PlayerSettings: ");
		if (sdkNames != null)
		{
			foreach (string s in sdkNames)
				Debug.Log("- " + s);
		}

		PlayerSettings[] playerSettings = Resources.FindObjectsOfTypeAll<PlayerSettings>();
		if (playerSettings == null)
			return;

		SerializedObject playerSettingsSerializedObject = new SerializedObject(playerSettings);
		SerializedProperty settingsGroup = playerSettingsSerializedObject.FindProperty("m_BuildTargetVRSettings");
		if (settingsGroup == null)
			return;

		for (int i = 0;i < settingsGroup.arraySize;i++)
		{
			SerializedProperty settingVal = settingsGroup.GetArrayElementAtIndex(i);
			if (settingVal == null)
				continue;

			IEnumerator enumerator = settingVal.GetEnumerator();
			if (enumerator == null)
				continue;

			while(enumerator.MoveNext())
			{
				SerializedProperty property = (SerializedProperty)enumerator.Current;

				if (property != null && property.name == "m_BuildTarget")
				{
					// only change setting on "Standalone" entry
					if (property.stringValue != "Standalone")
						break;
				}

				if (property != null && property.name == "m_Devices")
				{
					property.ClearArray();
					property.arraySize = (sdkNames != null) ? sdkNames.Length : 0;
					for (int j = 0; j < property.arraySize; j++)
					{
						property.GetArrayElementAtIndex(j).stringValue = sdkNames[j];
					}
				}
			}
		}

		playerSettingsSerializedObject.ApplyModifiedProperties();
	}

	static void RefreshClientVRSDKs()
	{
#if VRC_CLIENT

#if VRC_VR_STEAM
		SetVRSDKs(new string[] { "None", "OpenVR", "Oculus" });
#else
		SetVRSDKs(new string[] { "None", "Oculus", "OpenVR" });
#endif

#endif // VRC_CLIENT
    }

    public static bool CheckForFirstInit()
	{
		bool firstLaunch = UnityEditor.SessionState.GetBool("EnvConfigFirstLaunch", true);  
		if (firstLaunch)
    		UnityEditor.SessionState.SetBool("EnvConfigFirstLaunch", false);

		return firstLaunch;
	}

    static void SetDefaultGraphicsAPIs()
    {
        Debug.Log("Setting Graphics APIs");
        foreach (BuildTarget target in relevantBuildTargets)
        {
            var apis = allowedGraphicsAPIs[target];
            if (apis == null)
                SetGraphicsAPIs(target, true);
            else
                SetGraphicsAPIs(target, false, apis);
        }
    }

    static void SetGraphicsAPIs(BuildTarget platform, bool auto, UnityEngine.Rendering.GraphicsDeviceType[] allowedTypes = null)
    {
        try
        {
            if (auto != PlayerSettings.GetUseDefaultGraphicsAPIs(platform))
                PlayerSettings.SetUseDefaultGraphicsAPIs(platform, auto);
        }
        catch { }

        try
        {
            UnityEngine.Rendering.GraphicsDeviceType[] graphicsAPIs = PlayerSettings.GetGraphicsAPIs(platform);
            if (((allowedTypes == null || allowedTypes.Length == 0) && (graphicsAPIs != null || graphicsAPIs.Length != 0))
                || !allowedTypes.SequenceEqual(graphicsAPIs))
                PlayerSettings.SetGraphicsAPIs(platform, allowedTypes);
        }
        catch { }
    }

    static void SetGraphicsSettings()
    {
        Debug.Log("Setting Graphics Settings");

        const string GraphicsSettingsAssetPath = "ProjectSettings/GraphicsSettings.asset";
        SerializedObject graphicsManager = new SerializedObject(UnityEditor.AssetDatabase.LoadAllAssetsAtPath(GraphicsSettingsAssetPath)[0]);

        SerializedProperty deferred = graphicsManager.FindProperty("m_Deferred.m_Mode");
        deferred.enumValueIndex = 1;

        SerializedProperty deferredReflections = graphicsManager.FindProperty("m_DeferredReflections.m_Mode");
        deferredReflections.enumValueIndex = 1;

        SerializedProperty screenSpaceShadows = graphicsManager.FindProperty("m_ScreenSpaceShadows.m_Mode");
        screenSpaceShadows.enumValueIndex = 1;

        SerializedProperty legacyDeferred = graphicsManager.FindProperty("m_LegacyDeferred.m_Mode");
        legacyDeferred.enumValueIndex = 1;

        SerializedProperty depthNormals = graphicsManager.FindProperty("m_DepthNormals.m_Mode");
        depthNormals.enumValueIndex = 1;

        SerializedProperty motionVectors = graphicsManager.FindProperty("m_MotionVectors.m_Mode");
        motionVectors.enumValueIndex = 1;

        SerializedProperty lightHalo = graphicsManager.FindProperty("m_LightHalo.m_Mode");
        lightHalo.enumValueIndex = 1;

        SerializedProperty lensFlare = graphicsManager.FindProperty("m_LensFlare.m_Mode");
        lensFlare.enumValueIndex = 1;

#if ENV_SET_INCLUDED_SHADERS
        SerializedProperty alwaysIncluded = graphicsManager.FindProperty("m_AlwaysIncludedShaders");

        System.Collections.Generic.List<Shader> foundShaders = minimalShaders.Aggregate(new System.Collections.Generic.List<Shader>(), (l, name) => { var s = Shader.Find(name); if (s) l.Add(s); return l; });

        for (int shaderIdx = 0; shaderIdx < alwaysIncluded.arraySize; ++shaderIdx)
        {
            Shader s = alwaysIncluded.GetArrayElementAtIndex(shaderIdx).objectReferenceValue as Shader;
            if (s != null && foundShaders.Contains(s))
                foundShaders.Remove(s);
        }

        {
            int endIdx = alwaysIncluded.arraySize;
            alwaysIncluded.arraySize += foundShaders.Count;
            for (int shaderIdx = endIdx; shaderIdx < alwaysIncluded.arraySize; ++shaderIdx)
                alwaysIncluded.GetArrayElementAtIndex(shaderIdx).objectReferenceValue = foundShaders[shaderIdx - endIdx];
        }
#endif

        SerializedProperty preloaded = graphicsManager.FindProperty("m_PreloadedShaders");
        preloaded.ClearArray();
        preloaded.arraySize = 0;

        SerializedProperty spritesDefaultMaterial = graphicsManager.FindProperty("m_SpritesDefaultMaterial");
        spritesDefaultMaterial.objectReferenceValue = Shader.Find("Sprites/Default");

        SerializedProperty renderPipeline = graphicsManager.FindProperty("m_CustomRenderPipeline");
        renderPipeline.objectReferenceValue = null;

        SerializedProperty transparencySortMode = graphicsManager.FindProperty("m_TransparencySortMode");
        transparencySortMode.enumValueIndex = 0;

        SerializedProperty transparencySortAxis = graphicsManager.FindProperty("m_TransparencySortAxis");
        transparencySortAxis.vector3Value = Vector3.forward;
        
        SerializedProperty defaultRenderingPath = graphicsManager.FindProperty("m_DefaultRenderingPath");
        defaultRenderingPath.intValue = 1;

        SerializedProperty defaultMobileRenderingPath = graphicsManager.FindProperty("m_DefaultMobileRenderingPath");
        defaultMobileRenderingPath.intValue = 1;

        SerializedProperty tierSettings = graphicsManager.FindProperty("m_TierSettings");
        tierSettings.ClearArray();
        tierSettings.arraySize = 0;

#if ENV_SET_LIGHTMAP
        SerializedProperty lightmapStripping = graphicsManager.FindProperty("m_LightmapStripping");
        lightmapStripping.enumValueIndex = 1;

        SerializedProperty instancingStripping = graphicsManager.FindProperty("m_InstancingStripping");
        instancingStripping.enumValueIndex = 2;

        SerializedProperty lightmapKeepPlain = graphicsManager.FindProperty("m_LightmapKeepPlain");
        lightmapKeepPlain.boolValue = true;

        SerializedProperty lightmapKeepDirCombined = graphicsManager.FindProperty("m_LightmapKeepDirCombined");
        lightmapKeepDirCombined.boolValue = true;

        SerializedProperty lightmapKeepDynamicPlain = graphicsManager.FindProperty("m_LightmapKeepDynamicPlain");
        lightmapKeepDynamicPlain.boolValue = true;

        SerializedProperty lightmapKeepDynamicDirCombined = graphicsManager.FindProperty("m_LightmapKeepDynamicDirCombined");
        lightmapKeepDynamicDirCombined.boolValue = true;

        SerializedProperty lightmapKeepShadowMask = graphicsManager.FindProperty("m_LightmapKeepShadowMask");
        lightmapKeepShadowMask.boolValue = true;

        SerializedProperty lightmapKeepSubtractive = graphicsManager.FindProperty("m_LightmapKeepSubtractive");
        lightmapKeepSubtractive.boolValue = true;
#endif

#if ENV_SET_FOG
        SerializedProperty fogStripping = graphicsManager.FindProperty("m_FogStripping");
        fogStripping.enumValueIndex = 1;

        SerializedProperty fogKeepLinear = graphicsManager.FindProperty("m_FogKeepLinear");
        fogKeepLinear.boolValue = true;

        SerializedProperty fogKeepExp = graphicsManager.FindProperty("m_FogKeepExp");
        fogKeepExp.boolValue = true;

        SerializedProperty fogKeepExp2 = graphicsManager.FindProperty("m_FogKeepExp2");
        fogKeepExp2.boolValue = true;
#endif

        SerializedProperty albedoSwatchInfos = graphicsManager.FindProperty("m_AlbedoSwatchInfos");
        albedoSwatchInfos.ClearArray();
        albedoSwatchInfos.arraySize = 0;

        SerializedProperty lightsUseLinearIntensity = graphicsManager.FindProperty("m_LightsUseLinearIntensity");
        lightsUseLinearIntensity.boolValue = true;

        SerializedProperty lightsUseColorTemperature = graphicsManager.FindProperty("m_LightsUseColorTemperature");
        lightsUseColorTemperature.boolValue = true;

        graphicsManager.ApplyModifiedProperties();
    }

    static void SetPlayerSettings()
    {
        // asset bundles MUST be built with settings that are compatible with VRC client
        PlayerSettings.colorSpace = ColorSpace.Linear;
        PlayerSettings.virtualRealitySupported = true;
        PlayerSettings.stereoRenderingPath = StereoRenderingPath.SinglePass;

        EnableBatching(true);
    }

    static void SetBuildTarget()
    {
        Debug.Log("Setting build target");

        BuildTarget target = UnityEditor.EditorUserBuildSettings.activeBuildTarget;

        if (!allowedTargets.Contains(target))
        {
            Debug.LogError("Target not supported, switching to one that is.");
            target = allowedTargets[0];
#pragma warning disable CS0618 // Type or member is obsolete
            EditorUserBuildSettings.SwitchActiveBuildTarget(target);
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }               
}
