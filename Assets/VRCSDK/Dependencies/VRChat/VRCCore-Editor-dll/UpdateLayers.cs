using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using VRC;

public class UpdateLayers
{
	private static VRCProjectSettings projectSettings;

	private static Dictionary<int, string> layersToSet;

	private static Assembly editorAsm;

	private static MethodInfo AddSortingLayer_Method;

	private static void SetupLayersToSet()
	{
		if (projectSettings == null)
		{
			projectSettings = Resources.Load<VRCProjectSettings>("VRCProjectSettings");
		}
		layersToSet = new Dictionary<int, string>();
		for (int i = 8; i < projectSettings.numLayers; i++)
		{
			layersToSet[i] = projectSettings.layers[i];
		}
	}

	public static void SetupEditorLayers()
	{
		if (layersToSet == null)
		{
			SetupLayersToSet();
		}
		SetLayers(layersToSet);
	}

	public static void SetLayers(Dictionary<int, string> layersToSet)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		SerializedObject val = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
		SerializedProperty val2 = val.FindProperty("layers");
		int[] array = new int[layersToSet.Count];
		layersToSet.Keys.CopyTo(array, 0);
		List<string> list = new List<string>();
		int[] array2 = array;
		foreach (int num in array2)
		{
			SerializedProperty arrayElementAtIndex = val2.GetArrayElementAtIndex(num);
			if (arrayElementAtIndex != null && !string.IsNullOrEmpty(arrayElementAtIndex.get_stringValue()) && !layersToSet.ContainsValue(arrayElementAtIndex.get_stringValue()))
			{
				list.Add(arrayElementAtIndex.get_stringValue());
			}
		}
		foreach (KeyValuePair<int, string> item in layersToSet)
		{
			SerializedProperty arrayElementAtIndex2 = val2.GetArrayElementAtIndex(item.Key);
			arrayElementAtIndex2.set_stringValue(item.Value);
		}
		val.ApplyModifiedProperties();
		CheckLayers(list.ToArray());
	}

	public static void CheckLayers(string[] layerNames)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		SerializedObject val = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
		SerializedProperty val2 = val.FindProperty("layers");
		foreach (string text in layerNames)
		{
			bool flag = false;
			for (int j = 0; j <= 31; j++)
			{
				SerializedProperty arrayElementAtIndex = val2.GetArrayElementAtIndex(j);
				if (arrayElementAtIndex != null && text.Equals(arrayElementAtIndex.get_stringValue()))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				SerializedProperty val3 = null;
				for (int k = 8; k <= 31; k++)
				{
					SerializedProperty arrayElementAtIndex2 = val2.GetArrayElementAtIndex(k);
					if (arrayElementAtIndex2 != null && string.IsNullOrEmpty(arrayElementAtIndex2.get_stringValue()))
					{
						val3 = arrayElementAtIndex2;
						break;
					}
				}
				if (val3 != null)
				{
					val3.set_stringValue(text);
				}
				else
				{
					Debug.LogError((object)("Could not find an open Layer Slot for: " + text));
				}
			}
		}
		val.ApplyModifiedProperties();
	}

	public static void CheckTags(string[] tagNames)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		SerializedObject val = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
		SerializedProperty val2 = val.FindProperty("tags");
		List<string> list = new List<string>();
		list.Add("Untagged");
		list.Add("Respawn");
		list.Add("Finish");
		list.Add("EditorOnly");
		list.Add("MainCamera");
		list.Add("Player");
		list.Add("GameController");
		List<string> list2 = list;
		foreach (string text in tagNames)
		{
			if (!list2.Contains(text))
			{
				bool flag = false;
				for (int j = 0; j < val2.get_arraySize(); j++)
				{
					SerializedProperty arrayElementAtIndex = val2.GetArrayElementAtIndex(j);
					if (arrayElementAtIndex.get_stringValue().Equals(text))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					val2.InsertArrayElementAtIndex(0);
					SerializedProperty arrayElementAtIndex2 = val2.GetArrayElementAtIndex(0);
					arrayElementAtIndex2.set_stringValue(text);
				}
			}
		}
		val.ApplyModifiedProperties();
	}

	public static void CheckSortLayers(string[] tagNames)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		SerializedObject val = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
		SerializedProperty val2 = val.FindProperty("m_SortingLayers");
		foreach (string text in tagNames)
		{
			bool flag = false;
			for (int j = 0; j < val2.get_arraySize(); j++)
			{
				SerializedProperty arrayElementAtIndex = val2.GetArrayElementAtIndex(j);
				SerializedProperty val3 = arrayElementAtIndex.FindPropertyRelative("name");
				if (val3.get_stringValue().Equals(text))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				val.ApplyModifiedProperties();
				AddSortingLayer();
				val.Update();
				int num = val2.get_arraySize() - 1;
				SerializedProperty arrayElementAtIndex2 = val2.GetArrayElementAtIndex(num);
				SerializedProperty val4 = arrayElementAtIndex2.FindPropertyRelative("name");
				val4.set_stringValue(text);
			}
		}
		val.ApplyModifiedProperties();
	}

	public static void AddSortingLayer()
	{
		if (AddSortingLayer_Method == null)
		{
			if (editorAsm == null)
			{
				editorAsm = Assembly.GetAssembly(typeof(Editor));
			}
			Type type = editorAsm.GetType("UnityEditorInternal.InternalEditorUtility");
			AddSortingLayer_Method = type.GetMethod("AddSortingLayer", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[0], null);
		}
		AddSortingLayer_Method.Invoke(null, null);
	}

	public static bool AreLayersSetup()
	{
		if (layersToSet == null)
		{
			SetupLayersToSet();
		}
		bool result = true;
		foreach (KeyValuePair<int, string> item in layersToSet)
		{
			if (LayerMask.LayerToName(item.Key) != item.Value)
			{
				return false;
			}
		}
		return result;
	}

	public static void SetupCollisionLayerMatrix()
	{
		if (projectSettings == null)
		{
			projectSettings = Resources.Load<VRCProjectSettings>("VRCProjectSettings");
		}
		bool[,] array = Tools.OneDArrayToTwoDArray(projectSettings.layerCollisionArr, 32, 32);
		int numLayers = projectSettings.numLayers;
		for (int i = 0; i < numLayers; i++)
		{
			for (int j = 0; j < numLayers; j++)
			{
				bool flag = !array[i, j];
				Physics.IgnoreLayerCollision(i, j, flag);
			}
		}
	}

	public static bool[,] GetLayerCollisionMatrix()
	{
		bool[,] array = new bool[32, 32];
		for (int i = 0; i < 32; i++)
		{
			for (int j = 0; j < 32; j++)
			{
				bool[,] array2 = array;
				int num = i;
				int num2 = j;
				bool num3 = !Physics.GetIgnoreLayerCollision(i, j);
				array2[num, num2] = num3;
			}
		}
		return array;
	}

	public static bool[] GetLayerCollisionArray()
	{
		bool[,] array = new bool[32, 32];
		for (int i = 0; i < 32; i++)
		{
			for (int j = 0; j < 32; j++)
			{
				bool[,] array2 = array;
				int num = i;
				int num2 = j;
				bool num3 = !Physics.GetIgnoreLayerCollision(i, j);
				array2[num, num2] = num3;
			}
		}
		return Tools.TwoDArrayToOneDArray(array);
	}

	public static bool IsCollisionLayerMatrixSetup()
	{
		if (projectSettings == null)
		{
			projectSettings = Resources.Load<VRCProjectSettings>("VRCProjectSettings");
		}
		bool[,] array = Tools.OneDArrayToTwoDArray(projectSettings.layerCollisionArr, 32, 32);
		int numLayers = projectSettings.numLayers;
		bool[,] array2 = Tools.OneDArrayToTwoDArray(GetLayerCollisionArray(), 32, 32);
		bool result = true;
		for (int i = 0; i < numLayers; i++)
		{
			for (int j = 0; j < numLayers; j++)
			{
				if (array[i, j] != array2[i, j])
				{
					result = false;
					break;
				}
			}
		}
		return result;
	}
}
