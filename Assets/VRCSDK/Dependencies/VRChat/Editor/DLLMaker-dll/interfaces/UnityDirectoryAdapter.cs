using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace interfaces
{
	public class UnityDirectoryAdapter
	{
		private readonly string baseProjectPath = Path.GetFullPath(Application.get_dataPath() + Path.DirectorySeparatorChar + "..");

		public string assetPathToFullPath(string assetBasedPathIn)
		{
			string[] array = assetBasedPathIn.Split(new string[2]
			{
				"/",
				"\\"
			}, StringSplitOptions.RemoveEmptyEntries);
			StringBuilder stringBuilder = new StringBuilder(baseProjectPath);
			stringBuilder.Append(Path.DirectorySeparatorChar);
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Append(array[i]);
				stringBuilder.Append(Path.DirectorySeparatorChar);
			}
			return stringBuilder.ToString();
		}

		public string fullPathToAssetsPath(string fullPathIn)
		{
			string text = fullPathIn.Replace("\\", "/");
			string projectRelativePath = FileUtil.GetProjectRelativePath(text);
			if (string.IsNullOrEmpty(projectRelativePath) || !text.Contains(baseProjectPath))
			{
				throw new DirectoryNotInProjectPathException("fullPathIn not found in asset path:" + fullPathIn);
			}
			return projectRelativePath;
		}

		public void createDirectory(string fullPath)
		{
			string fullPath2 = Path.GetFullPath(fullPath);
			if (!Directory.Exists(fullPath2))
			{
				Directory.CreateDirectory(fullPath2);
			}
		}

		public string cleanPath(string fullPath)
		{
			return Path.GetFullPath(fullPath);
		}

		public void deleteAllFilesInDirectory(string directory)
		{
			string[] files = Directory.GetFiles(directory);
			foreach (string path in files)
			{
				File.Delete(path);
			}
		}

		public void deleteFileIfItExists(string fullPath)
		{
			if (File.Exists(fullPath))
			{
				File.Delete(fullPath);
			}
		}

		public void deleteFileInDirectoryIfItExists(string directory, string fileNameToDelete)
		{
			string[] files = Directory.GetFiles(directory);
			foreach (string text in files)
			{
				if (text == fileNameToDelete)
				{
					File.Delete(text);
				}
			}
		}

		public List<string> readAllFiles(List<string> fileList)
		{
			List<string> list = new List<string>();
			foreach (string file in fileList)
			{
				list.Add(File.ReadAllText(file));
			}
			return list;
		}

		public List<string> getAllMatchingFilesRecursive(string directoryIn, List<string> blackList, string wildcardToLookFor)
		{
			List<string> list = new List<string>();
			string text = cleanPath(directoryIn);
			if (!Directory.Exists(text))
			{
				return list;
			}
			string text2 = text;
			if (text2.EndsWith(Path.DirectorySeparatorChar + string.Empty))
			{
				text2 = text2.Substring(0, text2.Length - 1);
			}
			string[] files = Directory.GetFiles(text2, wildcardToLookFor);
			foreach (string item in files)
			{
				list.Add(item);
			}
			string[] directories = Directory.GetDirectories(text);
			foreach (string text3 in directories)
			{
				if (!isBlackListed(text3, blackList))
				{
					list.AddRange(getAllMatchingFilesRecursive(text3, blackList, wildcardToLookFor));
				}
			}
			return list;
		}

		public bool IsFileLocked(FileInfo file)
		{
			FileStream fileStream = null;
			try
			{
				fileStream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
			}
			catch (IOException)
			{
				return true;
				IL_0019:;
			}
			finally
			{
				fileStream?.Close();
			}
			return false;
		}

		private bool isBlackListed(string fullPath, List<string> blackList)
		{
			string value = ".";
			string[] array = fullPath.Split(Path.DirectorySeparatorChar);
			if (array[array.Length - 1].StartsWith(value))
			{
				return true;
			}
			foreach (string black in blackList)
			{
				string text = black;
				if (black.EndsWith(string.Empty + Path.DirectorySeparatorChar))
				{
					text = text.Substring(0, text.Length - 1);
				}
				if (fullPath.Contains(text))
				{
					return true;
				}
			}
			return false;
		}
	}
}
