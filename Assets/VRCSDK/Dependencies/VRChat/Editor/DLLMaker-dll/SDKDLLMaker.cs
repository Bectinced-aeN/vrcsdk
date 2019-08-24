using interfaces;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SDKDLLMaker
{
	public const string FILE_CORE_EDITOR_DLL = "VRCCore-Editor.dll";

	public const string FILE_CORE_STANDALONE_DLL = "VRCCore-Standalone.dll";

	public const string FILE_CORE_STRIPPED_DLL = "VRCCore-Editor.dll";

	public const string FILE_DLL_MAKER_DLL = "DLLMaker.dll";

	public const string FILE_SDK_DLL = "VRCSDK2.dll";

	public const string FILE_SDKGUI_DLL = "VRCSDK2-GUI.dll";

	public const string SOURCE_PATH = "Assets/WebPlayerTemplates/VRC/Source";

	public static string CORE_SOURCE_FULL_PATH = Application.get_dataPath() + "/WebPlayerTemplates/VRC/Source";

	public static string DLL_MAKER_SOURCE_FULL_PATH = Application.get_dataPath() + "/Scripts/DLLMaker";

	public static string DLL_MAKER_OUTPUT_FULL_PATH = Application.get_dataPath() + "/../Temp";

	public static string SDK_OUTPUT_FULL_PATH = Application.get_dataPath() + "/VRCSDK/Dependencies/VRChat";

	public static string STRIPPED_OUTPUT_FULL_PATH = Application.get_dataPath() + "/Scripts/VRChatSDK-Internal";

	public static void MakeAll(bool debug, bool isInternal, bool noGraphics)
	{
		TryMakeCoreEditorDLL(debug, isInternal, noGraphics);
		TryMakeCoreStrippedDLL(debug, isInternal, noGraphics);
		TryMakeCoreStandaloneDLL(debug, isInternal, noGraphics);
		TryMakeSdkDll(debug, isInternal, noGraphics);
		TryMakeSdkGuiDll(debug, isInternal, noGraphics);
		TryMakeDllMakerDll(debug, isInternal, noGraphics);
		AssetDatabase.Refresh();
	}

	public static void TryMakeCoreEditorDLL(bool debug, bool isInternal, bool noGraphics)
	{
		try
		{
			CompilerResults dllCompileResult = MakeEditorCoreDLL(debug, isInternal);
			if (!HandleErrors("VRCCore-Editor.dll", dllCompileResult, noGraphics))
			{
			}
		}
		catch (Exception ex)
		{
			Debug.LogError((object)("Error - " + ex.Message + "\n" + ex.StackTrace));
			if (!noGraphics)
			{
				EditorUtility.DisplayDialog("DLL build fail", "Could not build VRCCore-Editor.dll. Unexpected error. Please see console errors for details.", "OK");
			}
			goto end_IL_0025;
			IL_0066:
			end_IL_0025:;
		}
	}

	public static void TryMakeCoreStrippedDLL(bool debug, bool isInternal, bool noGraphics)
	{
		try
		{
			CompilerResults dllCompileResult = MakeStrippedCoreDLL(debug, isInternal);
			if (!HandleErrors("VRCCore-Editor.dll-stripped", dllCompileResult, noGraphics))
			{
			}
		}
		catch (Exception ex)
		{
			Debug.LogError((object)("Error - " + ex.Message + "\n" + ex.StackTrace));
			if (!noGraphics)
			{
				EditorUtility.DisplayDialog("DLL build fail", "Could not build VRCCore-Editor.dll. Unexpected error. Please see console errors for details.", "OK");
			}
			goto end_IL_0025;
			IL_0066:
			end_IL_0025:;
		}
	}

	public static void TryMakeCoreStandaloneDLL(bool debug, bool isInternal, bool noGraphics)
	{
		try
		{
			CompilerResults compilerResults = MakeStandaloneCoreDLL(debug, isInternal);
			if (!HandleErrors("VRCCore-Standalone.dll", compilerResults, noGraphics))
			{
				Obfuscate(compilerResults.PathToAssembly, debug);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError((object)("Error - " + ex.Message + "\n" + ex.StackTrace));
			if (!noGraphics)
			{
				EditorUtility.DisplayDialog("DLL build fail", "Could not build VRCCore-Standalone.dll. Unexpected error. Please see console errors for details.", "OK");
			}
			goto end_IL_0031;
			IL_0072:
			end_IL_0031:;
		}
	}

	private static bool HandleErrors(string dllName, CompilerResults dllCompileResult, bool noGraphics)
	{
		if (dllCompileResult == null)
		{
			throw new ArgumentNullException("dllCompileResult");
		}
		if (!dllCompileResult.Errors.HasErrors)
		{
			return false;
		}
		if (noGraphics)
		{
			return true;
		}
		CompilerError compilerError = null;
		for (int i = 0; i < dllCompileResult.Errors.Count; i++)
		{
			CompilerError compilerError2 = dllCompileResult.Errors[i];
			if (!compilerError2.IsWarning)
			{
				compilerError = compilerError2;
				break;
			}
		}
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(compilerError.FileName);
		string text = (compilerError.ErrorText.Length > 200) ? compilerError.ErrorText.Substring(0, 200) : compilerError.ErrorText;
		string text2 = "Could not build " + dllName + ". Please see console errors for details.\n\nError at " + compilerError.FileName + ":" + compilerError.Line + "\n\n" + text;
		string[] array = AssetDatabase.FindAssets(fileNameWithoutExtension);
		if (array == null || array.Length == 0)
		{
			EditorUtility.DisplayDialog("DLL build fail", text2, "OK");
		}
		else if (EditorUtility.DisplayDialog("DLL build fail", text2, "Jump to First Error", "OK"))
		{
			string[] array2 = array;
			foreach (string text3 in array2)
			{
				string text4 = AssetDatabase.GUIDToAssetPath(text3);
				Object val = AssetDatabase.LoadAssetAtPath(text4, typeof(Object));
				if (val != null && val.get_name() == fileNameWithoutExtension)
				{
					AssetDatabase.OpenAsset(val, compilerError.Line);
				}
			}
		}
		return true;
	}

	private static void TryMakeDllMakerDll(bool debug, bool isInternal, bool noGraphics)
	{
		try
		{
			MakeDLLMakerDLL(debug, isInternal);
		}
		catch (Exception ex)
		{
			Debug.LogError((object)("Error - " + ex.Message + "\n" + ex.StackTrace));
			if (!noGraphics)
			{
				EditorUtility.DisplayDialog("DLL build fail", "Could not build DLLMaker.dll. Please see console errors for details.", "OK");
			}
			goto end_IL_000c;
			IL_004d:
			end_IL_000c:;
		}
	}

	private static void TryMakeSdkGuiDll(bool debug, bool isInternal, bool noGraphics)
	{
		try
		{
			CompilerResults compilerResults = MakeSDKGuiDLL(debug, isInternal);
			if (!HandleErrors("VRCSDK2-GUI.dll", compilerResults, noGraphics))
			{
				Obfuscate(compilerResults.PathToAssembly, debug);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError((object)("Error - " + ex.Message + "\n" + ex.StackTrace));
			if (!noGraphics)
			{
				EditorUtility.DisplayDialog("DLL build fail", "Could not build VRCSDK2-GUI.dll. Please see console errors for details.", "OK");
			}
			goto end_IL_0031;
			IL_0072:
			end_IL_0031:;
		}
	}

	private static void TryMakeSdkDll(bool debug, bool isInternal, bool noGraphics)
	{
		try
		{
			CompilerResults compilerResults = MakeSDKDLL(debug, isInternal);
			if (!HandleErrors("VRCSDK2.dll", compilerResults, noGraphics))
			{
				Obfuscate(compilerResults.PathToAssembly, debug);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError((object)("Error - " + ex.Message + "\n" + ex.StackTrace));
			if (!noGraphics)
			{
				EditorUtility.DisplayDialog("DLL build fail", "Could not build VRCSDK2.dll. Please see console errors for details.", "OK");
			}
			goto end_IL_0031;
			IL_0072:
			end_IL_0031:;
		}
	}

	public static CompilerResults MakeSDKGuiDLL(bool debug, bool isInternal)
	{
		DLLMaker dLLMaker = new DLLMaker();
		dLLMaker.debug = debug;
		dLLMaker.strongNameKeyFile = "VRCSDK2.snk";
		dLLMaker.sourcePaths = new List<string>();
		dLLMaker.sourcePaths.Add(CORE_SOURCE_FULL_PATH + "/VRCSDK-Gui/Scripts");
		dLLMaker.DllDependencies = new List<string>();
		dLLMaker.DllDependencies.Add(DLLMaker.UnityExtensionDLLDirectoryPath + "GUISystem" + Path.DirectorySeparatorChar + "UnityEngine.UI.dll");
		dLLMaker.DllDependencies.Add(DLLMaker.UnityDLLDirectoryPath + "UnityEditor.dll");
		dLLMaker.DllDependencies.Add(SDK_OUTPUT_FULL_PATH + "/VRCCore-Editor.dll");
		dLLMaker.DllDependencies.Add(SDK_OUTPUT_FULL_PATH + "/VRCSDK2.dll");
		if (isInternal)
		{
			dLLMaker.defines.Add("INTERNAL_SDK");
		}
		dLLMaker.BuildTargetDir = SDK_OUTPUT_FULL_PATH;
		dLLMaker.BuildTargetFile = "VRCSDK2-GUI.dll";
		return dLLMaker.CreateDLL();
	}

	public static CompilerResults MakeSDKDLL(bool debug, bool isInternal)
	{
		DLLMaker dLLMaker = new DLLMaker();
		dLLMaker.debug = debug;
		dLLMaker.strongNameKeyFile = "VRCSDK2.snk";
		dLLMaker.sourcePaths = new List<string>();
		dLLMaker.sourcePaths.Add(CORE_SOURCE_FULL_PATH + "/VRCSDK/Scripts");
		dLLMaker.DllDependencies = new List<string>();
		dLLMaker.DllDependencies.Add(DLLMaker.UnityExtensionDLLDirectoryPath + "GUISystem" + Path.DirectorySeparatorChar + "UnityEngine.UI.dll");
		if (isInternal)
		{
			dLLMaker.defines.Add("INTERNAL_SDK");
		}
		dLLMaker.BuildTargetDir = SDK_OUTPUT_FULL_PATH;
		dLLMaker.BuildTargetFile = "VRCSDK2.dll";
		return dLLMaker.CreateDLL();
	}

	private static void Obfuscate(string dllPath, bool debug)
	{
		if (string.IsNullOrEmpty(dllPath))
		{
			Debug.LogError((object)"SDKDLLMaker: Cannot obfuscate dll, given path is null");
		}
	}

	public static CompilerResults MakeEditorCoreDLL(bool debug, bool isInternal)
	{
		DLLMaker dLLMaker = new DLLMaker();
		dLLMaker.debug = debug;
		dLLMaker.strongNameKeyFile = "VRCSDK2.snk";
		dLLMaker.sourcePaths = new List<string>();
		dLLMaker.sourcePaths.Add(CORE_SOURCE_FULL_PATH + "/VRCCore/Scripts");
		dLLMaker.DllDependencies = new List<string>();
		dLLMaker.DllDependencies.Add(DLLMaker.UnityExtensionDLLDirectoryPath + "GUISystem" + Path.DirectorySeparatorChar + "UnityEngine.UI.dll");
		if (isInternal)
		{
			dLLMaker.defines.Add("INTERNAL_SDK");
		}
		dLLMaker.defines.Add("INCLUDE_PIPELINE_MANAGER");
		dLLMaker.IsEditor = true;
		dLLMaker.BuildTargetDir = SDK_OUTPUT_FULL_PATH;
		dLLMaker.BuildTargetFile = "VRCCore-Editor.dll";
		return dLLMaker.CreateDLL();
	}

	public static CompilerResults MakeStrippedCoreDLL(bool debug, bool isInternal)
	{
		DLLMaker dLLMaker = new DLLMaker();
		dLLMaker.debug = debug;
		dLLMaker.strongNameKeyFile = "VRCSDK2.snk";
		dLLMaker.sourcePaths = new List<string>();
		dLLMaker.sourceFilePaths = new List<string>();
		dLLMaker.sourceFilePaths.Add(CORE_SOURCE_FULL_PATH + "/VRCCore/Scripts/PipelineManager.cs");
		dLLMaker.DllDependencies = new List<string>();
		dLLMaker.DllDependencies.Add(DLLMaker.UnityExtensionDLLDirectoryPath + "GUISystem" + Path.DirectorySeparatorChar + "UnityEngine.UI.dll");
		dLLMaker.DllDependencies.Add(SDK_OUTPUT_FULL_PATH + "/VRCCore-Standalone.dll");
		if (isInternal)
		{
			dLLMaker.defines.Add("INTERNAL_SDK");
		}
		dLLMaker.defines.Add("INCLUDE_PIPELINE_MANAGER");
		dLLMaker.IsEditor = true;
		dLLMaker.BuildTargetDir = STRIPPED_OUTPUT_FULL_PATH;
		dLLMaker.BuildTargetFile = "VRCCore-Editor.dll";
		return dLLMaker.CreateDLL(explicitFiles: true);
	}

	public static CompilerResults MakeStandaloneCoreDLL(bool debug, bool isInternal)
	{
		DLLMaker dLLMaker = new DLLMaker();
		dLLMaker.debug = debug;
		dLLMaker.strongNameKeyFile = "VRCSDK2.snk";
		dLLMaker.sourcePaths = new List<string>();
		dLLMaker.sourcePaths.Add(CORE_SOURCE_FULL_PATH + "/VRCCore/Scripts");
		dLLMaker.DllDependencies = new List<string>();
		dLLMaker.DllDependencies.Add(DLLMaker.UnityExtensionDLLDirectoryPath + "GUISystem" + Path.DirectorySeparatorChar + "UnityEngine.UI.dll");
		if (isInternal)
		{
			dLLMaker.defines.Add("INTERNAL_SDK");
		}
		dLLMaker.IsEditor = false;
		dLLMaker.BuildTargetDir = SDK_OUTPUT_FULL_PATH;
		dLLMaker.BuildTargetFile = "VRCCore-Standalone.dll";
		return dLLMaker.CreateDLL();
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
		dLLMaker.DllDependencies = new List<string>();
		dLLMaker.DllDependencies.Add(SDK_OUTPUT_FULL_PATH + "/VRCCore-Editor.dll");
		dLLMaker.DllDependencies.Add(SDK_OUTPUT_FULL_PATH + "/VRCSDK2.dll");
		dLLMaker.IsEditor = true;
		dLLMaker.BuildTargetDir = DLL_MAKER_OUTPUT_FULL_PATH;
		dLLMaker.BuildTargetFile = "DLLMaker.dll";
		dLLMaker.CreateDLL();
	}
}
