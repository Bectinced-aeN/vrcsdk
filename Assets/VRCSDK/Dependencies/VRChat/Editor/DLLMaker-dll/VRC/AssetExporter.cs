using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

		public static void ExportCurrentSceneAsUnityPackageIfNotExist()
		{
			if (File.Exists(GetActiveSceneUnityPackageExportPath()) && string.Compare(EditorPrefs.GetString("VRC_exportedUnityPackagePath", string.Empty), GetActiveSceneUnityPackageExportPath(), ignoreCase: true) == 0)
			{
				Debug.Log((object)("Exported package already up to date: " + GetActiveSceneUnityPackageExportPath()));
			}
			else
			{
				ExportCurrentSceneAsUnityPackage();
			}
		}

		public static void ExportCurrentSceneAsUnityPackage()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			Scene activeScene = SceneManager.GetActiveScene();
			string path = activeScene.get_path();
			List<string> list = new List<string>();
			list.Add(path);
			list.Add("Assets/VRCScripts");
			ExportCurrentAssetAsUnityPackage(list, GetActiveSceneUnityPackageExportPath());
		}

		public static void ExportCurrentAssetAsUnityPackage(List<string> assetPaths, string dstPath)
		{
			Debug.Log((object)("Exporting package to: " + dstPath));
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

		public static string GetActiveSceneUnityPackageExportPath()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			Scene activeScene = SceneManager.GetActiveScene();
			string path = activeScene.get_path();
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
			return Path.GetDirectoryName(Application.get_dataPath()) + Path.DirectorySeparatorChar + fileNameWithoutExtension + ".unitypackage";
		}

		public static void ExportCurrentSceneResource()
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
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
					FindDynamicContent(val);
					val.gravity = Physics.get_gravity();
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
						BuildTargetGroup selectedBuildTargetGroup = EditorUserBuildSettings.get_selectedBuildTargetGroup();
						BuildPipeline.BuildAssetBundles(Application.get_temporaryCachePath(), 0, EditorUserBuildSettings.get_activeBuildTarget());
						EditorUserBuildSettings.SwitchActiveBuildTarget(selectedBuildTargetGroup, activeBuildTarget);
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
						IL_0205:;
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
			if (string.IsNullOrEmpty(text2) || !File.Exists(text2))
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
			ProcessStartInfo processStartInfo = new ProcessStartInfo(text2, text3);
			processStartInfo.WorkingDirectory = Path.GetDirectoryName(text2);
			for (int i = 0; i < numClientsToLaunch; i++)
			{
				Process.Start(processStartInfo);
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
					val2.contentType = 1;
				}
			}
			CustomDLLMaker.Cleanup();
		}

		private static void FindDynamicContent(VRC_SceneDescriptor desc)
		{
			List<GameObject> prefabs = new List<GameObject>();
			List<Material> materials = new List<Material>();
			FindDynamicPrefabsInScene(ref prefabs);
			FindMaterialsAndPrefabsInScene(ref prefabs, ref materials);
			FindMaterialsOnObjects(prefabs, ref prefabs, ref materials);
			while (FindPrefabReferencesOnPrefabs(ref prefabs))
			{
			}
			desc.DynamicMaterials = materials;
			desc.DynamicPrefabs = prefabs;
			Debug.LogFormat("Found {0} prefabs and {1} materials", new object[2]
			{
				prefabs.Count,
				materials.Count
			});
		}

		private static void AddAlwaysIncludedShaders(IEnumerable<Shader> shaders)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Expected O, but got Unknown
			SerializedObject val = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/GraphicsSettings.asset")[0]);
			SerializedProperty val2 = val.FindProperty("m_AlwaysIncludedShaders");
			List<Shader> list = (from s in shaders
			where s != null && !s.get_name().Contains("Standard")
			select s).ToList();
			for (int i = 0; i < val2.get_arraySize(); i++)
			{
				Shader val3 = val2.GetArrayElementAtIndex(i).get_objectReferenceValue() as Shader;
				if (val3 != null && list.Contains(val3))
				{
					list.Remove(val3);
				}
			}
			int arraySize = val2.get_arraySize();
			SerializedProperty obj = val2;
			obj.set_arraySize(obj.get_arraySize() + list.Count);
			for (int j = arraySize; j < val2.get_arraySize(); j++)
			{
				Debug.LogFormat("Adding {0} to Always Included Shaders", new object[1]
				{
					list[j - arraySize].get_name()
				});
				val2.GetArrayElementAtIndex(j).set_objectReferenceValue(list[j - arraySize]);
			}
			val.ApplyModifiedProperties();
		}

		private static void FindMaterialsAndPrefabsInScene(ref List<GameObject> prefabs, ref List<Material> materials)
		{
			FindMaterialsOnObjects(Object.FindObjectsOfType<GameObject>(), ref prefabs, ref materials);
		}

		private static void FindMaterialsOnObjects(IEnumerable<GameObject> objs, ref List<GameObject> prefabs, ref List<Material> materials)
		{
			foreach (GameObject obj in objs)
			{
				if (!(obj == null))
				{
					VRC_Trigger[] componentsInChildren = obj.GetComponentsInChildren<VRC_Trigger>();
					VRC_Trigger[] array = componentsInChildren;
					foreach (VRC_Trigger val in array)
					{
						foreach (TriggerEvent trigger in val.Triggers)
						{
							foreach (VrcEvent item in from evt in trigger.Events
							where (int)evt.EventType == 18
							select evt)
							{
								Material val2 = AssetDatabase.LoadAssetAtPath<Material>(item.ParameterString);
								if (val2 != null && !val2.get_name().Contains("(Instance)") && !materials.Contains(val2))
								{
									materials.Add(val2);
								}
							}
						}
					}
					Renderer[] componentsInChildren2 = obj.GetComponentsInChildren<Renderer>();
					Renderer[] array2 = componentsInChildren2;
					foreach (Renderer val3 in array2)
					{
						Material[] sharedMaterials = val3.get_sharedMaterials();
						foreach (Material val4 in sharedMaterials)
						{
							if (val4 != null && !val4.get_name().Contains("(Instance)") && !materials.Contains(val4))
							{
								materials.Add(val4);
							}
						}
					}
					Terrain[] componentsInChildren3 = obj.GetComponentsInChildren<Terrain>();
					Terrain[] array3 = componentsInChildren3;
					foreach (Terrain val5 in array3)
					{
						if (val5.get_materialTemplate() != null && !val5.get_materialTemplate().get_name().Contains("(Instance)") && !materials.Contains(val5.get_materialTemplate()))
						{
							materials.Add(val5.get_materialTemplate());
						}
						if (val5.get_terrainData() != null)
						{
							if (val5.get_terrainData().get_treePrototypes() != null)
							{
								IEnumerable<GameObject> collection = from p in val5.get_terrainData().get_treePrototypes()
								select p.get_prefab() into o
								where o != null
								select o;
								prefabs.AddRange(collection);
							}
							if (val5.get_terrainData().get_detailPrototypes() != null)
							{
								IEnumerable<GameObject> collection2 = from p in val5.get_terrainData().get_detailPrototypes()
								select p.get_prototype() into o
								where o != null
								select o;
								prefabs.AddRange(collection2);
							}
						}
					}
				}
			}
		}

		private static void FindDynamicPrefabsInScene(ref List<GameObject> prefabs, GameObject rootObj = null)
		{
			VRC_ObjectSpawn[] array = (!(rootObj != null)) ? Tools.FindSceneObjectsOfTypeAll<VRC_ObjectSpawn>() : rootObj.GetComponents<VRC_ObjectSpawn>();
			VRC_ObjectSpawn[] array2 = array;
			foreach (VRC_ObjectSpawn val in array2)
			{
				if (!prefabs.Contains(val.ObjectPrefab))
				{
					prefabs.Add(val.ObjectPrefab);
				}
			}
			if (rootObj != null)
			{
				for (int j = 0; j < rootObj.get_transform().get_childCount(); j++)
				{
					FindDynamicPrefabsInScene(ref prefabs, rootObj.get_transform().GetChild(j).get_gameObject());
				}
			}
			VRC_Trigger[] array3 = (!(rootObj != null)) ? Tools.FindSceneObjectsOfTypeAll<VRC_Trigger>() : rootObj.GetComponents<VRC_Trigger>();
			VRC_Trigger[] array4 = array3;
			foreach (VRC_Trigger val2 in array4)
			{
				foreach (TriggerEvent trigger in val2.Triggers)
				{
					foreach (VrcEvent item in from evt in trigger.Events
					where (int)evt.EventType == 13
					select evt)
					{
						GameObject val3 = AssetDatabase.LoadAssetAtPath(item.ParameterString, typeof(GameObject)) as GameObject;
						if (val3 != null && !prefabs.Contains(val3))
						{
							prefabs.Add(val3);
						}
					}
					foreach (VrcEvent item2 in from evt in trigger.Events
					where (int)evt.EventType == 10 && 0 != (int)evt.ParameterBoolOp
					select evt)
					{
						if (item2.ParameterObjects != null)
						{
							GameObject[] parameterObjects = item2.ParameterObjects;
							foreach (GameObject val4 in parameterObjects)
							{
								if (!(null == val4) && !prefabs.Contains(val4))
								{
									prefabs.Add(val4);
								}
							}
						}
					}
				}
			}
		}

		private static void FindDynamicContentInProject(ref List<GameObject> prefabs, ref List<Material> materials, string rootPath)
		{
			if (Directory.Exists(rootPath))
			{
				string[] files = Directory.GetFiles(rootPath);
				foreach (string text in files)
				{
					string text2 = "Assets" + Path.DirectorySeparatorChar + text.Substring((Application.get_dataPath() + Path.DirectorySeparatorChar).Length);
					if (text2.Contains("Assets/Resources") && text2.Contains("Assets/StreamingAssets"))
					{
						string text3 = text.ToLower();
						if (text3.EndsWith(".prefab"))
						{
							GameObject val = AssetDatabase.LoadMainAssetAtPath(text2) as GameObject;
							if (val != null)
							{
								prefabs.Add(val);
							}
							else
							{
								Debug.LogError((object)("Could not load " + text2));
							}
						}
						else if (text3.EndsWith(".mat"))
						{
							Material val2 = AssetDatabase.LoadMainAssetAtPath(text2) as Material;
							if (val2 != null)
							{
								materials.Add(val2);
							}
							else
							{
								Debug.LogError((object)("Could not load " + text2));
							}
						}
					}
				}
				string[] directories = Directory.GetDirectories(rootPath);
				foreach (string rootPath2 in directories)
				{
					FindDynamicContentInProject(ref prefabs, ref materials, rootPath2);
				}
			}
		}

		private static bool FindPrefabReferencesOnPrefabs(ref List<GameObject> prefabs)
		{
			List<GameObject> list = new List<GameObject>();
			foreach (GameObject item in from p in prefabs
			where p != null
			select p)
			{
				VRC_Trigger[] components = item.GetComponents<VRC_Trigger>();
				foreach (VRC_Trigger item2 in components.Where((VRC_Trigger t) => t != null))
				{
					foreach (TriggerEvent item3 in item2.Triggers.Where((TriggerEvent te) => te != null))
					{
						foreach (VrcEvent item4 in item3.Events.Where((VrcEvent e) => e != null && (int)e.EventType == 13))
						{
							GameObject val = AssetDatabase.LoadAssetAtPath(item4.ParameterString, typeof(GameObject)) as GameObject;
							if (val != null && !prefabs.Contains(val))
							{
								list.Add(val);
							}
						}
					}
				}
			}
			if (list.Count > 0)
			{
				prefabs.AddRange(list);
				return true;
			}
			return false;
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
