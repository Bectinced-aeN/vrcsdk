using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VRC.Core;
using VRC.Core.BestHTTP;
using VRC.Core.BestHTTP.Caching;

namespace VRC
{
	public static class Tools
	{
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
				list.Add(obj.ToString());
			}
			return list;
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
			Caching.CleanCache();
			PlayerPrefs.DeleteAll();
			HTTPCacheService.BeginClear();
			string path = Application.get_persistentDataPath() + "/ab";
			if (Directory.Exists(path))
			{
				Directory.Delete(path, recursive: true);
			}
		}

		public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
		{
			return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTimeStamp).ToUniversalTime();
		}

		public static T[] FindSceneObjectsOfTypeAll<T>() where T : Component
		{
			return Resources.FindObjectsOfTypeAll<T>();
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
			int num = 0;
			for (int i = 0; i < 32; i++)
			{
				if (!string.IsNullOrEmpty(LayerMask.LayerToName(i)) && i > num)
				{
					num = i;
				}
			}
			return num + 1;
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
	}
}
