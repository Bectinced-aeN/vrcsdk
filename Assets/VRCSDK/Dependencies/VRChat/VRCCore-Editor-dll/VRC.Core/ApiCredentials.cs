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
			catch
			{
			}
			return !string.IsNullOrEmpty(authToken);
		}

		public static void Set(string humanName, string providerid, string provider, string token)
		{
			if (provider == null)
			{
				provider = string.Empty;
			}
			if (providerid == null)
			{
				providerid = string.Empty;
			}
			if (token == null)
			{
				token = string.Empty;
			}
			if (humanName == null)
			{
				humanName = string.Empty;
			}
			SecurePlayerPrefs.SetString("authTokenProvider", "vrchat", "vl9u1grTnvXA");
			SecurePlayerPrefs.SetString("authTokenProviderUserId", providerid, "vl9u1grTnvXA");
			SecurePlayerPrefs.SetString("authToken", token, "vl9u1grTnvXA");
			SecurePlayerPrefs.SetString("humanName", humanName, "vl9u1grTnvXA");
			humanName = humanName;
			authToken = token;
			providerid = providerid;
			provider = provider;
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
	}
}
