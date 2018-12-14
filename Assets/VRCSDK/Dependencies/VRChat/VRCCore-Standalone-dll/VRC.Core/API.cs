using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VRC.Core.BestHTTP;
using VRC.Core.BestHTTP.Authentication;
using VRC.Core.BestHTTP.Cookies;
using VRC.Core.BestHTTP.JSON;

namespace VRC.Core
{
	public static class API
	{
		public class EndpointAccessEntry
		{
			public float time;

			public int count;
		}

		public const string devApiUrl = "https://dev-api.vrchat.cloud/api/1/";

		public const string betaApiUrl = "https://beta-api.vrchat.cloud/api/1/";

		public const string releaseApiUrl = "https://api.vrchat.cloud/api/1/";

		public const float ResponseCacheLifetime = 3600f;

		public const int MAX_RETRY_COUNT = 2;

		public static string API_URL = "https://api.vrchat.cloud/api/1/";

		public static string API_ORGANIZATION = "vrchat";

		public static ApiOnlineMode API_ONLINE_MODE = ApiOnlineMode.Uninitialized;

		public static string ApiKey;

		public static Dictionary<string, EndpointAccessEntry> EndpointAccessTimes = new Dictionary<string, EndpointAccessEntry>();

		private static int lastRequestId = -1;

		private static Dictionary<string, HTTPRequest> activeRequests = new Dictionary<string, HTTPRequest>();

		private static List<object> offlineQueries;

		public static string DeviceID => SystemInfo.get_deviceUniqueIdentifier();

		public static void SetOrganization(string s)
		{
			API_ORGANIZATION = s;
		}

		public static string GetOrganization()
		{
			return API_ORGANIZATION;
		}

		public static bool IsReady()
		{
			return API_ORGANIZATION != null && API_ONLINE_MODE != ApiOnlineMode.Uninitialized;
		}

		public static T FromCacheOrNew<T>(string id, float maxCacheAge = -1f) where T : ApiModel, ApiCacheObject, new()
		{
			T val = new T();
			val.id = id;
			T target = val;
			if (!ApiCache.Fetch(id, ref target, maxCacheAge))
			{
				ApiCache.Save(id, target);
			}
			return target;
		}

		public static T CreateFromJson<T>(Dictionary<string, object> json) where T : ApiModel, ApiCacheObject, new()
		{
			try
			{
				T result = new T();
				string Error = null;
				if (!result.SetApiFieldsFromJson(json, ref Error))
				{
					Debug.LogError((object)(typeof(T).Name + ": Unable to CreateFromJson: " + Error));
					return (T)null;
				}
				return result;
				IL_004f:
				T result2;
				return result2;
			}
			catch (Exception ex)
			{
				Debug.LogError((object)(typeof(T).Name + ": Unable to CreateFromJson: " + ex.Message + "\n" + ex.StackTrace));
				return (T)null;
				IL_00a5:
				T result2;
				return result2;
			}
		}

		public static T Clone<T>(ApiModel model) where T : ApiModel, ApiCacheObject, new()
		{
			return model.Clone(typeof(T)) as T;
		}

		public unsafe static T Fetch<T>(string id, Action<ApiContainer> onSuccess = null, Action<ApiContainer> onFailure = null, bool disableCache = false) where T : ApiModel, ApiCacheObject, new()
		{
			T val = new T();
			val.id = id;
			T model = (T)val;
			if (!disableCache && ApiCache.Fetch(id, ref *(T*)(&model)))
			{
				if (onSuccess != null)
				{
					UpdateDelegator.Dispatch(delegate
					{
						onSuccess(new ApiModelContainer<T>((T)model));
					});
				}
				return (T)model;
			}
			model.Fetch(onSuccess, onFailure);
			return (T)model;
		}

		public static void Delete<T>(string id, Action<ApiContainer> onSuccess = null, Action<ApiContainer> onFailure = null) where T : ApiModel, ApiCacheObject, new()
		{
			T val = new T();
			val.id = id;
			T val2 = val;
			val2.Delete(onSuccess, onFailure);
		}

		public static void Save(ApiModel model, Action<ApiContainer> onSuccess = null, Action<ApiContainer> onFailure = null)
		{
			model.Save(onSuccess, onFailure);
		}

		public static void Save(IEnumerable<ApiModel> models, Action<ApiContainer> onSuccess = null, Action<ApiContainer> onFailure = null)
		{
			foreach (ApiModel model in models)
			{
				model.Save(onSuccess, onFailure);
			}
		}

		public static void Post(ApiModel model, Action<ApiContainer> onSuccess = null, Action<ApiContainer> onFailure = null)
		{
			model.Post(onSuccess, onFailure);
		}

		public static void Post(IEnumerable<ApiModel> models, Action<ApiContainer> onSuccess = null, Action<ApiContainer> onFailure = null)
		{
			foreach (ApiModel model in models)
			{
				model.Post(onSuccess, onFailure);
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

		public static string GetAssetPlatformString()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Invalid comparison between Unknown and I4
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Invalid comparison between Unknown and I4
			RuntimePlatform platform = Application.get_platform();
			if ((int)platform == 2 || (int)platform == 7)
			{
				return "standalonewindows";
			}
			return "unknownplatform";
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

		public static void SendGetRequest(string target, ApiContainer responseContainer = null, Dictionary<string, object> requestParams = null, bool disableCache = false, float cacheLifetime = 3600f)
		{
			SendRequest(target, HTTPMethods.Get, responseContainer, requestParams, needsAPIKey: true, authenticationRequired: true, disableCache, cacheLifetime);
		}

		public static void SendPostRequest(string target, ApiContainer responseContainer = null, Dictionary<string, object> requestParams = null)
		{
			SendRequest(target, HTTPMethods.Post, responseContainer, requestParams);
		}

		public static void SendPutRequest(string target, ApiContainer responseContainer = null, Dictionary<string, object> requestParams = null)
		{
			SendRequest(target, HTTPMethods.Put, responseContainer, requestParams);
		}

		public static void SendDeleteRequest(string target, ApiContainer responseContainer = null, Dictionary<string, object> requestParams = null)
		{
			SendRequest(target, HTTPMethods.Delete, responseContainer, requestParams);
		}

		public static List<T> ConvertJsonListToModelList<T>(List<object> json, ref string error, float dataTimestamp) where T : ApiModel, new()
		{
			if (json != null)
			{
				try
				{
					List<T> list = new List<T>();
					foreach (object item2 in json)
					{
						Dictionary<string, object> fields = item2 as Dictionary<string, object>;
						T item = new T();
						if (!item.SetApiFieldsFromJson(fields, ref error))
						{
							return null;
						}
						list.Add(item);
					}
					return list;
					IL_0079:
					List<T> result;
					return result;
				}
				catch (Exception ex)
				{
					error = "An exception was caught when filling the models: " + ex.Message + "\n" + ex.StackTrace;
					return null;
					IL_00a7:
					List<T> result;
					return result;
				}
			}
			return null;
		}

		public static void SendRequest(string endpoint, HTTPMethods method, ApiContainer responseContainer = null, Dictionary<string, object> requestParams = null, bool needsAPIKey = true, bool authenticationRequired = true, bool disableCache = false, float cacheLifetime = 3600f)
		{
			string text = (!disableCache) ? "cyan" : "red";
			Debug.Log((object)("<color=" + text + ">Dispatch " + method + " " + endpoint + ((requestParams == null) ? string.Empty : (" params: " + Json.Encode(requestParams))) + " disableCache: " + disableCache.ToString() + "</color>"));
			UpdateDelegator.Dispatch(delegate
			{
				SendRequestInternal(endpoint, method, responseContainer, requestParams, needsAPIKey, authenticationRequired, disableCache, cacheLifetime);
			});
		}

		private static void SendRequestInternal(string endpoint, HTTPMethods method, ApiContainer responseContainer = null, Dictionary<string, object> requestParams = null, bool needsAPIKey = true, bool authenticationRequired = true, bool disableCache = false, float cacheLifetime = 3600f)
		{
			if (responseContainer == null)
			{
				responseContainer = new ApiContainer();
			}
			if (API_ONLINE_MODE == ApiOnlineMode.Offline)
			{
				SendOfflineRequest(endpoint, method, responseContainer, requestParams);
			}
			else
			{
				if (API_ONLINE_MODE == ApiOnlineMode.Uninitialized)
				{
					Debug.LogError((object)"Api Web Request send before online mode is initialized.");
				}
				string apiUrl = GetApiUrl();
				Action action = delegate
				{
					string uri = apiUrl + endpoint;
					UriBuilder baseUri = new UriBuilder(uri);
					if (!string.IsNullOrEmpty(ApiKey))
					{
						AppendQuery(ref baseUri, "apiKey=" + ApiKey);
					}
					if (API_ORGANIZATION == null)
					{
						throw new Exception("ApiModel does not have it's organization set!");
					}
					AppendQuery(ref baseUri, "organization=" + API_ORGANIZATION);
					string text = null;
					if (requestParams != null)
					{
						if (method == HTTPMethods.Get)
						{
							foreach (KeyValuePair<string, object> requestParam in requestParams)
							{
								string text2 = null;
								AppendQuery(ref baseUri, string.Concat(str2: (!(requestParam.Value is string)) ? ((!typeof(List<>).IsAssignableFrom(requestParam.Value.GetType())) ? Json.Encode(requestParam.Value) : Json.Encode((requestParam.Value as IList).Cast<object>().ToArray())) : (requestParam.Value as string), str0: requestParam.Key, str1: "="));
							}
						}
						else
						{
							text = Json.Encode(requestParams);
						}
					}
					string uriPath = baseUri.Uri.PathAndQuery;
					bool useCache = !disableCache && method == HTTPMethods.Get;
					if (endpoint == "worlds/recent")
					{
						ApiCache.recentWorldsQuery = uriPath;
					}
					ApiCache.CachedResponse cachedResponse = (!useCache) ? null : ApiCache.GetOrClearCachedResponse(baseUri.Uri.PathAndQuery, cacheLifetime);
					if (cachedResponse != null)
					{
						Debug.Log((object)("<color=cyan>Using cached " + method + " request to " + baseUri.Uri + "</color>"));
						try
						{
							if (responseContainer.OnComplete(success: true, baseUri.Uri.PathAndQuery, 200, string.Empty, () => cachedResponse.Data, () => cachedResponse.DataAsText, cachedResponse.Timestamp))
							{
								responseContainer.OnSuccess(responseContainer);
							}
							else
							{
								Debug.LogError((object)"Something went wrong re-serving data from cache.");
							}
						}
						catch (Exception ex)
						{
							Debug.LogException(ex);
						}
					}
					else if (method == HTTPMethods.Get && activeRequests.ContainsKey(uriPath))
					{
						Debug.Log((object)("<color=cyan>Piggy-backing " + method + " request to " + baseUri.Uri + "</color>"));
						OnRequestFinishedDelegate originalCallback = activeRequests[uriPath].Callback;
						activeRequests[uriPath].Callback = delegate(HTTPRequest req, HTTPResponse resp)
						{
							if (activeRequests.ContainsKey(uriPath))
							{
								activeRequests.Remove(uriPath);
							}
							if (originalCallback != null)
							{
								originalCallback(req, resp);
							}
							try
							{
								APIResponseHandler.HandleReponse(0, req, resp, responseContainer, 2, useCache);
							}
							catch (Exception ex2)
							{
								Debug.LogException(ex2);
							}
						};
					}
					else
					{
						int requestId = ++lastRequestId;
						Debug.Log((object)("<color=lightblue>[" + requestId + "] Sending " + method + " request to " + baseUri.Uri + "</color>"));
						HTTPRequest hTTPRequest = new HTTPRequest(baseUri.Uri, delegate(HTTPRequest req, HTTPResponse resp)
						{
							if (activeRequests.ContainsKey(uriPath))
							{
								activeRequests.Remove(uriPath);
							}
							APIResponseHandler.HandleReponse(requestId, req, resp, responseContainer, 2, useCache);
						});
						if (method == HTTPMethods.Get)
						{
							activeRequests.Add(uriPath, hTTPRequest);
						}
						hTTPRequest.AddHeader("X-Requested-With", "XMLHttpRequest");
						hTTPRequest.AddHeader("X-MacAddress", DeviceID);
						hTTPRequest.AddHeader("Content-Type", (method != 0) ? "application/json" : "application/x-www-form-urlencoded");
						hTTPRequest.AddHeader("Origin", "vrchat.com");
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
					string key = endpoint.ToLower().Split('?')[0];
					if (!EndpointAccessTimes.ContainsKey(key))
					{
						EndpointAccessTimes.Add(key, new EndpointAccessEntry
						{
							count = 1,
							time = Time.get_realtimeSinceStartup()
						});
					}
					else
					{
						EndpointAccessTimes[key].time = Time.get_realtimeSinceStartup();
						EndpointAccessTimes[key].count++;
					}
				};
				if (needsAPIKey && !IsOffline())
				{
					FetchApiKey(action);
				}
				else
				{
					action();
				}
			}
		}

		private static void AppendQuery(ref UriBuilder baseUri, string queryToAppend)
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

		private static void FetchApiKey(Action onSuccess = null, Action<string> onError = null)
		{
			Action onInit = delegate
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
				RemoteConfig.Init(onInit);
			}
			else if (onSuccess != null)
			{
				onSuccess();
			}
		}

		public static void SetOnlineMode(bool online, string organization = null)
		{
			if (!online)
			{
				API_ONLINE_MODE = ApiOnlineMode.Offline;
				string text = Resources.Load<TextAsset>("offline").get_text();
				object obj = Json.Decode(text);
				offlineQueries = (obj as List<object>);
			}
			else
			{
				API_ONLINE_MODE = ApiOnlineMode.Online;
			}
			if (!string.IsNullOrEmpty(organization))
			{
				SetOrganization(organization);
			}
		}

		public static bool IsOffline()
		{
			return API_ONLINE_MODE == ApiOnlineMode.Offline;
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

		private static void SendOfflineRequest(string endpoint, HTTPMethods method, ApiContainer responseContainer = null, Dictionary<string, object> requestParams = null)
		{
			foreach (object offlineQuery in offlineQueries)
			{
				Dictionary<string, object> dictionary = offlineQuery as Dictionary<string, object>;
				if (dictionary["url"].ToString() == endpoint)
				{
					object result = dictionary["result"];
					string s = Json.Encode(result);
					byte[] data = Encoding.UTF8.GetBytes(s);
					responseContainer.OnComplete(success: true, endpoint, 200, string.Empty, () => data, () => Json.Encode(result));
					if (!responseContainer.IsValid)
					{
						if (responseContainer.OnError != null)
						{
							responseContainer.OnError(responseContainer);
						}
					}
					else if (responseContainer.OnSuccess != null)
					{
						responseContainer.OnSuccess(responseContainer);
					}
				}
			}
			Debug.LogError((object)("Query used by application in offline mode not found - " + endpoint));
			responseContainer.Error = "query not found in offline results - " + endpoint;
			if (responseContainer.OnError != null)
			{
				responseContainer.OnError(responseContainer);
			}
		}
	}
}
