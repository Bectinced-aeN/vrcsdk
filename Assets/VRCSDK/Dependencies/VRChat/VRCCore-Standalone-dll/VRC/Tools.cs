using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRC.Core;
using VRC.Core.BestHTTP;
using VRC.Core.BestHTTP.Caching;
using VRC.Core.BestHTTP.Cookies;
using VRC.Core.BestHTTP.JSON;

namespace VRC
{
	public static class Tools
	{
		private static bool? _isClient = null;

		private static string _clientVersion = null;

		private static UnityVersion _unityVersion = new UnityVersion(0, 0, 0, 0);

		private static string _platform = null;

		public static bool isClient
		{
			get
			{
				bool? isClient = _isClient;
				if (!isClient.HasValue)
				{
					_isClient = HasType("VRCApplication");
				}
				bool? isClient2 = _isClient;
				return isClient2.Value;
			}
		}

		public static string ClientVersion
		{
			get
			{
				if (_clientVersion == null)
				{
					_clientVersion = Application.get_version();
				}
				return _clientVersion;
			}
		}

		public static UnityVersion UnityVersion
		{
			get
			{
				if (_unityVersion.major == 0)
				{
					_unityVersion = UnityVersion.Parse(Application.get_unityVersion());
				}
				return _unityVersion;
			}
		}

		public static string Platform
		{
			get
			{
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Invalid comparison between Unknown and I4
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Invalid comparison between Unknown and I4
				//IL_002d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Invalid comparison between Unknown and I4
				if (_platform == null)
				{
					RuntimePlatform platform = Application.get_platform();
					if ((int)platform == 2 || (int)platform == 7)
					{
						_platform = "standalonewindows";
					}
					else if ((int)platform == 11)
					{
						_platform = "android";
					}
					else
					{
						_platform = "unknownplatform";
					}
				}
				return _platform;
			}
		}

		private static bool HasType(string typeStr)
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly assembly in assemblies)
			{
				Type[] types = assembly.GetTypes();
				foreach (Type type in types)
				{
					if (type.Name == typeStr)
					{
						return true;
					}
				}
			}
			return false;
		}

		public static void SetLayerRecursively(GameObject obj, int newLayer, int except = -1)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Expected O, but got Unknown
			if (obj.get_layer() != except)
			{
				obj.set_layer(newLayer);
			}
			foreach (Transform item in obj.get_transform())
			{
				Transform val = item;
				SetLayerRecursively(val.get_gameObject(), newLayer, except);
			}
		}

		public static object GetOrDefaultFromDictionary(Dictionary<string, object> dict, string key, object defaultValue)
		{
			if (!dict.TryGetValue(key, out object value))
			{
				return defaultValue;
			}
			return value;
		}

		public static string ArrToString(object[] arr, string separator = ", ")
		{
			string text = string.Empty;
			foreach (object obj in arr)
			{
				string text2 = obj.ToString();
				text = ((!string.IsNullOrEmpty(text)) ? (text + separator + text2) : text2);
			}
			return text;
		}

		public static string ListToString(IList list, string separator = ", ")
		{
			string text = string.Empty;
			if (list != null)
			{
				foreach (object item in list)
				{
					string text2 = item.ToString();
					text = ((!string.IsNullOrEmpty(text)) ? (text + separator + text2) : text2);
				}
				return text;
			}
			return text;
		}

		public static bool ScrambledEquals<T>(IEnumerable<T> list1, IEnumerable<T> list2)
		{
			Dictionary<T, int> dictionary = new Dictionary<T, int>();
			foreach (T item in list1)
			{
				if (dictionary.ContainsKey(item))
				{
					Dictionary<T, int> dictionary2;
					Dictionary<T, int> dictionary3 = dictionary2 = dictionary;
					T key;
					T key2 = key = item;
					int num = dictionary2[key];
					dictionary3[key2] = num + 1;
				}
				else
				{
					dictionary.Add(item, 1);
				}
			}
			foreach (T item2 in list2)
			{
				if (!dictionary.ContainsKey(item2))
				{
					return false;
				}
				Dictionary<T, int> dictionary4;
				Dictionary<T, int> dictionary5 = dictionary4 = dictionary;
				T key;
				T key3 = key = item2;
				int num = dictionary4[key];
				dictionary5[key3] = num - 1;
			}
			int num2 = 0;
			foreach (KeyValuePair<T, int> item3 in dictionary)
			{
				num2 += item3.Value;
			}
			return num2 == 0;
		}

		public static List<string> ObjListToStringList(List<object> objList)
		{
			List<string> list = new List<string>();
			foreach (object obj in objList)
			{
				if (obj != null)
				{
					list.Add(obj.ToString());
				}
			}
			return list;
		}

		public static Dictionary<string, string> ObjDictToStringDict(Dictionary<string, object> dict)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (KeyValuePair<string, object> item in dict)
			{
				dictionary[item.Key] = item.Value.ToString();
			}
			return dictionary;
		}

		public static string GetGameObjectPath(GameObject obj)
		{
			string text = "/" + obj.get_name();
			while (obj.get_transform().get_parent() != null)
			{
				obj = obj.get_transform().get_parent().get_gameObject();
				text = "/" + obj.get_name() + text;
			}
			return text;
		}

		public static int CombineHashCodes(int a, int b)
		{
			int num = 17;
			num = num * 31 + a.GetHashCode();
			return num * 31 + b.GetHashCode();
		}

		public static string GetRandomDigits(int digits)
		{
			string text = string.Empty;
			for (int i = 0; i < digits; i++)
			{
				text += Random.Range(0, 9).ToString();
			}
			return text;
		}

		public static bool IsValidURL(string url)
		{
			bool result = true;
			try
			{
				HTTPRequest hTTPRequest = new HTTPRequest(new Uri(url), null);
				return result;
			}
			catch
			{
				return false;
			}
		}

		public static void ClearUserData()
		{
			APIUser.Logout();
			Caching.ClearCache();
			PlayerPrefs.DeleteAll();
			HTTPCacheService.BeginClear();
			string path = Application.get_persistentDataPath() + "/ab";
			if (Directory.Exists(path))
			{
				Directory.Delete(path, recursive: true);
			}
		}

		public static void ClearCookies()
		{
			CookieJar.Clear();
		}

		public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
		{
			return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTimeStamp).ToUniversalTime();
		}

		public static T[] FindSceneObjectsOfTypeAll<T>() where T : Component
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Invalid comparison between Unknown and I4
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Invalid comparison between Unknown and I4
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			List<T> list = new List<T>();
			T[] array = Resources.FindObjectsOfTypeAll<T>();
			T[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				T item = array2[i];
				if ((int)item.get_gameObject().get_hideFlags() != 8 && (int)item.get_gameObject().get_hideFlags() != 61)
				{
					Scene scene = item.get_gameObject().get_scene();
					if (scene.get_name() != null)
					{
						list.Add(item);
					}
				}
			}
			return list.ToArray();
		}

		public static T[] TwoDArrayToOneDArray<T>(T[,] twoDArr)
		{
			int length = twoDArr.GetLength(0);
			int length2 = twoDArr.GetLength(1);
			T[] array = new T[length * length2];
			for (int i = 0; i < length; i++)
			{
				for (int j = 0; j < length2; j++)
				{
					array[length * i + j] = twoDArr[i, j];
				}
			}
			return array;
		}

		public static T[,] OneDArrayToTwoDArray<T>(T[] oneDArr, int width, int height)
		{
			T[,] array = new T[width, height];
			for (int i = 0; i < oneDArr.Length; i++)
			{
				int num = i / width;
				int num2 = i % height;
				T[,] array2 = array;
				int num3 = num;
				int num4 = num2;
				T val = oneDArr[i];
				array2[num3, num4] = val;
			}
			return array;
		}

		public static int GetNumReservedLayers()
		{
			return 22;
		}

		public static string[] GetReservedLayers()
		{
			int numReservedLayers = GetNumReservedLayers();
			string[] array = new string[numReservedLayers];
			for (int i = 0; i < numReservedLayers; i++)
			{
				array[i] = LayerMask.LayerToName(i);
			}
			return array;
		}

		public static void ClearExpiredBestHTTPCache()
		{
			TimeSpan deleteOlder = TimeSpan.FromDays(14.0);
			ulong maxCacheSize = 5242880uL;
			HTTPCacheService.BeginMaintainence(new HTTPCacheMaintananceParams(deleteOlder, maxCacheSize));
			Debug.LogError((object)"Cleaning Cache");
		}

		public static string GetTempFolderPath(string subFolderName = "")
		{
			return Path.Combine(Application.get_temporaryCachePath(), subFolderName);
		}

		public static string GetTempFileName(string extension, out string errorStr, string subFolderName = "", bool createFolder = true)
		{
			try
			{
				string tempFolderPath = GetTempFolderPath(subFolderName);
				string empty = string.Empty;
				int num = 0;
				bool flag = false;
				do
				{
					empty = Path.Combine(tempFolderPath, Path.GetRandomFileName());
					if (extension != null)
					{
						empty = Path.ChangeExtension(empty, extension);
					}
					flag = File.Exists(empty);
				}
				while (flag && num++ < 10);
				if (flag)
				{
					errorStr = "Couldn't generate unique filename!";
					return string.Empty;
				}
				if (createFolder && !string.IsNullOrEmpty(subFolderName))
				{
					string directoryName = Path.GetDirectoryName(empty);
					if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
					{
						Directory.CreateDirectory(directoryName);
					}
				}
				errorStr = string.Empty;
				return empty;
				IL_00a5:;
			}
			catch (Exception ex)
			{
				errorStr = ex.Message;
			}
			return string.Empty;
		}

		public static bool FileCanRead(string filename)
		{
			string whyNot;
			return FileCanRead(filename, out whyNot);
		}

		public static bool FileCanRead(string filename, out string whyNot)
		{
			FileStream fileStream = null;
			try
			{
				fileStream = File.OpenRead(filename);
			}
			catch (Exception ex)
			{
				whyNot = ex.Message;
				return false;
				IL_001e:;
			}
			fileStream.Close();
			whyNot = string.Empty;
			return true;
		}

		public static void FileCopy(string filename, string targetFilename, Action onSuccess, Action<string> onError)
		{
			string text = string.Empty;
			try
			{
				File.Copy(filename, targetFilename);
			}
			catch (Exception ex)
			{
				text = ex.Message;
			}
			if (string.IsNullOrEmpty(text))
			{
				onSuccess?.Invoke();
			}
			else
			{
				onError?.Invoke(text);
			}
		}

		public static void FileMove(string filename, string targetFilename, Action onSuccess, Action<string> onError)
		{
			string text = string.Empty;
			try
			{
				File.Move(filename, targetFilename);
			}
			catch (Exception ex)
			{
				text = ex.Message;
			}
			if (string.IsNullOrEmpty(text))
			{
				onSuccess?.Invoke();
			}
			else
			{
				onError?.Invoke(text);
			}
		}

		public static float DivideSafe(float num, float den)
		{
			return (den == 0f) ? 0f : (num / den);
		}

		public static bool GetFileSize(string filename, out long size, out string errorStr)
		{
			try
			{
				FileInfo fileInfo = new FileInfo(filename);
				size = fileInfo.Length;
				errorStr = string.Empty;
				return true;
				IL_001d:
				bool result;
				return result;
			}
			catch (Exception ex)
			{
				size = 0L;
				errorStr = ex.Message;
				return false;
				IL_0036:
				bool result;
				return result;
			}
		}

		public static void FileMD5(string filename, Action<byte[]> onSuccess, Action<string> onError)
		{
			string text = string.Empty;
			byte[] array = null;
			try
			{
				MD5 mD = MD5.Create();
				array = mD.ComputeHash(File.OpenRead(filename));
			}
			catch (Exception ex)
			{
				array = null;
				text = ex.Message;
			}
			if (string.IsNullOrEmpty(text))
			{
				onSuccess?.Invoke(array);
			}
			else
			{
				onError?.Invoke(text);
			}
		}

		public static string JsonEncode(object obj)
		{
			return Json.Encode(obj);
		}

		public static object JsonDecode(string json)
		{
			return Json.Decode(json);
		}
	}
}
