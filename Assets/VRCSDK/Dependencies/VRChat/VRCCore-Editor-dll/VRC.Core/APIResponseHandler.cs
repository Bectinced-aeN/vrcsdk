using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VRC.Core.BestHTTP;
using VRC.Core.BestHTTP.Caching;
using VRC.Core.BestHTTP.Cookies;

namespace VRC.Core
{
	internal static class APIResponseHandler
	{
		public static void HandleReponse(int requestId, HTTPRequest req, HTTPResponse resp, ApiContainer responseContainer, int retryCount = 0, bool useCache = false)
		{
			if (responseContainer == null)
			{
				Logger.LogError("ResponseContainer was null!");
			}
			else if (req == null)
			{
				responseContainer.Error = "The request was null.";
				if (responseContainer.OnError != null)
				{
					responseContainer.OnError(responseContainer);
				}
			}
			else if (resp == null)
			{
				responseContainer.Error = "The response was null.";
				if (responseContainer.OnError != null)
				{
					responseContainer.OnError(responseContainer);
				}
			}
			else
			{
				string text3;
				string text2;
				string text;
				string empty;
				string text4 = text3 = (text2 = (text = (empty = string.Empty)));
				try
				{
					text4 = ((resp != null) ? resp.StatusCode.ToString() : "null");
					text3 = ((req != null) ? req.MethodType.ToString() : "null");
					text2 = ((req != null) ? req.Uri.ToString() : "null");
					text = ((resp != null) ? resp.DataAsText.Replace("{", "{{").Replace("}", "}}") : "null");
					empty = ((req.RawData != null) ? Encoding.UTF8.GetString(req.RawData).Replace("{", "{{").Replace("}", "}}") : string.Empty);
				}
				catch (Exception ex)
				{
					Logger.LogErrorFormat("Exception in reading response: {0}\n{1}", new object[2]
					{
						ex.Message,
						ex.StackTrace
					});
					responseContainer.Error = "Bad response data.";
					if (responseContainer.OnError != null)
					{
						responseContainer.OnError(responseContainer);
					}
					return;
					IL_01ca:;
				}
				if (req.State == HTTPRequestStates.Finished && (resp == null || resp.StatusCode < 200 || resp.StatusCode >= 400))
				{
					req.State = HTTPRequestStates.Error;
				}
				responseContainer.Cookies = ((resp != null && resp.Cookies != null) ? (from c in resp.Cookies
				where c != null
				select new KeyValuePair<string, string>(c.Name, c.Value)).ToDictionary((KeyValuePair<string, string> t) => t.Key, (KeyValuePair<string, string> t) => t.Value) : new Dictionary<string, string>());
				try
				{
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
						string text5 = (req.Exception == null) ? "No Exception" : req.Exception.Message;
						Logger.LogErrorFormat(DebugLevel.API, "[{0}, {1}, {2}, {3}] Request Finished with Error!\n{4}\n{5}\n{6}\n{7}", requestId, text4, text3, retryCount, text2, empty, text5, text);
						responseContainer.OnComplete(success: false, req.Uri.PathAndQuery, resp.StatusCode, resp.Message, () => resp.Data, () => resp.DataAsText);
						if (resp.StatusCode >= 400 && resp.StatusCode < 500)
						{
							retryCount = 0;
						}
						RetryRequest(requestId, req, resp, responseContainer, --retryCount, useCache, string.Empty);
						break;
					}
					case HTTPRequestStates.Aborted:
						Logger.LogErrorFormat(DebugLevel.API, "[{0}, {1}, {2}, {3}] Request Request Aborted!\n{4}\n{5}", requestId, text4, text3, retryCount, text2, text);
						responseContainer.Error = "The request was cancelled.";
						if (responseContainer.OnError != null)
						{
							responseContainer.OnError(responseContainer);
						}
						break;
					case HTTPRequestStates.ConnectionTimedOut:
						Logger.LogErrorFormat(DebugLevel.API, "[{0}, {1}, {2}, {3}] Connection Timed Out!\n{4}\n{4}", requestId, text4, text3, retryCount, text2, text);
						RetryRequest(requestId, req, resp, responseContainer, --retryCount, useCache, "The request timed out.");
						break;
					case HTTPRequestStates.TimedOut:
						Logger.LogErrorFormat(DebugLevel.API, "[{0}, {1}, {2}, {3}] Processing the request Timed Out!\n{4}\n{5}", requestId, text4, text3, retryCount, text2, text);
						RetryRequest(requestId, req, resp, responseContainer, --retryCount, useCache, "The request timed out.");
						break;
					}
				}
				catch (Exception ex2)
				{
					Logger.LogErrorFormat("Exception handling {0}: {1}\n{2}", req.State, ex2.Message, ex2.StackTrace);
				}
			}
		}

		private static void RetryRequest(int requestId, HTTPRequest request, HTTPResponse resp, ApiContainer responseContainer, int retryCount, bool useCache, string errorMessage = "")
		{
			HTTPCacheService.DeleteEntity(request.Uri);
			string text = (resp != null) ? resp.StatusCode.ToString() : "No Status Code";
			if (retryCount > 0)
			{
				Debug.LogFormat("[{0}, {1}, {2}, {3}] Retrying request, because: {4}\n{5}", new object[6]
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
				Debug.LogFormat("[{0}, {1}, {2}, {3}] Abandoning request, because: {4}\n{5}", new object[6]
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
