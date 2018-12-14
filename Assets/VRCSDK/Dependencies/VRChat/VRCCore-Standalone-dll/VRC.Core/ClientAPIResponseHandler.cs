using Frankfort.Threading;
using System;
using System.Collections.Generic;
using System.Threading;
using VRC.Core.BestHTTP;
using VRC.Core.BestHTTP.JSON;

namespace VRC.Core
{
	internal class ClientAPIResponseHandler : APIResponseHandler
	{
		public unsafe override void HandleReponse(HTTPRequest req, HTTPResponse resp, int requestId, Action<string> successCallbackWithResponse = null, Action<Dictionary<string, object>> successCallbackWithDict = null, Action<List<object>> successCallbackWithList = null, Action<string> errorCallback = null, int retryCount = 0)
		{
			string empty = string.Empty;
			switch (req.State)
			{
			case HTTPRequestStates.Finished:
			{
				Logger.Log(string.Format("[" + requestId + "] Request finished! Loaded from local cache: {0}, with size: {1}", req.Response.IsFromCache.ToString(), resp.Data.Length), DebugLevel.API);
				string jsonResponse = resp.DataAsText;
				Loom.StartSingleThread((ThreadStart)delegate
				{
					//IL_0125: Unknown result type (might be due to invalid IL or missing references)
					//IL_0131: Expected O, but got Unknown
					Logger.Log("[" + requestId + "] Response: " + jsonResponse, DebugLevel.API);
					Dictionary<string, object> objDict = null;
					if (successCallbackWithDict != null)
					{
						objDict = (Json.Decode(jsonResponse) as Dictionary<string, object>);
					}
					List<object> objList = null;
					if (successCallbackWithList != null)
					{
						objList = (Json.Decode(jsonResponse) as List<object>);
					}
					if (resp.IsSuccess)
					{
						if (resp.StatusCode == 200 || resp.StatusCode == 304)
						{
							Logger.Log("[" + requestId + "] Successful response.", DebugLevel.API);
							_003CHandleReponse_003Ec__AnonStorey24._003CHandleReponse_003Ec__AnonStorey25 _003CHandleReponse_003Ec__AnonStorey;
							Loom.DispatchToMainThread(new ThreadDispatchDelegate((object)_003CHandleReponse_003Ec__AnonStorey, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), false, true);
						}
						else
						{
							string errorMessage = GetErrorMessage(objDict);
							if (errorCallback != null)
							{
								errorCallback(errorMessage);
							}
							Logger.LogError("[" + requestId + "] Response error - " + errorMessage);
						}
					}
					else
					{
						Logger.Log("[" + requestId + "] Response: " + jsonResponse, DebugLevel.API);
						Logger.LogWarning(string.Format("[" + requestId + "] Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}", resp.StatusCode, resp.Message, resp.DataAsText));
						string errorMessage2 = GetErrorMessage(objDict);
						if (errorCallback != null)
						{
							errorCallback(errorMessage2);
						}
					}
				}, ThreadPriority.Normal, true);
				break;
			}
			case HTTPRequestStates.Error:
				empty = ((req.Exception == null) ? "No Exception" : (req.Exception.Message + "\n" + req.Exception.StackTrace));
				Logger.Log("[" + requestId + "] Request Finished with Error! " + empty);
				RetryRequest(requestId, req, successCallbackWithResponse, successCallbackWithDict, successCallbackWithList, errorCallback, empty, --retryCount);
				break;
			case HTTPRequestStates.Aborted:
				empty = "Request Aborted!";
				Logger.Log("[" + requestId + "] " + empty);
				if (errorCallback != null)
				{
					errorCallback(empty);
				}
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
