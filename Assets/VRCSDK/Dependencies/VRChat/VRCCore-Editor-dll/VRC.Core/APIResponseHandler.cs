using System.Text;
using UnityEngine;
using VRC.Core.BestHTTP;
using VRC.Core.BestHTTP.Caching;

namespace VRC.Core
{
	internal static class APIResponseHandler
	{
		public static void HandleReponse(int requestId, HTTPRequest req, HTTPResponse resp, ApiContainer responseContainer, int retryCount = 0, bool useCache = false)
		{
			string text = (resp != null) ? resp.StatusCode.ToString() : "null";
			string text2 = (req != null) ? req.MethodType.ToString() : "null";
			string text3 = (req != null) ? req.Uri.ToString() : "null";
			string text4 = (resp != null) ? resp.DataAsText : "null";
			string text5 = (req.RawData != null) ? Encoding.UTF8.GetString(req.RawData) : string.Empty;
			if (req.State == HTTPRequestStates.Finished && (resp.StatusCode < 200 || resp.StatusCode >= 400))
			{
				req.State = HTTPRequestStates.Error;
			}
			switch (req.State)
			{
			case HTTPRequestStates.Finished:
				responseContainer.OnComplete(resp.IsSuccess, req.Uri.PathAndQuery, resp.StatusCode, resp.Message, () => resp.Data, () => resp.DataAsText);
				if (!responseContainer.IsValid)
				{
					RetryRequest(requestId, req, resp, responseContainer, --retryCount, useCache, string.Empty);
				}
				else
				{
					if (useCache && req.MethodType == HTTPMethods.Get)
					{
						ApiCache.CacheResponse(req.Uri.PathAndQuery, resp.Data);
					}
					if (responseContainer.OnSuccess != null)
					{
						responseContainer.OnSuccess(responseContainer);
					}
				}
				break;
			default:
			{
				string text6 = (req.Exception == null) ? "No Exception" : req.Exception.Message;
				Debug.LogErrorFormat("[{0}, {1}, {2}, {3}] Request Finished with Error!\n{4}\n{5}\n{6}\n{7}", new object[8]
				{
					requestId,
					text,
					text2,
					retryCount,
					text3,
					text5,
					text6,
					text4
				});
				responseContainer.OnComplete(success: false, req.Uri.PathAndQuery, resp.StatusCode, resp.Message, () => resp.Data, () => resp.DataAsText);
				if (resp.StatusCode >= 400 && resp.StatusCode < 500)
				{
					retryCount = 0;
				}
				RetryRequest(requestId, req, resp, responseContainer, --retryCount, useCache, string.Empty);
				break;
			}
			case HTTPRequestStates.Aborted:
				Debug.LogErrorFormat("[{0}, {1}, {2}, {3}] Request Request Aborted!\n{4}\n{5}", new object[6]
				{
					requestId,
					text,
					text2,
					retryCount,
					text3,
					text4
				});
				responseContainer.Error = "The request was cancelled.";
				if (responseContainer.OnError != null)
				{
					responseContainer.OnError(responseContainer);
				}
				break;
			case HTTPRequestStates.ConnectionTimedOut:
				Debug.LogErrorFormat("[{0}, {1}, {2}, {3}] Connection Timed Out!\n{4}\n{4}", new object[6]
				{
					requestId,
					text,
					text2,
					retryCount,
					text3,
					text4
				});
				RetryRequest(requestId, req, resp, responseContainer, --retryCount, useCache, "The request timed out.");
				break;
			case HTTPRequestStates.TimedOut:
				Debug.LogErrorFormat("[{0}, {1}, {2}, {3}] Processing the request Timed Out!\n{4}\n{5}", new object[6]
				{
					requestId,
					text,
					text2,
					retryCount,
					text3,
					text4
				});
				RetryRequest(requestId, req, resp, responseContainer, --retryCount, useCache, "The request timed out.");
				break;
			}
		}

		private static void RetryRequest(int requestId, HTTPRequest request, HTTPResponse resp, ApiContainer responseContainer, int retryCount, bool useCache, string errorMessage = "")
		{
			HTTPCacheService.DeleteEntity(request.Uri);
			string text = (resp != null) ? resp.StatusCode.ToString() : "No Status Code";
			if (retryCount > 0)
			{
				Debug.LogFormat("<color=orange>[{0}, {1}, {2}, {3}]</color> Retrying request, because: {4}\n{5}", new object[6]
				{
					requestId,
					text,
					request.MethodType.ToString(),
					retryCount,
					responseContainer.Error,
					request.Uri
				});
				request.Callback = delegate(HTTPRequest originalRequest, HTTPResponse response)
				{
					HandleReponse(requestId, originalRequest, response, responseContainer, retryCount, useCache);
				};
				request.Send();
			}
			else
			{
				if (!string.IsNullOrEmpty(errorMessage))
				{
					responseContainer.Error = errorMessage;
				}
				Debug.LogFormat("<color=red>[{0}, {1}, {2}, {3}]</color> Abandoning request, because: {4}\n{5}", new object[6]
				{
					requestId,
					text,
					request.MethodType.ToString(),
					retryCount,
					responseContainer.Error,
					request.Uri
				});
				if (responseContainer.OnError != null)
				{
					responseContainer.OnError(responseContainer);
				}
			}
		}
	}
}
