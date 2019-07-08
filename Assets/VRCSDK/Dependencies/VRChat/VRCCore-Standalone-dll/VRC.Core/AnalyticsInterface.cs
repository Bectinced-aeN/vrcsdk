using AmplitudeSDKWrapper;
using System;
using System.Collections.Generic;

namespace VRC.Core
{
	public static class AnalyticsInterface
	{
		public static void Initialize(string apiKey)
		{
			AmplitudeWrapper.Initialize(apiKey);
		}

		public static void SetBuildVersion(string buildVersion)
		{
			CheckInstance();
			AmplitudeWrapper.Instance.SetBuildVersion(buildVersion);
		}

		public static void Send(string eventType)
		{
			Send(eventType, null);
		}

		public static void Send(string eventType, Dictionary<string, object> eventProperties)
		{
			Send(eventType, eventProperties, AnalyticsEventOptions.None);
		}

		public static void Send(string eventType, Dictionary<string, object> eventProperties, AnalyticsEventOptions options)
		{
			CheckInstance();
			AmplitudeWrapper.Instance.LogEvent(eventType, eventProperties, options);
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
