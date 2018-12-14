using System;
using System.Collections.Generic;
using UnityEngine;
using VRC.Core.BestHTTP;

namespace VRC.Core
{
	public class RemoteConfig
	{
		public delegate void OnConfigInitialized();

		private const string SECURE_PLAYER_PREFS_PW = "vl9u1grTnvXA";

		private static Dictionary<string, object> config;

		public static OnConfigInitialized onConfigInitialized;

		private static string configBlobKey => API.GetApiUrl() + " " + API.GetOrganization() + " configBlob";

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
			if (!IsInitialized())
			{
				FetchConfig(onInit, onError);
			}
			else
			{
				onInit();
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
				result = (GetValue(key) != null);
			}
			return result;
		}

		public static object GetObject(string key)
		{
			if (!IsInitialized())
			{
				Init();
			}
			object result = null;
			if (IsInitialized() && HasKey(key))
			{
				result = GetValue(key);
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
					Debug.LogError((object)("RemoteConfig: " + key + " is not an int"));
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
					Debug.LogError((object)("RemoteConfig: " + key + " is not a bool"));
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

		public static List<Dictionary<string, string>> GetListOfDictionaries(string key)
		{
			if (!IsInitialized())
			{
				Init();
			}
			List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
			if (IsInitialized())
			{
				object value = GetValue(key);
				if (value != null)
				{
					try
					{
						List<object> list2 = (List<object>)value;
						if (list2 != null)
						{
							foreach (object item2 in list2)
							{
								if (item2 != null)
								{
									Dictionary<string, string> item = Tools.ObjDictToStringDict((Dictionary<string, object>)item2);
									list.Add(item);
								}
							}
							return list;
						}
						return list;
					}
					catch (Exception arg)
					{
						Debug.LogError((object)("Failed casting type from RemoteConfig - " + arg));
						return list;
					}
				}
			}
			return list;
		}

		public static bool IsInitialized()
		{
			return config != null;
		}

		private static void FetchConfig(Action onFetched = null, Action onError = null)
		{
			Logger.Log("FetchConfig", DebugLevel.All);
			API.SendRequest("config", HTTPMethods.Get, new ApiDictContainer
			{
				OnSuccess = delegate(ApiContainer c)
				{
					config = (c as ApiDictContainer).ResponseDictionary;
					Logger.Log("finshed fetching and set config", DebugLevel.All);
					if (onFetched != null)
					{
						onFetched();
					}
					if (onConfigInitialized != null)
					{
						onConfigInitialized();
					}
				},
				OnError = delegate
				{
					Logger.LogError("Could not fetch fresh config file. Using cached if available.");
					if (onError != null)
					{
						onError();
					}
				}
			}, null, needsAPIKey: false, authenticationRequired: false, disableCache: true);
		}
	}
}
