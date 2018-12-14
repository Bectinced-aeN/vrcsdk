using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using VRC;
using VRC.Core;
using VRCSDK2;

public class VRCSdkBuilder : MonoBehaviour
{
	private static string vrcPath;

	public static bool shouldBuildUnityPackage;

	public static int mNumClientsToLaunch = 1;

	private static string RandomRoomDigits;

	public static int numClientsToLaunch
	{
		get
		{
			mNumClientsToLaunch = EditorPrefs.GetInt("VRC_numClientsToLaunch");
			return mNumClientsToLaunch;
		}
		set
		{
			mNumClientsToLaunch = value;
			EditorPrefs.SetInt("VRC_numClientsToLaunch", mNumClientsToLaunch);
		}
	}

	public VRCSdkBuilder()
		: this()
	{
	}

	private static bool VerifyCredentials()
	{
		if (!RemoteConfig.IsInitialized())
		{
			RemoteConfig.Init(true, (Action)null, (Action)null);
		}
		if (!APIUser.get_IsLoggedInWithCredentials() && APIUser.get_IsCached())
		{
			APIUser.CachedLogin((Action<APIUser>)null, (Action<string>)null, false);
		}
		if (!APIUser.get_IsLoggedInWithCredentials())
		{
			AccountEditorWindow.Init();
			return false;
		}
		return true;
	}

	public static void ExportAndUploadAvatarBlueprint(GameObject externalReference = null)
	{
		if (VerifyCredentials())
		{
			EditorPrefs.DeleteKey("externalPluginPath");
			ExportCurrentAvatarResource(externalReference);
			UploadAvatarResource();
		}
	}

	private static void UploadAvatarResource()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (!string.IsNullOrEmpty(vrcPath))
		{
			VRC_AvatarDescriptor component = Selection.get_activeObject().GetComponent<VRC_AvatarDescriptor>();
			PipelineSaver val = component.get_gameObject().AddComponent<PipelineSaver>();
			val.contentType = 0;
			EditorApplication.set_isPlaying(true);
			VRCPipelineManagerEditor.launchedFromSDKPipeline = true;
		}
	}

	private static void ExportCurrentAvatarResource(GameObject avatarResource = null)
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		if (avatarResource != null)
		{
			Selection.set_activeObject(avatarResource);
		}
		vrcPath = null;
		if (Selection.get_activeObject() == null)
		{
			EditorUtility.DisplayDialog("Build Custom Avatar", "You must select the avatar root gameobject you want to build.", "Ok");
		}
		else
		{
			VRC_AvatarDescriptor component = (Selection.get_activeObject() as GameObject).GetComponent<VRC_AvatarDescriptor>();
			if (component == null)
			{
				EditorUtility.DisplayDialog("Build Custom Avatar", "You must place a VRC_AvatarDescriptor on the root of you custom scene", "Ok");
			}
			else
			{
				component.unityVersion = Application.get_unityVersion();
				EditorUtility.SetDirty(component);
				PipelineManager component2 = component.get_gameObject().GetComponent<PipelineManager>();
				if (component2 != null)
				{
					component2.contentType = 0;
					EditorUtility.SetDirty(component2);
					EditorSceneManager.MarkSceneDirty(component2.get_gameObject().get_scene());
					EditorSceneManager.SaveScene(component2.get_gameObject().get_scene());
					PrefabUtility.CreatePrefab("Assets/_CustomAvatar.prefab", Selection.get_activeObject() as GameObject);
					if (shouldBuildUnityPackage)
					{
						Debug.Log((object)"Got inshide the build thing");
						List<string> list = new List<string>();
						list.Add("Assets/_CustomAvatar.prefab");
						string dstPath = Application.get_dataPath() + "/../" + Selection.get_activeObject().get_name() + ".unitypackage";
						AssetExporter.ExportCurrentAssetAsUnityPackage(list, dstPath);
					}
					else
					{
						AssetExporter.CleanupUnityPackageExport();
					}
					AssetBundleBuild val = default(AssetBundleBuild);
					val.assetNames = new string[1]
					{
						"Assets/_CustomAvatar.prefab"
					};
					val.assetBundleName = "customAvatar.unity3d";
					string sourceFileName = Application.get_temporaryCachePath() + "/" + val.assetBundleName;
					vrcPath = Application.get_temporaryCachePath() + "/custom.vrca";
					BuildTarget activeBuildTarget = EditorUserBuildSettings.get_activeBuildTarget();
					BuildPipeline.BuildAssetBundles(Application.get_temporaryCachePath(), (AssetBundleBuild[])new AssetBundleBuild[1]
					{
						val
					}, 0, 5);
					if (File.Exists(vrcPath))
					{
						File.Delete(vrcPath);
					}
					File.Move(sourceFileName, vrcPath);
					EditorPrefs.SetString("currentBuildingAssetBundlePath", vrcPath);
					EditorUserBuildSettings.SwitchActiveBuildTarget(activeBuildTarget);
					AssetDatabase.DeleteAsset("Assets/_CustomAvatar.prefab");
					AssetDatabase.Refresh();
					VRC_Editor.RecordActivity("avatar", Path.GetFileName(vrcPath));
				}
				else
				{
					EditorUtility.DisplayDialog("Could not find PipelineManager", "Your scene does not contain a PipelineManager. It should be auto added by the AvatarDescriptor.", "Ok");
				}
			}
		}
	}

	public static void UploadLastExportedSceneBlueprint()
	{
		if (VerifyCredentials())
		{
			string @string = EditorPrefs.GetString("lastExternalPluginPath");
			string text = WWW.UnEscapeURL(EditorPrefs.GetString("lastVRCPath"));
			bool flag = false;
			if (string.IsNullOrEmpty(text) || (!string.IsNullOrEmpty(text) && !File.Exists(text)))
			{
				flag = true;
			}
			else if (CustomDLLMaker.CustomScriptsAvailable() && !string.IsNullOrEmpty(@string) && !File.Exists(@string))
			{
				flag = true;
			}
			if (!flag)
			{
				EditorPrefs.SetString("currentBuildingAssetBundlePath", text);
				if (APIUser.get_CurrentUser().get_hasScriptingAccess() && CustomDLLMaker.CustomScriptsAvailable())
				{
					EditorPrefs.SetString("externalPluginPath", @string);
				}
				else
				{
					EditorPrefs.DeleteKey("externalPluginPath");
				}
				if (shouldBuildUnityPackage)
				{
					AssetExporter.ExportCurrentSceneAsUnityPackage();
				}
				else
				{
					AssetExporter.CleanupUnityPackageExport();
				}
				AssetExporter.LaunchSceneBlueprintUploader();
			}
			else
			{
				EditorUtility.DisplayDialog("Could not run VRChat scene", "Last built VRChat scene could not be found. Please Test/Compile Full Scene (slow).", "OK");
			}
		}
	}

	public static void ExportAndUploadSceneBlueprint()
	{
		if (VerifyCredentials())
		{
			ExportSceneAndPrepareForUpload();
		}
	}

	private static void RecompileSceneScripts()
	{
		if (CustomDLLMaker.CustomScriptsAvailable())
		{
			string @string = EditorPrefs.GetString("lastExternalPluginPath");
			string text = WWW.UnEscapeURL(EditorPrefs.GetString("lastVRCPath"));
			bool flag = false;
			if (string.IsNullOrEmpty(text) || (!string.IsNullOrEmpty(text) && !File.Exists(text)))
			{
				flag = true;
			}
			else if (!string.IsNullOrEmpty(@string) && !File.Exists(@string))
			{
				flag = true;
			}
			if (!flag)
			{
				CustomDLLMaker.BuildAndLoadPlugin(recompileCurrentPlugin: true);
			}
			else
			{
				EditorUtility.DisplayDialog("Could not recompile scripts", "Something went wrong. Please Test/Compile Full Scene (slow)", "OK");
			}
		}
	}

	public static void RunLastExportedSceneResource()
	{
		string @string = EditorPrefs.GetString("lastExternalPluginPath");
		string string2 = EditorPrefs.GetString("lastVRCPath");
		bool flag = false;
		if (string.IsNullOrEmpty(string2) || (!string.IsNullOrEmpty(string2) && !File.Exists(string2)))
		{
			flag = true;
		}
		else if (CustomDLLMaker.CustomScriptsAvailable() && !string.IsNullOrEmpty(@string) && !File.Exists(@string))
		{
			flag = true;
		}
		if (!flag)
		{
			if (CustomDLLMaker.CustomScriptsAvailable())
			{
				RecompileSceneScripts();
			}
			AssetExporter.RunScene(string2, @string);
		}
		else
		{
			EditorUtility.DisplayDialog("Could not run VRChat scene", "Last built VRChat scene could not be found. Please Test/Compile Full Scene (slow).", "OK");
		}
	}

	public static void ExportSceneResourceAndRun()
	{
		try
		{
			if (ExportSceneResource())
			{
				EditorAssemblies.AddOnAssemblyReloadCallback("VRC.AssetExporter", "RunExportedSceneResourceAndCleanupPlugin");
			}
			else
			{
				AssetExporter.RunExportedSceneResourceAndCleanupPlugin();
			}
		}
		catch (Exception ex)
		{
			AssetExporter.CleanupTmpFiles();
			EditorAssemblies.ClearAssemblyReloadCallbacks();
			throw ex;
			IL_0038:;
		}
	}

	private static void ExportSceneAndPrepareForUpload()
	{
		try
		{
			if (shouldBuildUnityPackage)
			{
				AssetExporter.ExportCurrentSceneAsUnityPackage();
			}
			else
			{
				AssetExporter.CleanupUnityPackageExport();
			}
			if (ExportSceneResource())
			{
				if (!APIUser.get_CurrentUser().get_hasScriptingAccess() && CustomDLLMaker.CustomScriptsAvailable())
				{
					EditorPrefs.DeleteKey("externalPluginPath");
				}
				EditorAssemblies.AddOnAssemblyReloadCallback("CustomDLLMaker", "Cleanup");
				EditorAssemblies.AddOnAssemblyReloadCallback("VRC.AssetExporter", "LaunchSceneBlueprintUploader");
			}
			else
			{
				EditorPrefs.DeleteKey("externalPluginPath");
				AssetExporter.LaunchSceneBlueprintUploader();
			}
		}
		catch (Exception ex)
		{
			AssetExporter.CleanupTmpFiles();
			EditorAssemblies.ClearAssemblyReloadCallbacks();
			throw ex;
			IL_008c:;
		}
	}

	public static string GetLastUrl()
	{
		if (string.IsNullOrEmpty(EditorPrefs.GetString("lastVRCPath")))
		{
			return null;
		}
		string @string = EditorPrefs.GetString("lastExternalPluginPath");
		string string2 = EditorPrefs.GetString("lastVRCPath");
		string str = WWW.EscapeURL(string2).Replace("+", "%20");
		string text = WWW.EscapeURL(@string).Replace("+", "%20");
		if (string.IsNullOrEmpty(RandomRoomDigits))
		{
			RandomRoomDigits = Tools.GetRandomDigits(10);
		}
		string randomRoomDigits = RandomRoomDigits;
		string str2 = "vrchat://create?roomId=" + randomRoomDigits;
		str2 = str2 + "&url=file:///" + str;
		if (!string.IsNullOrEmpty(text))
		{
			str2 = str2 + "&pluginUrl=file:///" + text;
		}
		return str2;
	}

	public static bool ExportSceneResource()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		PipelineManager[] array = Object.FindObjectsOfType<PipelineManager>();
		if (array.Length > 0)
		{
			PipelineManager val = array[0];
			val.contentType = 1;
			EditorUtility.SetDirty(array[0]);
			EditorSceneManager.MarkSceneDirty(val.get_gameObject().get_scene());
			EditorSceneManager.SaveScene(val.get_gameObject().get_scene());
		}
		if (CustomDLLMaker.CustomScriptsAvailable())
		{
			ExportCurrentSceneResourceWithPlugin();
			return true;
		}
		AssetExporter.ExportCurrentSceneResource();
		return false;
	}

	private static void ExportCurrentSceneResourceWithPlugin()
	{
		CustomDLLMaker.PrepareSceneForExport();
		EditorAssemblies.AddOnAssemblyReloadCallback("VRC.AssetExporter", "FinishExportCurrentSceneResourceWithPlugin");
	}

	[MenuItem("VRChat SDK/Clean Excess SDK References")]
	private static void CleanExcessReferences()
	{
		VRC_ReflectionReference[] array = Tools.FindSceneObjectsOfTypeAll<VRC_ReflectionReference>();
		VRC_ReflectionReference[] array2 = array;
		foreach (VRC_ReflectionReference val in array2)
		{
			Object.DestroyImmediate(val);
		}
		VRC_LightmapReference[] array3 = Tools.FindSceneObjectsOfTypeAll<VRC_LightmapReference>();
		VRC_LightmapReference[] array4 = array3;
		foreach (VRC_LightmapReference val2 in array4)
		{
			Object.DestroyImmediate(val2);
		}
		VRC_StaticReference[] array5 = Tools.FindSceneObjectsOfTypeAll<VRC_StaticReference>();
		VRC_StaticReference[] array6 = array5;
		foreach (VRC_StaticReference val3 in array6)
		{
			Object.DestroyImmediate(val3);
		}
	}

	[MenuItem("VRChat SDK/Clear Cache and PlayerPrefs")]
	private static void ClearPlayerPrefs()
	{
		Tools.ClearUserData();
	}
}
