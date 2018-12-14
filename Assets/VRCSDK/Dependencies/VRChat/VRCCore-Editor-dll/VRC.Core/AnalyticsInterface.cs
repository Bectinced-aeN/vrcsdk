using AmplitudeSDKWrapper;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace VRC.Core
{
	public class AnalyticsInterface
	{
		public static void Initialize(string apiKey, string buildVersion)
		{
			AmplitudeWrapper.Initialize(apiKey, buildVersion);
		}

		public static void Send(IEnumerable<ApiAnalyticEvent.EventInfo> events)
		{
			ApiAnalyticEvent.Send(events);
		}

		public static void Send(ApiAnalyticEvent.EventType type)
		{
			Send(type, (string)null, (Vector3?)null, (Action<bool>)null);
		}

		public static void Send(ApiAnalyticEvent.EventType eventType, string detail, Vector3? location = default(Vector3?), Action<bool> completeCallback = null)
		{
			Send(new ApiAnalyticEvent.EventInfo[1]
			{
				new ApiAnalyticEvent.EventInfo
				{
					eventType = eventType,
					location = location,
					parameters = (string.IsNullOrEmpty(detail) ? new Dictionary<string, object>() : new Dictionary<string, object>
					{
						{
							"parameter",
							detail
						}
					}),
					completeCallback = completeCallback
				}
			});
		}

		public static void Send(ApiAnalyticEvent.EventType eventType, Dictionary<string, object> details, Vector3? location = default(Vector3?), Action<bool> completeCallback = null)
		{
			Send(new ApiAnalyticEvent.EventInfo[1]
			{
				new ApiAnalyticEvent.EventInfo
				{
					eventType = eventType,
					location = location,
					parameters = details,
					completeCallback = completeCallback
				}
			});
		}

		public static void Send(string eventType)
		{
			Send(eventType, null);
		}

		public static void Send(string eventType, Dictionary<string, object> eventProperties)
		{
			CheckInstance();
			AmplitudeWrapper.Instance.LogEvent(eventType, eventProperties);
		}

		public static void SetUserId(string userId)
		{
			CheckInstance();
			AmplitudeWrapper.Instance.SetUserId(userId);
		}

		public static void SetUserProperties(Dictionary<string, object> userProps, bool replace = false)
		{
			CheckInstance();
			AmplitudeWrapper.Instance.SetUserProperties(userProps, replace);
		}

		public static void OnApplicationQuit()
		{
			CheckInstance();
			AmplitudeWrapper.Instance.OnApplicationQuit();
		}

		public static void OnApplicationFocus(bool isFocused)
		{
			CheckInstance();
			AmplitudeWrapper.Instance.OnApplicationFocus(isFocused);
		}

		private static void CheckInstance()
		{
			if (AmplitudeWrapper.Instance == null)
			{
				throw new Exception("AnalyticsInterface not initialized! Call AnalyticsInterface.Initialize before sending events");
			}
		}
	}
}
