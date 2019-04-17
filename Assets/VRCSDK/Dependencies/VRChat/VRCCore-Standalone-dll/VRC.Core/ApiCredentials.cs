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

		public static bool Clear()
		{
			Set(null, null, null, null);
			return !Load();
		}

		public static bool Load()
		{
			authToken = (provider = (providerUserId = null));
			try
			{
				SecurePlayerPrefs.SetString("username", string.Empty, "vl9u1grTnvXA");
				SecurePlayerPrefs.SetString("password", string.Empty, "vl9u1grTnvXA");
				provider = SecurePlayerPrefs.GetString("authTokenProvider", "vl9u1grTnvXA");
				providerUserId = SecurePlayerPrefs.GetString("authTokenProviderUserId", "vl9u1grTnvXA");
				authToken = SecurePlayerPrefs.GetString("authToken", "vl9u1grTnvXA");
				humanName = SecurePlayerPrefs.GetString("humanName", "vl9u1grTnvXA");
			}
			catch (Exception ex)
			{
				Debug.LogError((object)("Exception loading credentials: " + ex.Message + "\n" + ex.StackTrace));
			}
			return !string.IsNullOrEmpty(authToken);
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
			SecurePlayerPrefs.SetString("authTokenProvider", _provider, "vl9u1grTnvXA");
			SecurePlayerPrefs.SetString("authTokenProviderUserId", _providerId, "vl9u1grTnvXA");
			SecurePlayerPrefs.SetString("authToken", _token, "vl9u1grTnvXA");
			SecurePlayerPrefs.SetString("humanName", _humanName, "vl9u1grTnvXA");
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
