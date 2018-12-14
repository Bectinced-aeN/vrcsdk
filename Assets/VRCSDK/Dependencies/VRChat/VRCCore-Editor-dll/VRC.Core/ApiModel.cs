using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using VRC.Core.BestHTTP;
using VRC.Core.BestHTTP.Authentication;
using VRC.Core.BestHTTP.Cookies;
using VRC.Core.BestHTTP.JSON;

namespace VRC.Core
{
	public class ApiModel
	{
		protected class RequestInfo
		{
			public string endpoint;

			public HTTPMethods method = HTTPMethods.Post;

			public Dictionary<string, object> requestParams;

			public Action<string> errorCallback;

			public Action<string> successCallbackWithResponse;

			public Action<Dictionary<string, object>> successCallbackWithDict;

			public Action<List<object>> successCallbackWithList;
		}

		public const string devApiUrl = "https://dev-api.vrchat.cloud/api/1/";

		public const string betaApiUrl = "https://beta-api.vrchat.cloud/api/1/";

		public const string releaseApiUrl = "https://api.vrchat.cloud/api/1/";

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

		public static string DeviceID => SystemInfo.get_deviceUniqueIdentifier();

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

		protected static void AppendQuery(ref UriBuilder baseUri, string queryToAppend)
		{
			if (baseUri.Query != null && baseUri.Query.Length > 1)
			{
				baseUri.Query = baseUri.Query.Substring(1) + "&" + queryToAppend;
			}
			else
			{
				baseUri.Query = queryToAppend;
			}
		}

		protected static void SendGetRequest(string endpoint, Action<List<object>> successCallback = null, Action<string> errorCallback = null)
		{
			SendRequest(endpoint, HTTPMethods.Get, null, successCallback, errorCallback);
		}

		protected static void SendGetRequest(string endpoint, Action<string> successCallbackWithResponse = null, Action<Dictionary<string, object>> successCallbackWithDict = null, Action<string> errorCallback = null, bool needsAPIKey = true)
		{
			SendRequest(endpoint, HTTPMethods.Get, (Dictionary<string, string>)null, successCallbackWithResponse, successCallbackWithDict, (Action<List<object>>)null, errorCallback, needsAPIKey, authenticationRequired: true, -1f);
		}

		protected static void SendGetRequest(string endpoint, Action<Dictionary<string, object>> successCallback = null, Action<string> errorCallback = null)
		{
			SendRequest(endpoint, HTTPMethods.Get, null, successCallback, errorCallback);
		}

		protected static void SendPostRequest(string endpoint, Action<Dictionary<string, object>> successCallback = null, Action<string> errorCallback = null)
		{
			SendRequest(endpoint, HTTPMethods.Post, null, successCallback, errorCallback);
		}

		protected static void SendPostRequest(string endpoint, Dictionary<string, string> requestParams = null, Action<Dictionary<string, object>> successCallback = null, Action<string> errorCallback = null)
		{
			SendRequest(endpoint, HTTPMethods.Post, requestParams, successCallback, errorCallback);
		}

		protected static void SendPostRequest(string endpoint, Dictionary<string, object> requestParams = null, Action<Dictionary<string, object>> successCallback = null, Action<string> errorCallback = null)
		{
			SendRequest(endpoint, HTTPMethods.Post, requestParams, null, successCallback, null, errorCallback);
		}

		protected static void SendPutRequest(string endpoint, Action<Dictionary<string, object>> successCallback = null, Action<string> errorCallback = null)
		{
			SendRequest(endpoint, HTTPMethods.Put, null, successCallback, errorCallback);
		}

		protected static void SendPutRequest(string endpoint, Dictionary<string, string> requestParams = null, Action<Dictionary<string, object>> successCallback = null, Action<string> errorCallback = null)
		{
			SendRequest(endpoint, HTTPMethods.Put, requestParams, successCallback, errorCallback);
		}

		protected static void SendPutRequest(string endpoint, Dictionary<string, object> requestParams = null, Action<Dictionary<string, object>> successCallback = null, Action<string> errorCallback = null)
		{
			SendRequest(endpoint, HTTPMethods.Put, requestParams, null, successCallback, null, errorCallback);
		}

		protected static void SendRequest(string endpoint, HTTPMethods method = HTTPMethods.Get, Dictionary<string, string> requestParams = null, Action<Dictionary<string, object>> successCallback = null, Action<string> errorCallback = null, bool needsAPIKey = true, bool authenticationRequired = true, float cacheLifetime = -1f)
		{
			SendRequest(endpoint, method, requestParams, null, successCallback, null, errorCallback, needsAPIKey, authenticationRequired, cacheLifetime);
		}

		protected static void SendRequest(string endpoint, HTTPMethods method = HTTPMethods.Get, Dictionary<string, string> requestParams = null, Action<List<object>> successCallback = null, Action<string> errorCallback = null, bool needsAPIKey = true, bool authenticationRequired = true, float cacheLifetime = -1f)
		{
			SendRequest(endpoint, method, requestParams, null, null, successCallback, errorCallback, needsAPIKey, authenticationRequired, cacheLifetime);
		}

		protected static void SendRequest(string endpoint, HTTPMethods method = HTTPMethods.Get, Dictionary<string, string> requestParams = null, Action<string> successCallbackWithResponse = null, Action<Dictionary<string, object>> successCallbackWithDict = null, Action<List<object>> successCallbackWithList = null, Action<string> errorCallback = null, bool needsAPIKey = true, bool authenticationRequired = true, float cacheLifetime = -1f)
		{
			Dictionary<string, object> dictionary = null;
			if (requestParams != null)
			{
				dictionary = new Dictionary<string, object>();
				foreach (KeyValuePair<string, string> requestParam in requestParams)
				{
					dictionary[requestParam.Key] = requestParam.Value;
				}
			}
			SendRequest(endpoint, method, dictionary, successCallbackWithResponse, successCallbackWithDict, successCallbackWithList, errorCallback, needsAPIKey, authenticationRequired, cacheLifetime);
		}

		protected static void SendRequest(string endpoint, HTTPMethods method = HTTPMethods.Get, Dictionary<string, object> requestParams = null, Action<string> successCallbackWithResponse = null, Action<Dictionary<string, object>> successCallbackWithDict = null, Action<List<object>> successCallbackWithList = null, Action<string> errorCallback = null, bool needsAPIKey = true, bool authenticationRequired = true, float cacheLifetime = -1f)
		{
			string apiUrl = GetApiUrl();
			int requestId = Random.Range(100, 999);
			Action action = delegate
			{
				string uri = apiUrl + endpoint;
				UriBuilder baseUri = new UriBuilder(uri);
				if (!string.IsNullOrEmpty(ApiKey))
				{
					AppendQuery(ref baseUri, "apiKey=" + ApiKey);
				}
				string text = null;
				if (requestParams != null)
				{
					if (method == HTTPMethods.Get)
					{
						foreach (KeyValuePair<string, object> requestParam in requestParams)
						{
							AppendQuery(ref baseUri, requestParam.Key + "=" + requestParam.Value);
						}
					}
					else
					{
						text = Json.Encode(requestParams);
					}
				}
				if (APIUser.CurrentUser != null && APIUser.CurrentUser.developerType == APIUser.DeveloperType.Internal)
				{
					string text2 = (method == HTTPMethods.Get) ? string.Empty : Json.Encode(requestParams);
					Debug.Log((object)("<color=cyan>[" + requestId + "] Sending request: " + method.ToString() + " " + baseUri.Uri + " with params: " + text2 + "</color>"));
				}
				else
				{
					Logger.Log("[" + requestId + "] Sending " + method + " request to " + baseUri.Uri, DebugLevel.API);
				}
				ApiCachedResponse apiCachedResponse = (method != 0) ? null : APIResponseHandler.GetOrClearCachedResponse(baseUri.Uri.PathAndQuery, cacheLifetime);
				if (apiCachedResponse != null)
				{
					if (APIUser.CurrentUser != null && APIUser.CurrentUser.developerType == APIUser.DeveloperType.Internal)
					{
						Debug.Log((object)("<color=lime>[" + requestId + "] Using Cached Response (age " + (Time.get_realtimeSinceStartup() - apiCachedResponse.Timestamp) + "s, max " + apiCachedResponse.Lifetime + "s):\n" + apiCachedResponse.Data + "</color>"));
					}
					APIResponseHandler aPIResponseHandler = new EditorAPIResponseHandler();
					aPIResponseHandler.HandleSuccessReponse(apiCachedResponse.Data, successCallbackWithResponse, successCallbackWithDict, successCallbackWithList);
				}
				else
				{
					HTTPRequest hTTPRequest = new HTTPRequest(baseUri.Uri, delegate(HTTPRequest req, HTTPResponse resp)
					{
						APIResponseHandler aPIResponseHandler2 = new EditorAPIResponseHandler();
						aPIResponseHandler2.HandleReponse(req, resp, requestId, successCallbackWithResponse, successCallbackWithDict, successCallbackWithList, errorCallback, 2, cacheLifetime);
					});
					hTTPRequest.AddHeader("X-Requested-With", "XMLHttpRequest");
					hTTPRequest.AddHeader("X-MacAddress", DeviceID);
					hTTPRequest.AddHeader("Content-Type", (method != 0) ? "application/json" : "application/x-www-form-urlencoded");
					hTTPRequest.MethodType = method;
					hTTPRequest.Credentials = (ApiCredentials.GetWebCredentials() as Credentials);
					if (authenticationRequired && ApiCredentials.GetAuthToken() != null)
					{
						List<Cookie> cookies = hTTPRequest.Cookies;
						cookies.Add(new Cookie("auth", ApiCredentials.GetAuthToken()));
						hTTPRequest.Cookies = cookies;
					}
					hTTPRequest.ConnectTimeout = TimeSpan.FromSeconds(20.0);
					hTTPRequest.Timeout = TimeSpan.FromSeconds(20.0);
					if (!string.IsNullOrEmpty(text))
					{
						hTTPRequest.RawData = Encoding.UTF8.GetBytes(text);
					}
					hTTPRequest.DisableCache = true;
					hTTPRequest.Send();
				}
			};
			if (needsAPIKey)
			{
				FetchApiKey(action, delegate(string message)
				{
					Debug.LogError((object)("[" + requestId + "] Error sending request - " + message));
				});
			}
			else
			{
				action();
			}
		}

		protected static void SendRequestBatch(IEnumerable<RequestInfo> requests, bool needsAPIKey = true)
		{
			if (needsAPIKey)
			{
				FetchApiKey(delegate
				{
					SendRequestBatch(requests, needsAPIKey: false);
				}, delegate(string message)
				{
					Debug.LogError((object)("Error sending request - " + message));
				});
			}
			else
			{
				foreach (RequestInfo request in requests)
				{
					SendRequest(request.endpoint, request.method, request.requestParams, request.successCallbackWithResponse, request.successCallbackWithDict, request.successCallbackWithList, request.errorCallback, needsAPIKey: false);
				}
			}
		}

		public static void CleanExpiredReponseCache()
		{
			APIResponseHandler.CleanExpiredCache();
		}

		public static void ClearReponseCache()
		{
			APIResponseHandler.CleanExpiredCache();
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

		public static string GetAssetPlatformString()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Invalid comparison between Unknown and I4
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Invalid comparison between Unknown and I4
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Invalid comparison between Unknown and I4
			BuildTarget activeBuildTarget = EditorUserBuildSettings.get_activeBuildTarget();
			if ((int)activeBuildTarget != 5)
			{
				if ((int)activeBuildTarget == 13)
				{
					return "android";
				}
				if ((int)activeBuildTarget != 19)
				{
					return "unknownplatform";
				}
			}
			return "standalonewindows";
		}

		public static void SetApiUrlFromEnvironment(ApiServerEnvironment env)
		{
			SetApiUrl(GetApiUrlForEnvironment(env));
		}

		public static string GetApiUrlForEnvironment(ApiServerEnvironment env)
		{
			switch (env)
			{
			case ApiServerEnvironment.Dev:
				return "https://dev-api.vrchat.cloud/api/1/";
			case ApiServerEnvironment.Beta:
				return "https://beta-api.vrchat.cloud/api/1/";
			case ApiServerEnvironment.Release:
				return "https://api.vrchat.cloud/api/1/";
			default:
				Debug.LogError((object)("Unknown server environment! " + env.ToString()));
				return string.Empty;
			}
		}

		public static void SetApiUrl(string url)
		{
			API_URL = url;
		}

		public static string GetApiUrl()
		{
			return API_URL;
		}

		public static bool IsDevApi()
		{
			return GetApiUrl() == "https://dev-api.vrchat.cloud/api/1/";
		}

		public static ApiServerEnvironment GetServerEnvironmentForApiUrl()
		{
			return GetServerEnvironmentForApiUrl(API_URL);
		}

		public static ApiServerEnvironment GetServerEnvironmentForApiUrl(string url)
		{
			if (GetApiUrl() == "https://api.vrchat.cloud/api/1/")
			{
				return ApiServerEnvironment.Release;
			}
			if (GetApiUrl() == "https://beta-api.vrchat.cloud/api/1/")
			{
				return ApiServerEnvironment.Beta;
			}
			if (GetApiUrl() == "https://dev-api.vrchat.cloud/api/1/")
			{
				return ApiServerEnvironment.Dev;
			}
			Debug.LogError((object)("GetServerEnvironmentForApiUrl: unknown api url: " + url));
			return ApiServerEnvironment.Release;
		}

		protected virtual Dictionary<string, string> BuildWebParameters()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["id"] = mId;
			return dictionary;
		}
	}
}
