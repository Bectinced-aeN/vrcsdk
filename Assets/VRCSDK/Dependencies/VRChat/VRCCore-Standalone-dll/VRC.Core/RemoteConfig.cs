using System;
using System.Collections.Generic;
using UnityEngine;
using VRC.Core.BestHTTP.JSON;

namespace VRC.Core
{
	public class RemoteConfig : ApiModel
	{
		public delegate void OnConfigInitialized();

		private const string SECURE_PLAYER_PREFS_PW = "vl9u1grTnvXA";

		private static Dictionary<string, object> config;

		public static OnConfigInitialized onConfigInitialized;

		public static void Init(bool fetchFreshConfig = true, Action onInit = null, Action onError = null)
		{
			if (HasCachedConfig())
			{
				LoadCachedConfig();
				if (fetchFreshConfig)
				{
					FetchConfig(onInit, onError);
				}
				else
				{
					onInit?.Invoke();
				}
			}
			else
			{
				FetchConfig(onInit, onError);
			}
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
				result = config.ContainsKey(key);
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
			if (IsInitialized() && config.ContainsKey(key))
			{
				result = (string)config[key];
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
			if (IsInitialized() && config.ContainsKey(key))
			{
				result = Tools.ObjListToStringList((List<object>)config[key]);
			}
			return result;
		}

		public static bool IsInitialized()
		{
			return config != null;
		}

		private static void FetchConfig(Action onFetched, Action onError)
		{
			Debug.Log((object)"Fetching fresh config");
			Logger.Log("FetchConfig", DebugLevel.All);
			ApiModel.SendGetRequest("config", delegate(string obj)
			{
				CacheConfig(obj);
				Logger.Log("Caching config!", DebugLevel.All);
			}, delegate(Dictionary<string, object> obj)
			{
				config = obj;
				Logger.Log("finshed fetching and set config", DebugLevel.All);
				if (onFetched != null)
				{
					onFetched();
				}
				if (onConfigInitialized != null)
				{
					onConfigInitialized();
				}
			}, delegate
			{
				Logger.LogError("Could not fetch fresh config file. Using cached if available.");
				if (onError != null)
				{
					onError();
				}
			}, needsAPIKey: false);
		}

		private static void CacheConfig(string configBlob)
		{
			SecurePlayerPrefs.SetString("configBlob", configBlob, "vl9u1grTnvXA");
			PlayerPrefs.Save();
		}

		private static void LoadCachedConfig()
		{
			if (SecurePlayerPrefs.HasKey("configBlob"))
			{
				string @string = SecurePlayerPrefs.GetString("configBlob", "vl9u1grTnvXA");
				Dictionary<string, object> dictionary = config = (Json.Decode(@string) as Dictionary<string, object>);
				Logger.Log("Cached Config: " + Json.Encode(config), DebugLevel.All);
			}
		}

		public static bool HasCachedConfig()
		{
			return SecurePlayerPrefs.HasKey("configBlob");
		}

		private static void ClearCachedConfig()
		{
			SecurePlayerPrefs.DeleteKey("configBlob");
			PlayerPrefs.Save();
		}
	}
}
