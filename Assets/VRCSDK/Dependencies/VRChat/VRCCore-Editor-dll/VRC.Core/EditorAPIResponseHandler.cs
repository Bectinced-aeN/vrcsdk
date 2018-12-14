using System;
using System.Collections.Generic;
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
				Dictionary<string, object> dictionary = null;
				if (successCallbackWithDict != null)
				{
					dictionary = (Json.Decode(dataAsText) as Dictionary<string, object>);
				}
				List<object> obj = null;
				if (successCallbackWithList != null)
				{
					obj = (Json.Decode(dataAsText) as List<object>);
				}
				if (resp.IsSuccess)
				{
					if (resp.StatusCode == 200 || resp.StatusCode == 304)
					{
						successCallbackWithResponse?.Invoke(dataAsText);
						successCallbackWithDict?.Invoke(dictionary);
						successCallbackWithList?.Invoke(obj);
					}
					else
					{
						string errorMessage = GetErrorMessage(dictionary);
						errorCallback?.Invoke(errorMessage);
						Logger.LogError("[" + requestId + "] Response error - server returned status code " + resp.StatusCode + ", message - " + errorMessage);
					}
				}
				else
				{
					Logger.Log("[" + requestId + "] Response: " + dataAsText, DebugLevel.API);
					Logger.LogWarning(string.Format("[" + requestId + "] Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}", resp.StatusCode, resp.Message, resp.DataAsText));
					string errorMessage2 = GetErrorMessage(dictionary);
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
