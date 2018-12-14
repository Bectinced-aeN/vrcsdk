using System;
using System.Collections.Generic;
using UnityEngine;
using VRC.Core.BestHTTP;
using VRC.Core.BestHTTP.JSON;

namespace VRC.Core
{
	internal class EditorAPIResponseHandler : APIResponseHandler
	{
		public override void HandleReponse(HTTPRequest req, HTTPResponse resp, int requestId, Action<string> successCallbackWithResponse = null, Action<Dictionary<string, object>> successCallbackWithDict = null, Action<List<object>> successCallbackWithList = null, Action<string> errorCallback = null, int retryCount = 0)
		{
			string empty = string.Empty;
			switch (req.State)
			{
			case HTTPRequestStates.Finished:
			{
				string dataAsText = resp.DataAsText;
				Dictionary<string, object> obj = null;
				if (successCallbackWithDict != null)
				{
					obj = (Json.Decode(dataAsText) as Dictionary<string, object>);
				}
				List<object> obj2 = null;
				if (successCallbackWithList != null)
				{
					obj2 = (Json.Decode(dataAsText) as List<object>);
				}
				if (resp.IsSuccess)
				{
					if (resp.StatusCode == 200 || resp.StatusCode == 304)
					{
						successCallbackWithResponse?.Invoke(dataAsText);
						successCallbackWithDict?.Invoke(obj);
						successCallbackWithList?.Invoke(obj2);
					}
					else
					{
						obj = (Json.Decode(dataAsText) as Dictionary<string, object>);
						string errorMessage = GetErrorMessage(obj);
						errorCallback?.Invoke(errorMessage);
						Debug.LogError((object)("[" + requestId + "] Response error - server returned status code " + resp.StatusCode + ", message - " + errorMessage));
					}
				}
				else
				{
					Debug.Log((object)string.Format("<color=red>[" + requestId + "] Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2} </color>", resp.StatusCode, resp.Message, resp.DataAsText));
					obj = (Json.Decode(dataAsText) as Dictionary<string, object>);
					string errorMessage2 = GetErrorMessage(obj);
					errorCallback?.Invoke(errorMessage2);
				}
				break;
			}
			case HTTPRequestStates.Error:
			{
				empty = ((req.Exception == null) ? "No Exception" : req.Exception.Message);
				string text = (req.Exception == null) ? "No Exception" : (req.Exception.Message + "\n" + req.Exception.StackTrace);
				Logger.Log("[" + requestId + "] Request Finished with Error! " + text);
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
	}
}
