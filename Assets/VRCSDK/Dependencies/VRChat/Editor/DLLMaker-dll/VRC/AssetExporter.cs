using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRC.Core;
using VRCSDK2;

namespace VRC
{
	public class AssetExporter : MonoBehaviour
	{
		public static int mNumClientsToLaunch = 1;

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

		public AssetExporter()
			: this()
		{
		}

		public static void FinishExportCurrentSceneResourceWithPlugin()
		{
			try
			{
				ExportCurrentSceneResource();
				if (!CustomDLLMaker.Cleanup())
				{
					EditorAssemblies.AddOnAssemblyReloadCallback("CustomDLLMaker", "Cleanup");
					EditorAssemblies.ReloadAssemblies();
				}
			}
			catch (Exception ex)
			{
				Debug.LogError((object)(ex.ToString() + "\n" + ex.StackTrace));
				throw ex;
				IL_0047:;
			}
		}

		public static void ExportCurrentSceneAsUnityPackage()
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			Debug.Log((object)"ExpringCurrentSceneAsUnityPackage");
			Scene activeScene = SceneManager.GetActiveScene();
			string path = activeScene.get_path();
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
			string dstPath = Application.get_dataPath() + "/../" + fileNameWithoutExtension + ".unitypackage";
			List<string> list = new List<string>();
			list.Add(path);
			list.Add("Assets/VRCScripts");
			ExportCurrentAssetAsUnityPackage(list, dstPath);
		}

		public static void ExportCurrentAssetAsUnityPackage(List<string> assetPaths, string dstPath)
		{
			EditorUtility.DisplayProgressBar("Exporting unity package", "Future proofing your content!", 1f);
			Debug.Log((object)("Exporting to " + dstPath));
			if (!assetPaths.Contains("Assets/VRCSDK"))
			{
				assetPaths.Add("Assets/VRCSDK");
			}
			EditorPrefs.SetString("VRC_exportedUnityPackagePath", dstPath);
			AssetDatabase.ExportPackage(assetPaths.ToArray(), dstPath, 14);
			EditorUtility.ClearProgressBar();
		}

		public static void CleanupUnityPackageExport()
		{
			EditorPrefs.DeleteKey("VRC_exportedUnityPackagePath");
		}

		public static void ExportCurrentSceneResource()
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			object text;
			if (EditorPrefs.HasKey("originalScenePath"))
			{
				text = EditorPrefs.GetString("originalScenePath");
			}
			else
			{
				Scene activeScene = SceneManager.GetActiveScene();
				text = activeScene.get_path();
			}
			string path = (string)text;
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
			string text2 = null;
			VRC_SceneDescriptor[] array = Tools.FindSceneObjectsOfTypeAll<VRC_SceneDescriptor>();
			if (array == null || array.Length != 1)
			{
				EditorUtility.DisplayDialog("Build Custom Scene", "Scene needs exactly one VRC_SceneDescriptor", "Ok");
			}
			else
			{
				VRC_SceneDescriptor val = array[0];
				if (val == null)
				{
					EditorUtility.DisplayDialog("Build Custom Scene", "You must place a VRC_SceneDescriptor on the root of you custom scene", "Ok");
				}
				else if (val.spawns.Length < 1)
				{
					EditorUtility.DisplayDialog("Build Custom Scene", "You must add at least one spawn to spawns in your VRC_SceneDescriptor.", "Ok");
				}
				else
				{
					ListDynamicPrefabs(val.DynamicPrefabs, null);
					val.gravity = Physics.get_gravity();
					val.LoadRenderSettings = false;
					val.layerCollisionArr = UpdateLayers.GetLayerCollisionArray();
					val.unityVersion = Application.get_unityVersion();
					EditorUtility.SetDirty(val);
					EditorSceneManager.MarkSceneDirty(val.get_gameObject().get_scene());
					EditorSceneManager.SaveScene(val.get_gameObject().get_scene());
					try
					{
						Scene activeScene2 = SceneManager.GetActiveScene();
						string text3 = activeScene2.get_path();
						string text4 = fileNameWithoutExtension;
						bool flag = false;
						if (text4.Contains("."))
						{
							flag = true;
							string text5 = "Assets/customTmpScene12345.unity";
							AssetDatabase.CopyAsset(text3, text5);
							text3 = text5;
							AssetDatabase.Refresh();
						}
						AssetDatabase.RemoveUnusedAssetBundleNames();
						string text6 = "customscene.vrcw";
						AssetImporter atPath = AssetImporter.GetAtPath(text3);
						atPath.set_assetBundleName(text6);
						atPath.SaveAndReimport();
						text2 = Application.get_temporaryCachePath() + "/" + text6;
						BuildTarget activeBuildTarget = EditorUserBuildSettings.get_activeBuildTarget();
						BuildPipeline.BuildAssetBundles(Application.get_temporaryCachePath(), 0, 5);
						EditorUserBuildSettings.SwitchActiveBuildTarget(activeBuildTarget);
						EditorPrefs.SetString("currentBuildingAssetBundlePath", text2);
						EditorPrefs.SetString("lastVRCPath", text2);
						atPath.set_assetBundleName(string.Empty);
						atPath.SaveAndReimport();
						if (flag)
						{
							AssetDatabase.DeleteAsset(text3);
						}
						AssetDatabase.RemoveUnusedAssetBundleNames();
					}
					catch (Exception ex)
					{
						Debug.LogError((object)("Export Exception - " + ex.ToString()));
						throw ex;
						IL_0206:;
					}
					if (text2 != null)
					{
						VRC_Editor.RecordActivity("scene", Path.GetFileName(text2));
					}
				}
			}
		}

		public static void RunExportedSceneResourceAndCleanupPlugin()
		{
			RunExportedSceneResource();
			CustomDLLMaker.Cleanup();
		}

		private static void RunExportedSceneResource()
		{
			string @string = EditorPrefs.GetString("currentBuildingAssetBundlePath");
			if (!string.IsNullOrEmpty(@string))
			{
				string string2 = EditorPrefs.GetString("externalPluginPath");
				EditorPrefs.DeleteKey("externalPluginPath");
				RunScene(@string, string2);
			}
		}

		public static void RunScene(string vrcFilePath, string pluginFilePath)
		{
			string str = WWW.EscapeURL(vrcFilePath).Replace("+", "%20");
			string text = WWW.EscapeURL(pluginFilePath).Replace("+", "%20");
			string randomDigits = Tools.GetRandomDigits(10);
			string text2 = SDKClientUtilities.GetSavedVRCInstallPath();
			if (string.IsNullOrEmpty(text2))
			{
				text2 = "vrchat://create?roomId=" + randomDigits + "&hidden=true&name=BuildAndRun&url=file:///" + str;
				if (!string.IsNullOrEmpty(text))
				{
					text2 = text2 + "&pluginUrl=file:///" + text;
				}
			}
			string text3 = "--url=create?roomId=" + randomDigits + "&hidden=true&name=BuildAndRun&url=file:///" + str;
			if (!string.IsNullOrEmpty(text))
			{
				text3 = text3 + "&pluginUrl=file:///" + text;
			}
			for (int i = 0; i < numClientsToLaunch; i++)
			{
				Process.Start(text2, text3);
			}
		}

		public static void LaunchSceneBlueprintUploader()
		{
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			string @string = EditorPrefs.GetString("currentBuildingAssetBundlePath");
			if (!string.IsNullOrEmpty(@string))
			{
				EditorApplication.set_isPlaying(true);
				VRCPipelineManagerEditor.launchedFromSDKPipeline = true;
				PipelineManager[] array = Object.FindObjectsOfType<PipelineManager>();
				if (array.Length > 0)
				{
					PipelineManager val = array[0];
					PipelineSaver val2 = val.get_gameObject().AddComponent<PipelineSaver>();
					val2.contentType = 0;
				}
			}
			CustomDLLMaker.Cleanup();
		}

		private static void ListDynamicPrefabs(List<GameObject> list, GameObject obj)
		{
			VRC_ObjectSpawn[] array = (!(obj != null)) ? Tools.FindSceneObjectsOfTypeAll<VRC_ObjectSpawn>() : obj.GetComponents<VRC_ObjectSpawn>();
			VRC_ObjectSpawn[] array2 = array;
			foreach (VRC_ObjectSpawn val in array2)
			{
				if (!list.Contains(val.ObjectPrefab))
				{
					list.Add(val.ObjectPrefab);
				}
			}
			if (obj != null)
			{
				for (int j = 0; j < obj.get_transform().get_childCount(); j++)
				{
					ListDynamicPrefabs(list, obj.get_transform().GetChild(j).get_gameObject());
				}
			}
		}

		public static void CleanupTmpFiles()
		{
			if (File.Exists(EditorAssemblies.SERIALIZATION_PATH))
			{
				File.Delete(EditorAssemblies.SERIALIZATION_PATH);
			}
			if (File.Exists(EditorAssemblies.TMP_FILE_PATH))
			{
				File.Delete(EditorAssemblies.TMP_FILE_PATH);
			}
			CustomDLLMaker.Cleanup();
		}
	}
}
