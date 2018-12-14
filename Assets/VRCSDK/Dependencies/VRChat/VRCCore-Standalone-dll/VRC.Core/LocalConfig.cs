using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VRC.Core.BestHTTP.JSON;

namespace VRC.Core
{
	public class LocalConfig
	{
		public delegate void OnConfigInitialized();

		private static Dictionary<string, object> config;

		public static OnConfigInitialized onConfigInitialized;

		private static object GetValue(string key)
		{
			if (config.ContainsKey(key))
			{
				return config[key];
			}
			return null;
		}

		public static void Init(Action onInit = null, Action onError = null)
		{
			FetchConfig(onInit, onError);
		}

		public static bool HasKey(string key)
		{
			if (!IsInitialized())
			{
				Init();
			}
			bool result = false;
			if (IsInitialized())
			{
				result = (GetValue(key) != null);
			}
			return result;
		}

		public static string GetString(string key)
		{
			if (!IsInitialized())
			{
				Init();
			}
			string result = null;
			if (IsInitialized() && HasKey(key))
			{
				result = GetValue(key).ToString();
			}
			return result;
		}

		public static int GetInt(string key, int defaultVal = 0)
		{
			if (!IsInitialized())
			{
				Init();
			}
			int result = defaultVal;
			if (IsInitialized() && HasKey(key))
			{
				object value = GetValue(key);
				if (!(value is double) || (double)value != (double)(int)(double)value)
				{
					Debug.LogError((object)("LocalConfig: " + key + " is not an int"));
				}
				else
				{
					result = (int)(double)value;
				}
			}
			return result;
		}

		public static bool GetBool(string key, bool defaultVal = false)
		{
			if (!IsInitialized())
			{
				Init();
			}
			bool result = defaultVal;
			if (IsInitialized() && HasKey(key))
			{
				object value = GetValue(key);
				if (!(value is bool))
				{
					Debug.LogError((object)("LocalConfig: " + key + " is not a bool"));
				}
				else
				{
					result = (bool)value;
				}
			}
			return result;
		}

		public static List<string> GetList(string key)
		{
			if (!IsInitialized())
			{
				Init();
			}
			List<string> result = new List<string>();
			if (IsInitialized())
			{
				object value = GetValue(key);
				if (value != null)
				{
					result = Tools.ObjListToStringList((List<object>)value);
				}
			}
			return result;
		}

		public static bool IsInitialized()
		{
			return config != null;
		}

		private static void FetchConfig(Action onFetched = null, Action onError = null)
		{
			Debug.Log((object)"Fetching fresh local config");
			Logger.Log("FetchLocalConfig", DebugLevel.All);
			string path = Application.get_persistentDataPath() + Path.PathSeparator + "config.json";
			if (File.Exists(path))
			{
				try
				{
					object obj = Json.Decode(File.ReadAllText(path));
					config = (Dictionary<string, object>)obj;
					Logger.Log("finshed fetching and set local config", DebugLevel.All);
					onFetched?.Invoke();
					if (onConfigInitialized != null)
					{
						onConfigInitialized();
					}
				}
				catch (Exception)
				{
					Debug.LogError((object)"Error reading local config");
					onError?.Invoke();
				}
			}
			else
			{
				config = new Dictionary<string, object>();
				Logger.Log("no local config found", DebugLevel.All);
				onFetched?.Invoke();
				if (onConfigInitialized != null)
				{
					onConfigInitialized();
				}
			}
		}
	}
}
