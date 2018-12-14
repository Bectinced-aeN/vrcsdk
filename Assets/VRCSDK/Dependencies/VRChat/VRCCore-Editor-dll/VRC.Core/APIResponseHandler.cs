using System;
using System.Collections.Generic;
using VRC.Core.BestHTTP;

namespace VRC.Core
{
	internal abstract class APIResponseHandler
	{
		public virtual void HandleReponse(HTTPRequest req, HTTPResponse resp, int requestId, Action<string> successCallbackWithResponse = null, Action<Dictionary<string, object>> successCallbackWithDict = null, Action<List<object>> successCallbackWithList = null, Action<string> errorCallback = null, int retryCount = 0)
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
	}
}
