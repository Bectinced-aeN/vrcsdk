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
		private const string DllFolder = "Dlls";

		public static readonly string UnityExtensionDLLDirectoryPath = Path.Combine(EditorApplication.get_applicationContentsPath(), "UnityExtensions/Unity/");

		public static readonly string UnityDLLDirectoryPath = Path.Combine(EditorApplication.get_applicationContentsPath(), "Managed/");

		private static readonly string TempBuildDir = Application.get_dataPath() + "/../Temp/DllMaker";

		private static readonly string UnityDLLPath = UnityDLLDirectoryPath + "UnityEngine.dll";

		private static readonly string UnityEditorDLLPath = UnityDLLDirectoryPath + "UnityEditor.dll";

		public bool optimize = true;

		public bool debug;

		public List<string> blackList;

		public string strongNameKeyFile;

		public List<string> sourcePaths;

		public List<string> sourceFilePaths;

		public List<string> defines = new List<string>
		{
			"UNITY_4_6"
		};

		private readonly UnityDirectoryAdapter directory = new UnityDirectoryAdapter();

		public string DllRootOutputFolder => Path.GetFullPath(Application.get_dataPath() + "/../Dlls/");

		public string BuildTargetDir
		{
			get;
			set;
		}

		public string BuildTargetFile
		{
			get;
			set;
		}

		public List<string> DllDependencies
		{
			get;
			set;
		}

		public bool IsEditor
		{
			get;
			set;
		}

		public string FinalDllOutputPath => BuildTargetDir + "/" + BuildTargetFile;

		public string TempDllOutputPath => TempBuildDir + "/" + BuildTargetFile;

		public void CleanAllDLLs()
		{
			directory.deleteAllFilesInDirectory("Dlls");
		}

		public void CleanAllDLLMatchingName(string name)
		{
			string buildTargetDir = BuildTargetDir;
			directory.createDirectory(buildTargetDir);
			directory.deleteFileInDirectoryIfItExists(buildTargetDir, name);
		}

		private void AddDllDependenciesToReferencedAssemblies(CompilerParameters compileParams)
		{
			if (DllDependencies != null)
			{
				foreach (string dllDependency in DllDependencies)
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

		private CompilerParameters GetStandardCompilerParameters(string outputPath)
		{
			CompilerParameters compilerParameters = new CompilerParameters();
			compilerParameters.OutputAssembly = outputPath;
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
			if (!File.Exists(UnityDLLPath) || !File.Exists(UnityEditorDLLPath))
			{
				throw new Exception("quilt: Error unity path dll path does not exist");
			}
			compilerParameters.ReferencedAssemblies.Add(UnityDLLPath);
			defines.RemoveAll((string matchToRemove) => string.IsNullOrEmpty(matchToRemove));
			if (IsEditor)
			{
				defines.Add("UNITY_EDITOR");
				compilerParameters.ReferencedAssemblies.Add(UnityEditorDLLPath);
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

		public CompilerResults CreateDLL(bool explicitFiles = false)
		{
			if (TestIfCannotOverwriteDll(FinalDllOutputPath))
			{
				Debug.LogError((object)("Cannot build DLL, existing DLL is not overwritable, locked. DLL: " + FinalDllOutputPath));
				return null;
			}
			HashSet<string> allSourceDirectories = GetAllSourceDirectories();
			if (allSourceDirectories.Count == 0 && (sourceFilePaths == null || sourceFilePaths.Count == 0))
			{
				Debug.LogError((object)("Cannot build DLL, no source directories to compile for DLL:" + FinalDllOutputPath));
				return null;
			}
			if (File.Exists(TempDllOutputPath))
			{
				FileUtil.DeleteFileOrDirectory(TempDllOutputPath);
			}
			if (!Directory.Exists(TempBuildDir))
			{
				Directory.CreateDirectory(TempBuildDir);
			}
			DeleteSuperflousExtensions(FinalDllOutputPath);
			CompilerParameters standardCompilerParameters = GetStandardCompilerParameters(TempDllOutputPath);
			AddDllDependenciesToReferencedAssemblies(standardCompilerParameters);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("CompilerVersion", "v3.5");
			CSharpCodeProvider cSharpCodeProvider = new CSharpCodeProvider(dictionary);
			CompilerResults compilerResults = (!explicitFiles) ? cSharpCodeProvider.CompileAssemblyFromFile(standardCompilerParameters, allSourceDirectories.ToArray()) : cSharpCodeProvider.CompileAssemblyFromFile(standardCompilerParameters, sourceFilePaths.ToArray());
			LogCompileResults(compilerResults);
			if (!compilerResults.Errors.HasErrors)
			{
				string directoryName = Path.GetDirectoryName(FinalDllOutputPath);
				if (!Directory.Exists(directoryName))
				{
					Directory.CreateDirectory(directoryName);
				}
				File.Copy(compilerResults.PathToAssembly, FinalDllOutputPath, overwrite: true);
			}
			return compilerResults;
		}

		private HashSet<string> GetAllSourceDirectories()
		{
			List<string> allSourceFiles = GetAllSourceFiles();
			HashSet<string> hashSet = new HashSet<string>();
			foreach (string item2 in allSourceFiles)
			{
				string item = Path.GetDirectoryName(item2) + "/*.cs";
				if (!hashSet.Contains(item))
				{
					hashSet.Add(item);
				}
			}
			return hashSet;
		}

		private void DeleteSuperflousExtensions(string dllFullPath)
		{
			List<string> list = new List<string>();
			list.Add(".dll.mdb.meta");
			list.Add(".dll.mdb");
			list.Add(".sln");
			list.Add(".sln.meta");
			list.Add(".userprefs");
			list.Add(".userprefs.meta");
			List<string> list2 = list;
			string str = Path.GetDirectoryName(dllFullPath) + "/" + Path.GetFileNameWithoutExtension(dllFullPath);
			foreach (string item in list2)
			{
				if (File.Exists(str + item))
				{
					File.Delete(str + item);
				}
			}
		}

		private List<string> GetAllSourceFiles()
		{
			List<string> list = new List<string>();
			foreach (string sourcePath in sourcePaths)
			{
				list.AddRange(GetAllSourceFromCSFilesInPathRecursive(sourcePath));
				List<string> allDllsInPathRecursive = GetAllDllsInPathRecursive(sourcePath);
				if (DllDependencies == null)
				{
					DllDependencies = new List<string>();
				}
				foreach (string item in allDllsInPathRecursive)
				{
					if (!DllDependencies.Contains(item))
					{
						DllDependencies.Add(item);
					}
				}
			}
			return list;
		}

		private static void LogCompileResults(CompilerResults compilerResults)
		{
			if (!compilerResults.Errors.HasErrors)
			{
				Debug.Log((object)("<color=green>Compiled: " + compilerResults.PathToAssembly + "</color>"));
			}
			if (compilerResults.Errors.Count != 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				StringBuilder stringBuilder2 = new StringBuilder();
				foreach (CompilerError error in compilerResults.Errors)
				{
					if (!error.IsWarning && !string.IsNullOrEmpty(error.ErrorNumber))
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
			}
		}

		public List<string> GetAllSourceFromCSFilesInPathRecursive(string pathIn)
		{
			if (blackList == null)
			{
				blackList = new List<string>();
			}
			string directoryIn = directory.cleanPath(pathIn);
			return directory.getAllMatchingFilesRecursive(directoryIn, blackList, "*.cs");
		}

		public List<string> GetAllDllsInPathRecursive(string pathIn)
		{
			string directoryIn = directory.cleanPath(pathIn);
			return directory.getAllMatchingFilesRecursive(directoryIn, blackList, "*.dll");
		}

		public static bool TestIfCannotOverwriteDll(string dllPath)
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
			return flag;
		}
	}
}
