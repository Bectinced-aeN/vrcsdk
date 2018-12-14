using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VRC.Core.BestHTTP;
using VRC.Core.BestHTTP.Authentication;
using VRC.Core.BestHTTP.JSON;

namespace VRC.Core
{
	public class ApiModel : ScriptableObject
	{
		public const string releaseApiUrl = "https://api.vrchat.cloud/api/1/";

		public const string devApiUrl = "https://dev-api.vrchat.cloud/api/1/";

		private const int MAX_RETRY_COUNT = 2;

		public static string API_URL = "https://api.vrchat.cloud/api/1/";

		public static Dictionary<int, int> retryRequests;

		protected static string ApiKey;

		protected string mId;

		protected double mCreatedAtTimestamp;

		protected DateTime mCreatedAt;

		protected double mUpdatedAtTimestamp;

		protected DateTime mUpdatedAt;

		protected DateTime mFetchedAt;

		public string id => mId;

		public double createdAtTimestamp => mCreatedAtTimestamp;

		public DateTime createdAt => mCreatedAt;

		public double updatedAtTimestamp => mUpdatedAtTimestamp;

		public DateTime updatedAt => mUpdatedAt;

		public DateTime FetchedAt => mFetchedAt;

		public ApiModel()
			: this()
		{
		}

		public static void FetchApiKey(Action onSuccess = null, Action<string> onError = null)
		{
			Action action = delegate
			{
				if (RemoteConfig.IsInitialized())
				{
					ApiKey = RemoteConfig.GetString("clientApiKey");
					if (string.IsNullOrEmpty(ApiKey))
					{
						Logger.LogError("Could not fetch client api key - unknown error.");
						if (onError != null)
						{
							onError("Could not fetch client api key - unknown error.");
						}
					}
					else if (onSuccess != null)
					{
						onSuccess();
					}
				}
				else
				{
					Logger.LogWarning("Could not fetch client api key - config not initialized.");
					if (onError != null)
					{
						onError("Could not fetch client api key - config not initialized.");
					}
				}
			};
			if (string.IsNullOrEmpty(ApiKey))
			{
				if (!RemoteConfig.IsInitialized())
				{
					RemoteConfig.Init(fetchFreshConfig: false, action);
				}
				else
				{
					action();
				}
			}
			else if (onSuccess != null)
			{
				onSuccess();
			}
		}

		protected static void SendGetRequest(string endpoint, Action<List<object>> successCallback = null, Action<string> errorCallback = null)
		{
			SendRequest(endpoint, HTTPMethods.Get, null, successCallback, errorCallback);
		}

		protected static void SendGetRequest(string endpoint, Action<string> successCallbackWithResponse = null, Action<Dictionary<string, object>> successCallbackWithDict = null, Action<string> errorCallback = null, bool needsAPIKey = true)
		{
			SendRequest(null, null, endpoint, HTTPMethods.Get, null, successCallbackWithResponse, successCallbackWithDict, null, errorCallback, needsAPIKey);
		}

		protected static void SendGetRequest(string endpoint, Action<Dictionary<string, object>> successCallback = null, Action<string> errorCallback = null)
		{
			SendRequest(endpoint, HTTPMethods.Get, null, successCallback, errorCallback);
		}

		protected static void SendPostRequest(string endpoint, Dictionary<string, string> requestParams = null, Action<Dictionary<string, object>> successCallback = null, Action<string> errorCallback = null)
		{
			SendRequest(endpoint, HTTPMethods.Post, requestParams, successCallback, errorCallback);
		}

		protected static void SendPutRequest(string endpoint, Dictionary<string, string> requestParams = null, Action<Dictionary<string, object>> successCallback = null, Action<string> errorCallback = null)
		{
			SendRequest(endpoint, HTTPMethods.Put, requestParams, successCallback, errorCallback);
		}

		protected static void SendRequest(string endpoint, HTTPMethods method = HTTPMethods.Get, Dictionary<string, string> requestParams = null, Action<Dictionary<string, object>> successCallback = null, Action<string> errorCallback = null)
		{
			SendRequest(null, null, endpoint, method, requestParams, null, successCallback, null, errorCallback);
		}

		protected static void SendRequest(string endpoint, HTTPMethods method = HTTPMethods.Get, Dictionary<string, string> requestParams = null, Action<List<object>> successCallback = null, Action<string> errorCallback = null)
		{
			SendRequest(null, null, endpoint, method, requestParams, null, null, successCallback, errorCallback);
		}

		protected static void SendRequest(string username, string password, string endpoint, HTTPMethods method = HTTPMethods.Get, Dictionary<string, string> requestParams = null, Action<string> successCallbackWithResponse = null, Action<Dictionary<string, object>> successCallbackWithDict = null, Action<List<object>> successCallbackWithList = null, Action<string> errorCallback = null, bool needsAPIKey = true)
		{
			if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(password) && APIUser.CurrentUser != null)
			{
				username = APIUser.CurrentUser.username;
				password = APIUser.CurrentUser.password;
			}
			string apiUrl = GetApiUrl();
			int requestId = Random.Range(100, 999);
			Action action = delegate
			{
				//IL_016b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0172: Expected O, but got Unknown
				string text = apiUrl + endpoint + "?requestId=" + requestId.ToString() + "&apiKey=" + ApiKey;
				Logger.Log("[" + requestId + "] Sending " + method + " request to " + text, DebugLevel.API);
				if (requestParams != null)
				{
					string text2 = "&";
					int num = 0;
					foreach (KeyValuePair<string, string> requestParam in requestParams)
					{
						text2 = text2 + requestParam.Key + "=" + requestParam.Value;
						if (++num != requestParams.Count)
						{
							text2 += "&";
						}
					}
					text += text2;
				}
				HTTPRequest hTTPRequest = new HTTPRequest(new Uri(text), delegate(HTTPRequest req, HTTPResponse resp)
				{
					APIResponseHandler aPIResponseHandler = new EditorAPIResponseHandler();
					aPIResponseHandler.HandleReponse(req, resp, requestId, successCallbackWithResponse, successCallbackWithDict, successCallbackWithList, errorCallback, 2);
				});
				hTTPRequest.AddHeader("X-Requested-With", "XMLHttpRequest");
				hTTPRequest.MethodType = method;
				hTTPRequest.Credentials = new Credentials(AuthenticationTypes.Basic, username, password);
				WWWForm val = new WWWForm();
				if (requestParams != null)
				{
					foreach (KeyValuePair<string, string> requestParam2 in requestParams)
					{
						string text3 = (!string.IsNullOrEmpty(requestParam2.Value)) ? requestParam2.Value : string.Empty;
						val.AddField(requestParam2.Key, text3);
					}
				}
				Logger.Log("Request Params: " + Json.Encode(requestParams), DebugLevel.API);
				hTTPRequest.ConnectTimeout = TimeSpan.FromSeconds(20.0);
				hTTPRequest.Timeout = TimeSpan.FromSeconds(20.0);
				hTTPRequest.SetFields(val);
				hTTPRequest.Send();
			};
			if (needsAPIKey)
			{
				FetchApiKey(action, delegate(string message)
				{
					Logger.LogError("[" + requestId + "] Error sending request - " + message, DebugLevel.API);
				});
			}
			else
			{
				action();
			}
		}

		public static List<string> GetIds(IEnumerable<ApiModel> models)
		{
			List<string> list = new List<string>();
			foreach (ApiModel model in models)
			{
				list.Add(model.id);
			}
			return list;
		}

		public static void SetApiUrl(string url)
		{
			if (EditorPrefs.HasKey("VRC_ApiUrl") && url != EditorPrefs.GetString("VRC_ApiUrl"))
			{
				EditorPrefs.SetString("VRC_ApiUrl", url);
			}
		}

		public static string GetApiUrl()
		{
			if (!EditorPrefs.HasKey("VRC_ApiUrl"))
			{
				EditorPrefs.SetString("VRC_ApiUrl", API_URL);
			}
			return EditorPrefs.GetString("VRC_ApiUrl");
		}

		public static void ResetApi()
		{
			if (EditorPrefs.HasKey("VRC_ApiUrl"))
			{
				EditorPrefs.DeleteKey("VRC_ApiUrl");
			}
		}

		public static bool IsDevApi()
		{
			return GetApiUrl() == "https://dev-api.vrchat.cloud/api/1/";
		}

		protected virtual Dictionary<string, string> BuildWebParameters()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["id"] = mId;
			return dictionary;
		}
	}
}
