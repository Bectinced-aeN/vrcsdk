using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace interfaces
{
	public class DLLMaker
	{
		public class NoFilesInDLLException : Exception
		{
		}

		private static readonly string unityDLLDirectoryPath = Path.Combine(EditorApplication.get_applicationContentsPath(), "Managed" + Path.DirectorySeparatorChar);

		public static readonly string unityExtensionDLLDirectoryPath = Path.Combine(EditorApplication.get_applicationContentsPath(), "UnityExtensions" + Path.DirectorySeparatorChar + "Unity" + Path.DirectorySeparatorChar);

		private static readonly string unityDLLPath = unityDLLDirectoryPath + "UnityEngine.dll";

		private static readonly string unityEditorDLLPath = unityDLLDirectoryPath + "UnityEditor.dll";

		public bool optimize = true;

		public bool debug;

		public List<string> blackList;

		public string dllFolder = "Dlls";

		public string strongNameKeyFile;

		public List<string> sourcePaths;

		public List<string> defines = new List<string>
		{
			"UNITY_5_0",
			"UNITY_5",
			"UNITY_5_3"
		};

		private readonly UnityDirectoryAdapter directory = new UnityDirectoryAdapter();

		public string dllRootOutputFolder => Path.GetFullPath(Application.get_dataPath() + Path.DirectorySeparatorChar + "..") + Path.DirectorySeparatorChar + dllFolder + Path.DirectorySeparatorChar;

		public string buildTargetName
		{
			get;
			set;
		}

		public List<string> dllDependencies
		{
			get;
			set;
		}

		public bool isEditor
		{
			get;
			set;
		}

		public void cleanAllDLLs()
		{
			directory.deleteAllFilesInDirectory(dllFolder);
		}

		public void cleanAllDLLMatchingName(string name)
		{
			string fullPath = finalDLLOutputRootPath();
			directory.createDirectory(fullPath);
			directory.deleteFileInDirectoryIfItExists(fullPath, name);
		}

		public string finalDLLOutputRootPath()
		{
			return Path.GetDirectoryName(finalDllOutputPath());
		}

		public string finalDllOutputPath()
		{
			return buildTargetName;
		}

		private void AddDllDependenciesToReferencedAssemblies(CompilerParameters compileParams)
		{
			if (dllDependencies != null)
			{
				foreach (string dllDependency in dllDependencies)
				{
					if (!string.IsNullOrEmpty(dllDependency))
					{
						string text = dllDependency;
						if (!File.Exists(text))
						{
							Debug.LogError((object)("Does not think that dll with path:" + text + " exists"));
						}
						compileParams.ReferencedAssemblies.Add(text);
					}
				}
			}
		}

		private CompilerParameters getStandardCompilerParameters()
		{
			CompilerParameters compilerParameters = new CompilerParameters();
			compilerParameters.OutputAssembly = finalDllOutputPath();
			compilerParameters.CompilerOptions = string.Empty;
			if (optimize)
			{
				compilerParameters.CompilerOptions += " /optimize";
			}
			if (debug)
			{
				compilerParameters.GenerateInMemory = false;
				compilerParameters.IncludeDebugInformation = true;
				compilerParameters.TempFiles.KeepFiles = true;
				compilerParameters.TempFiles = new TempFileCollection(Environment.GetEnvironmentVariable("TEMP"), keepFiles: true);
				compilerParameters.CompilerOptions += " /debug";
			}
			if (!File.Exists(unityDLLPath) || !File.Exists(unityEditorDLLPath))
			{
				throw new Exception("quilt: Error unity path dll path does not exist");
			}
			compilerParameters.ReferencedAssemblies.Add(unityDLLPath);
			defines.RemoveAll((string matchToRemove) => string.IsNullOrEmpty(matchToRemove));
			if (isEditor)
			{
				defines.Add("UNITY_EDITOR");
				compilerParameters.ReferencedAssemblies.Add(unityEditorDLLPath);
			}
			if (defines.Count > 1)
			{
				CompilerParameters compilerParameters2 = compilerParameters;
				compilerParameters2.CompilerOptions = compilerParameters2.CompilerOptions + " /define:" + string.Join(";", defines.ToArray());
			}
			if (strongNameKeyFile != null)
			{
				CompilerParameters compilerParameters3 = compilerParameters;
				compilerParameters3.CompilerOptions = compilerParameters3.CompilerOptions + " /keyfile:" + strongNameKeyFile;
			}
			return compilerParameters;
		}

		public string createDLL()
		{
			ShowErrorIfCannotOverwriteDLL(finalDllOutputPath());
			if (blackList == null)
			{
				blackList = new List<string>();
			}
			List<string> list = new List<string>();
			foreach (string sourcePath in sourcePaths)
			{
				list.AddRange(getAllSourceFromCSFilesInPathRecursive(sourcePath));
				List<string> allDllsInPathRecursive = getAllDllsInPathRecursive(sourcePath);
				if (dllDependencies == null)
				{
					dllDependencies = new List<string>();
				}
				foreach (string item2 in allDllsInPathRecursive)
				{
					if (!dllDependencies.Contains(item2))
					{
						dllDependencies.Add(item2);
					}
				}
			}
			if (list.Count == 0)
			{
				throw new NoFilesInDLLException();
			}
			HashSet<string> hashSet = new HashSet<string>();
			foreach (string item3 in list)
			{
				string item = Path.GetDirectoryName(item3) + "/*.cs";
				if (!hashSet.Contains(item))
				{
					hashSet.Add(item);
				}
			}
			CompilerParameters standardCompilerParameters = getStandardCompilerParameters();
			AddDllDependenciesToReferencedAssemblies(standardCompilerParameters);
			string directoryName = Path.GetDirectoryName(finalDllOutputPath());
			if (!Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			List<string> list2 = new List<string>();
			list2.Add(".dll.mdb.meta");
			list2.Add(".dll.mdb");
			list2.Add(".sln");
			list2.Add(".sln.meta");
			list2.Add(".userprefs");
			list2.Add(".userprefs.meta");
			List<string> list3 = list2;
			string str = directoryName + "/" + Path.GetFileNameWithoutExtension(finalDllOutputPath());
			foreach (string item4 in list3)
			{
				if (File.Exists(str + item4))
				{
					File.Delete(str + item4);
				}
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("CompilerVersion", "v3.5");
			CSharpCodeProvider cSharpCodeProvider = new CSharpCodeProvider(dictionary);
			CompilerResults compilerResults = cSharpCodeProvider.CompileAssemblyFromFile(standardCompilerParameters, hashSet.ToArray());
			if (compilerResults.Errors.Count > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				StringBuilder stringBuilder2 = new StringBuilder();
				foreach (CompilerError error in compilerResults.Errors)
				{
					if (!error.IsWarning)
					{
						stringBuilder.Append(error);
						Debug.LogError((object)error.ToString());
					}
					else
					{
						stringBuilder2.Append(error);
						Debug.LogWarning((object)("WARNING:" + error));
					}
				}
				if (stringBuilder.Length > 0)
				{
					throw new Exception(stringBuilder.ToString());
				}
			}
			return finalDllOutputPath();
		}

		public List<string> getAllSourceFromCSFilesInPathRecursive(string pathIn)
		{
			if (blackList == null)
			{
				blackList = new List<string>();
			}
			string directoryIn = directory.cleanPath(pathIn);
			return directory.getAllMatchingFilesRecursive(directoryIn, blackList, "*.cs");
		}

		public List<string> getAllDllsInPathRecursive(string pathIn)
		{
			string directoryIn = directory.cleanPath(pathIn);
			return directory.getAllMatchingFilesRecursive(directoryIn, blackList, "*.dll");
		}

		public static void ShowErrorIfCannotOverwriteDLL(string dllPath)
		{
			bool flag = false;
			UnityDirectoryAdapter unityDirectoryAdapter = new UnityDirectoryAdapter();
			if (!Directory.Exists(dllPath) || Directory.GetFiles(dllPath).Length <= 0)
			{
				while (File.Exists(dllPath) && unityDirectoryAdapter.IsFileLocked(new FileInfo(dllPath)) && !flag)
				{
					if (!EditorUtility.DisplayDialog("Cannot Create DLL", "Old DLL is in use and cannot be overwritten. Please close MonoDevelop and press continue to continue DLL creation. Or press abort to cancel DLL Creation. ", "Continue", "Abort"))
					{
						flag = true;
					}
				}
			}
			else
			{
				string[] files = Directory.GetFiles(dllPath);
				foreach (string text in files)
				{
					while (unityDirectoryAdapter.IsFileLocked(new FileInfo(text)) && !flag)
					{
						Debug.Log((object)("File still locked! - " + text));
						EditorUtility.DisplayDialog("test", "hi", "ok");
						if (!EditorUtility.DisplayDialog("Cannot Create DLL", "Old DLL is in use and cannot be overwritten. Please close MonoDevelop and press continue to continue DLL creation. Or press abort to cancel DLL Creation. ", "Continue", "Abort"))
						{
							flag = true;
						}
					}
				}
			}
		}
	}
}
