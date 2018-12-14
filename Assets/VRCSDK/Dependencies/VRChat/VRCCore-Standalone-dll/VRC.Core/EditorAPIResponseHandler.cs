using System;
using System.Collections.Generic;
using UnityEngine;
using VRC.Core.BestHTTP;
using VRC.Core.BestHTTP.JSON;

namespace VRC.Core
{
	internal class EditorAPIResponseHandler : APIResponseHandler
	{
		public override void HandleReponse(HTTPRequest req, HTTPResponse resp, int requestId, Action<string> successCallbackWithResponse = null, Action<Dictionary<string, object>> successCallbackWithDict = null, Action<List<object>> successCallbackWithList = null, Action<string> errorCallback = null, int retryCount = 0, float cacheLifetime = -1f)
		{
			string empty = string.Empty;
			switch (req.State)
			{
			case HTTPRequestStates.Finished:
			{
				string dataAsText = resp.DataAsText;
				if (resp.IsSuccess)
				{
					if (resp.StatusCode == 200 || resp.StatusCode == 304)
					{
						APIResponseHandler.CacheResponse(req.Uri.PathAndQuery, cacheLifetime, dataAsText);
						HandleSuccessReponse(dataAsText, successCallbackWithResponse, successCallbackWithDict, successCallbackWithList);
					}
					else
					{
						Dictionary<string, object> dictionary = Json.Decode(dataAsText) as Dictionary<string, object>;
						string text = (dictionary == null) ? resp.Message : GetErrorMessage(dictionary);
						errorCallback?.Invoke(text);
						Debug.LogError((object)("[" + requestId + "] Response error - server returned status code " + resp.StatusCode + ", message - " + text));
					}
				}
				else
				{
					Debug.Log((object)string.Format("<color=red>[" + requestId + "] Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2} </color>", resp.StatusCode, resp.Message, resp.DataAsText));
					Dictionary<string, object> dictionary2 = Json.Decode(dataAsText) as Dictionary<string, object>;
					string obj = (dictionary2 == null) ? resp.Message : GetErrorMessage(dictionary2);
					errorCallback?.Invoke(obj);
				}
				break;
			}
			case HTTPRequestStates.Error:
			{
				empty = ((req.Exception == null) ? "No Exception" : req.Exception.Message);
				string text2 = (req.Exception == null) ? "No Exception" : (req.Exception.Message + "\n" + req.Exception.StackTrace);
				Logger.Log("[" + requestId + "] Request Finished with Error! " + text2);
				RetryRequest(requestId, req, successCallbackWithResponse, successCallbackWithDict, successCallbackWithList, errorCallback, empty, --retryCount);
				break;
			}
			case HTTPRequestStates.Aborted:
				empty = "Request Aborted!";
				Logger.Log("[" + requestId + "] " + empty);
				errorCallback?.Invoke(empty);
				break;
			case HTTPRequestStates.ConnectionTimedOut:
				empty = "Connection Timed Out!";
				Logger.Log("[" + requestId + "] " + empty);
				RetryRequest(requestId, req, successCallbackWithResponse, successCallbackWithDict, successCallbackWithList, errorCallback, empty, --retryCount);
				break;
			case HTTPRequestStates.TimedOut:
				empty = "Processing the request Timed Out!";
				Logger.Log("[" + requestId + "] " + empty);
				RetryRequest(requestId, req, successCallbackWithResponse, successCallbackWithDict, successCallbackWithList, errorCallback, empty, --retryCount);
				break;
			}
		}

		public override void HandleSuccessReponse(string jsonResponse, Action<string> successCallbackWithResponse = null, Action<Dictionary<string, object>> successCallbackWithDict = null, Action<List<object>> successCallbackWithList = null)
		{
			Dictionary<string, object> obj = null;
			if (successCallbackWithDict != null)
			{
				obj = (Json.Decode(jsonResponse) as Dictionary<string, object>);
			}
			List<object> obj2 = null;
			if (successCallbackWithList != null)
			{
				obj2 = (Json.Decode(jsonResponse) as List<object>);
			}
			successCallbackWithResponse?.Invoke(jsonResponse);
			successCallbackWithDict?.Invoke(obj);
			successCallbackWithList?.Invoke(obj2);
		}
	}
}
