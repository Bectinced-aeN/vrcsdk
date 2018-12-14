using Microsoft.Win32;
using System;
using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace VRC.Core
{
	public class SDKClientUtilities
	{
		public static bool IsInternalSDK()
		{
			return false;
		}

		public static string LoadRegistryVRCInstallPath()
		{
			RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\VRChat", writable: false);
			string text = string.Empty;
			if (registryKey != null)
			{
				text = (string)registryKey.GetValue(null, string.Empty);
				if (text != null)
				{
					text += "\\VRChat.exe";
				}
			}
			EditorPrefs.SetString("VRC_installedClientPath", text);
			return text;
		}

		public static string GetSavedVRCInstallPath()
		{
			string result = string.Empty;
			if (EditorPrefs.HasKey("VRC_installedClientPath"))
			{
				result = EditorPrefs.GetString("VRC_installedClientPath");
			}
			return result;
		}

		public static void SetVRCInstallPath(string clientInstallPath)
		{
			if (EditorPrefs.HasKey("VRC_installedClientPath") && clientInstallPath != EditorPrefs.GetString("VRC_installedClientPath"))
			{
				EditorPrefs.SetString("VRC_installedClientPath", clientInstallPath);
			}
		}

		public static bool IsClientNewerThanSDK()
		{
			bool result = false;
			string testClientVersionDate = GetTestClientVersionDate();
			string sDKVersionDate = GetSDKVersionDate();
			if (!string.IsNullOrEmpty(testClientVersionDate) && !string.IsNullOrEmpty(sDKVersionDate))
			{
				DateTime t = DateTime.ParseExact(testClientVersionDate, "yyyy.MM.dd.HH.mm", CultureInfo.InvariantCulture, DateTimeStyles.None);
				DateTime t2 = DateTime.ParseExact(sDKVersionDate, "yyyy.MM.dd.HH.mm", CultureInfo.InvariantCulture, DateTimeStyles.None);
				result = (t > t2);
			}
			return result;
		}

		public static string GetTestClientVersionDate()
		{
			string result = string.Empty;
			string savedVRCInstallPath = GetSavedVRCInstallPath();
			if (!string.IsNullOrEmpty(savedVRCInstallPath))
			{
				string directoryName = Path.GetDirectoryName(savedVRCInstallPath);
				string path = directoryName + "/VRChat_data/version.txt";
				if (File.Exists(path))
				{
					string[] array = File.ReadAllLines(path);
					if (array.Length > 1)
					{
						result = array[1];
					}
				}
			}
			return result;
		}

		public static string GetSDKVersionDate()
		{
			string result = string.Empty;
			string path = Application.get_dataPath() + "/VRCSDK/version.txt";
			if (File.Exists(path))
			{
				string[] array = File.ReadAllLines(path);
				if (array.Length > 0)
				{
					result = array[0];
				}
			}
			return result;
		}
	}
}
