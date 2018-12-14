using interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class PlaymakerDLLMaker
{
	public const string SOURCE_PATH = "Assets/WebPlayerTemplates/Playmaker_VRC";

	public static string SOURCE_FULL_PATH = Application.get_dataPath() + "/WebPlayerTemplates/Playmaker_VRC";

	public static string DLL_MAKER_SOURCE_FULL_PATH = Application.get_dataPath() + "/Scripts/DLLMaker";

	public static string DLL_MAKER_OUTPUT_FULL_PATH = Application.get_dataPath() + "/../Temp";

	public static string SDK_OUTPUT_FULL_PATH = Application.get_dataPath() + "/Plugins";

	public static void MakeAll(bool debug, bool isInternal)
	{
		try
		{
			MakePlaymakerDLL(debug, isInternal);
		}
		catch (Exception ex)
		{
			Debug.LogError((object)("Error - " + ex.Message + "\n" + ex.StackTrace));
		}
		AssetDatabase.Refresh();
	}

	public static void MakePlaymakerDLL(bool debug, bool isInternal)
	{
		DLLMaker dLLMaker = new DLLMaker();
		dLLMaker.debug = debug;
		dLLMaker.strongNameKeyFile = "PlaymakerVRC.snk";
		dLLMaker.sourcePaths = new List<string>();
		dLLMaker.sourcePaths.Add(SOURCE_FULL_PATH);
		dLLMaker.DllDependencies = new List<string>();
		dLLMaker.DllDependencies.Add(DLLMaker.UnityExtensionDLLDirectoryPath + "GUISystem" + Path.DirectorySeparatorChar + "UnityEngine.UI.dll");
		dLLMaker.DllDependencies.Add("Assets/VRCSDK/Dependencies/VRChat/VRCSDK2.dll");
		dLLMaker.DllDependencies.Add("Assets/Plugins/PlayMaker/PlayMaker.dll");
		dLLMaker.BuildTargetDir = SDK_OUTPUT_FULL_PATH + "/VRCSDK";
		dLLMaker.BuildTargetFile = "Playmaker_VRC.dll";
		dLLMaker.CreateDLL();
	}
}
