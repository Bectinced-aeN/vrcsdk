using System;
using System.Collections.Generic;
using UnityEngine;
using VRC.Core.BestHTTP;

namespace VRC.Core
{
	internal abstract class APIResponseHandler
	{
		private static Dictionary<int, ApiCachedResponse> apiResponseCache = new Dictionary<int, ApiCachedResponse>();

		public virtual void HandleReponse(HTTPRequest req, HTTPResponse resp, int requestId, Action<string> successCallbackWithResponse = null, Action<Dictionary<string, object>> successCallbackWithDict = null, Action<List<object>> successCallbackWithList = null, Action<string> errorCallback = null, int retryCount = 0, float cacheLifetime = -1f)
		{
		}

		public virtual void HandleSuccessReponse(string jsonResponse, Action<string> successCallbackWithResponse = null, Action<Dictionary<string, object>> successCallbackWithDict = null, Action<List<object>> successCallbackWithList = null)
		{
		}

		protected void RetryRequest(int requestId, HTTPRequest request, Action<string> successCallbackWithResponse, Action<Dictionary<string, object>> successCallbackWithDict, Action<List<object>> successCallbackWithList, Action<string> errorCallback, string errorMessage, int retryCount)
		{
			if (retryCount > 0)
			{
				Logger.Log("[" + requestId + "] Retrying request (" + retryCount + ")");
				request.Callback = delegate(HTTPRequest originalRequest, HTTPResponse response)
				{
					APIResponseHandler aPIResponseHandler = new EditorAPIResponseHandler();
					aPIResponseHandler.HandleReponse(originalRequest, response, requestId, successCallbackWithResponse, successCallbackWithDict, successCallbackWithList, errorCallback, retryCount);
				};
				request.Send();
			}
			else
			{
				Logger.Log("[" + requestId + "] Out of retries, reporting error");
				if (errorCallback != null)
				{
					errorCallback(errorMessage);
				}
			}
		}

		protected string GetErrorMessage(Dictionary<string, object> responseDict)
		{
			string result = "Unexpected error!";
			if (responseDict != null && responseDict.ContainsKey("error"))
			{
				Dictionary<string, object> dictionary = (Dictionary<string, object>)responseDict["error"];
				if (dictionary.ContainsKey("message"))
				{
					result = (string)dictionary["message"];
				}
			}
			return result;
		}

		public static ApiCachedResponse GetOrClearCachedResponse(string apiRequestPathAndQuery, float cacheLifetime)
		{
			int hashCode = apiRequestPathAndQuery.GetHashCode();
			if (apiResponseCache.TryGetValue(hashCode, out ApiCachedResponse value))
			{
				if (Time.get_realtimeSinceStartup() - value.Timestamp > Mathf.Min(value.Lifetime, cacheLifetime))
				{
					apiResponseCache.Remove(hashCode);
					return null;
				}
				return value;
			}
			return null;
		}

		public static void CacheResponse(string apiRequestPathAndQuery, float cacheLifetime, string data)
		{
			if (!(cacheLifetime <= 0f))
			{
				int hashCode = apiRequestPathAndQuery.GetHashCode();
				apiResponseCache[hashCode] = new ApiCachedResponse(data, Time.get_realtimeSinceStartup(), cacheLifetime);
			}
		}

		public static void ClearCache()
		{
			apiResponseCache.Clear();
		}

		public static void CleanExpiredCache()
		{
			List<int> list = new List<int>();
			foreach (KeyValuePair<int, ApiCachedResponse> item in apiResponseCache)
			{
				if (Time.get_realtimeSinceStartup() - item.Value.Timestamp > item.Value.Lifetime)
				{
					list.Add(item.Key);
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				apiResponseCache.Remove(list[i]);
			}
		}
	}
}
