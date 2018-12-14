using UnityEngine;
using VRC.Core.BestHTTP.Authentication;

namespace VRC.Core
{
	public class ApiCredentials
	{
		private const string SECURE_PLAYER_PREFS_PW = "vl9u1grTnvXA";

		private static string username;

		private static string password;

		private static Credentials webCredentials;

		private static string authToken;

		public static bool Load()
		{
			if (!SecurePlayerPrefs.HasKey("username") || !SecurePlayerPrefs.HasKey("password"))
			{
				return false;
			}
			username = SecurePlayerPrefs.GetString("username", "vl9u1grTnvXA");
			password = SecurePlayerPrefs.GetString("password", "vl9u1grTnvXA");
			webCredentials = new Credentials(AuthenticationTypes.Basic, username, password);
			authToken = null;
			return true;
		}

		public static void SetUser(string user, string pass)
		{
			username = user;
			password = pass;
			webCredentials = new Credentials(AuthenticationTypes.Basic, username, password);
			authToken = null;
			SecurePlayerPrefs.SetString("username", username, "vl9u1grTnvXA");
			SecurePlayerPrefs.SetString("password", password, "vl9u1grTnvXA");
			PlayerPrefs.Save();
		}

		public static void Clear()
		{
			username = null;
			password = null;
			webCredentials = null;
			authToken = null;
			SecurePlayerPrefs.DeleteKey("username");
			SecurePlayerPrefs.DeleteKey("password");
			SecurePlayerPrefs.DeleteKey("authTokenProvider");
			SecurePlayerPrefs.DeleteKey("authTokenProviderUserId");
			PlayerPrefs.Save();
		}

		public static void SetAuthToken(string token, string provider)
		{
			username = null;
			password = null;
			webCredentials = null;
			authToken = token;
			SecurePlayerPrefs.SetString("authTokenProvider", provider, "vl9u1grTnvXA");
			PlayerPrefs.Save();
		}

		public static void SetAuthToken(string token, string provider, string providerUserId)
		{
			username = null;
			password = null;
			webCredentials = null;
			authToken = token;
			SecurePlayerPrefs.SetString("authTokenProvider", provider, "vl9u1grTnvXA");
			SecurePlayerPrefs.SetString("authTokenProviderUserId", providerUserId, "vl9u1grTnvXA");
			PlayerPrefs.Save();
		}

		public static object GetWebCredentials()
		{
			return webCredentials;
		}

		public static string GetAuthToken()
		{
			return authToken;
		}

		public static string GetAuthTokenProvider()
		{
			return SecurePlayerPrefs.GetString("authTokenProvider", "vl9u1grTnvXA");
		}

		public static string GetAuthTokenProviderUserId()
		{
			return SecurePlayerPrefs.GetString("authTokenProviderUserId", "vl9u1grTnvXA");
		}

		public static string GetUsername()
		{
			return username;
		}
	}
}
