using interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SDKDLLMaker
{
	public const string SOURCE_PATH = "Assets/WebPlayerTemplates/VRC/Source";

	public static string SOURCE_FULL_PATH = Application.get_dataPath() + "/WebPlayerTemplates/VRC/Source";

	public static string DLL_MAKER_SOURCE_FULL_PATH = Application.get_dataPath() + "/Scripts/DLLMaker";

	public static string DLL_MAKER_OUTPUT_FULL_PATH = Application.get_dataPath() + "/../Temp";

	public static string SDK_OUTPUT_FULL_PATH = Application.get_dataPath() + "/VRCSDK/Dependencies/VRChat";

	public static void MakeAll(bool debug, bool isInternal)
	{
		try
		{
			MakeCoreDLLs(debug, isInternal);
		}
		catch (Exception ex)
		{
			Debug.LogError((object)("Error - " + ex.Message + "\n" + ex.StackTrace));
		}
		try
		{
			MakeSDKDLL(debug, isInternal);
		}
		catch (Exception ex2)
		{
			Debug.LogError((object)("Error - " + ex2.Message + "\n" + ex2.StackTrace));
		}
		try
		{
			MakeDLLMakerDLL(debug, isInternal);
		}
		catch (Exception ex3)
		{
			Debug.LogError((object)("Error - " + ex3.Message + "\n" + ex3.StackTrace));
		}
		AssetDatabase.Refresh();
	}

	public static void MakeSDKDLL(bool debug, bool isInternal)
	{
		DLLMaker dLLMaker = new DLLMaker();
		dLLMaker.debug = debug;
		dLLMaker.strongNameKeyFile = "VRCSDK2.snk";
		dLLMaker.sourcePaths = new List<string>();
		dLLMaker.sourcePaths.Add(SOURCE_FULL_PATH + "/VRCSDK/Scripts");
		dLLMaker.dllDependencies = new List<string>();
		dLLMaker.dllDependencies.Add(DLLMaker.unityExtensionDLLDirectoryPath + "GUISystem" + Path.DirectorySeparatorChar + "UnityEngine.UI.dll");
		if (isInternal)
		{
			dLLMaker.defines.Add("INTERNAL_SDK");
		}
		dLLMaker.buildTargetName = SDK_OUTPUT_FULL_PATH + "/VRCSDK2.dll";
		dLLMaker.createDLL();
	}

	public static void MakeCoreDLLs(bool debug, bool isInternal)
	{
		try
		{
			MakeEditorCoreDLL(debug, isInternal);
		}
		catch (Exception ex)
		{
			Debug.LogError((object)("Error - " + ex.Message + "\n" + ex.StackTrace));
		}
		try
		{
			MakeStandaloneCoreDLL(debug, isInternal);
		}
		catch (Exception ex2)
		{
			Debug.LogError((object)("Error - " + ex2.Message + "\n" + ex2.StackTrace));
		}
	}

	public static void MakeEditorCoreDLL(bool debug, bool isInternal)
	{
		DLLMaker dLLMaker = new DLLMaker();
		dLLMaker.debug = debug;
		dLLMaker.strongNameKeyFile = "VRCSDK2.snk";
		dLLMaker.sourcePaths = new List<string>();
		dLLMaker.sourcePaths.Add(SOURCE_FULL_PATH + "/VRCCore/Scripts");
		dLLMaker.dllDependencies = new List<string>();
		dLLMaker.dllDependencies.Add(DLLMaker.unityExtensionDLLDirectoryPath + "GUISystem" + Path.DirectorySeparatorChar + "UnityEngine.UI.dll");
		if (isInternal)
		{
			dLLMaker.defines.Add("INTERNAL_SDK");
		}
		dLLMaker.isEditor = true;
		dLLMaker.buildTargetName = SDK_OUTPUT_FULL_PATH + "/VRCCore-Editor.dll";
		dLLMaker.createDLL();
	}

	public static void MakeStandaloneCoreDLL(bool debug, bool isInternal)
	{
		DLLMaker dLLMaker = new DLLMaker();
		dLLMaker.debug = debug;
		dLLMaker.strongNameKeyFile = "VRCSDK2.snk";
		dLLMaker.sourcePaths = new List<string>();
		dLLMaker.sourcePaths.Add(SOURCE_FULL_PATH + "/VRCCore/Scripts");
		dLLMaker.dllDependencies = new List<string>();
		dLLMaker.dllDependencies.Add(DLLMaker.unityExtensionDLLDirectoryPath + "GUISystem" + Path.DirectorySeparatorChar + "UnityEngine.UI.dll");
		if (isInternal)
		{
			dLLMaker.defines.Add("INTERNAL_SDK");
		}
		dLLMaker.isEditor = false;
		dLLMaker.buildTargetName = SDK_OUTPUT_FULL_PATH + "/VRCCore-Standalone.dll";
		dLLMaker.createDLL();
	}

	public static void MakeDLLMakerDLL(bool debug, bool isInternal)
	{
		DLLMaker dLLMaker = new DLLMaker();
		dLLMaker.debug = debug;
		dLLMaker.strongNameKeyFile = "VRCSDK2.snk";
		dLLMaker.sourcePaths = new List<string>();
		dLLMaker.sourcePaths.Add(DLL_MAKER_SOURCE_FULL_PATH);
		dLLMaker.defines.Add("UNITY_EDITOR_WIN");
		if (isInternal)
		{
			dLLMaker.defines.Add("INTERNAL_SDK");
		}
		dLLMaker.dllDependencies = new List<string>();
		dLLMaker.dllDependencies.Add(SDK_OUTPUT_FULL_PATH + "/VRCCore-Editor.dll");
		dLLMaker.dllDependencies.Add(SDK_OUTPUT_FULL_PATH + "/VRCSDK2.dll");
		dLLMaker.isEditor = true;
		dLLMaker.buildTargetName = DLL_MAKER_OUTPUT_FULL_PATH + "/DLLMaker.dll";
		dLLMaker.createDLL();
	}
}
