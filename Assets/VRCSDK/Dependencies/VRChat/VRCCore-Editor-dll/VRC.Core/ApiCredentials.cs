using System;
using UnityEngine;

namespace VRC.Core
{
	public class ApiCredentials
	{
		private const string SECURE_PLAYER_PREFS_PW = "vl9u1grTnvXA";

		private static string authToken;

		private static string provider;

		private static string providerUserId;

		private static string humanName;

		private static uint? index;

		public static bool Clear()
		{
			Set(null, null, null, null);
			return !Load();
		}

		public static void SetProfileIndex(uint _index)
		{
			if (_index == 0)
			{
				index = null;
			}
			else
			{
				index = _index;
			}
		}

		private static void SetString(string key, string value)
		{
			key = string.Format("{0}{1}", key, (!index.HasValue) ? string.Empty : ("[" + index.Value.ToString() + "]"));
			SecurePlayerPrefs.SetString(key, value, "vl9u1grTnvXA");
		}

		private static string GetString(string key)
		{
			key = string.Format("{0}{1}", key, (!index.HasValue) ? string.Empty : ("[" + index.Value.ToString() + "]"));
			return SecurePlayerPrefs.GetString(key, "vl9u1grTnvXA");
		}

		public static bool Load()
		{
			authToken = (provider = (providerUserId = null));
			try
			{
				SetString("username", string.Empty);
				SetString("password", string.Empty);
				provider = GetString("authTokenProvider");
				providerUserId = GetString("authTokenProviderUserId");
				authToken = GetString("authToken");
				humanName = GetString("humanName");
			}
			catch (Exception ex)
			{
				Debug.LogError((object)("Exception loading credentials: " + ex.Message + "\n" + ex.StackTrace));
			}
			return !string.IsNullOrEmpty(authToken);
		}

		public static void SetHumanName(string _humanName)
		{
			if (!string.IsNullOrEmpty(_humanName))
			{
				SetString("humanName", _humanName);
				humanName = _humanName;
				PlayerPrefs.Save();
			}
		}

		public static void Set(string _humanName, string _providerId, string _provider, string _token)
		{
			if (_provider == null)
			{
				_provider = string.Empty;
			}
			if (_providerId == null)
			{
				_providerId = string.Empty;
			}
			if (_token == null)
			{
				_token = string.Empty;
			}
			if (_humanName == null)
			{
				_humanName = string.Empty;
			}
			SetString("authTokenProvider", _provider);
			SetString("authTokenProviderUserId", _providerId);
			SetString("authToken", _token);
			SetString("humanName", _humanName);
			humanName = _humanName;
			authToken = _token;
			providerUserId = _providerId;
			provider = _provider;
			PlayerPrefs.Save();
		}

		public static string GetHumanName()
		{
			return humanName;
		}

		public static string GetAuthToken()
		{
			return authToken;
		}

		public static string GetAuthTokenProvider()
		{
			return provider;
		}

		public static string GetAuthTokenProviderUserId()
		{
			return providerUserId;
		}

		public static bool IsLoaded()
		{
			return !string.IsNullOrEmpty(authToken);
		}
	}
}
