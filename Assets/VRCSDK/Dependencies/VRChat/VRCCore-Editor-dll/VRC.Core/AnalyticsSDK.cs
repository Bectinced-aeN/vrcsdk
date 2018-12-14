using System.Collections.Generic;

namespace VRC.Core
{
	public class AnalyticsSDK
	{
		private const string API_KEY = "05932e9b759849f1c921866f92047b03";

		private static bool _isInitialized;

		private static string _lastLoggedInUserId = string.Empty;

		public static void Initialize(string sdkVersion)
		{
			if (!_isInitialized)
			{
				AnalyticsInterface.Initialize("05932e9b759849f1c921866f92047b03");
				AnalyticsInterface.SetBuildVersion(sdkVersion);
				_isInitialized = true;
			}
		}

		public static void LoggedInUserChanged()
		{
			LoggedInUserChanged(APIUser.CurrentUser);
		}

		public static void LoggedInUserChanged(APIUser user)
		{
			string text = (user == null) ? string.Empty : user.id;
			if (!(text == _lastLoggedInUserId))
			{
				_lastLoggedInUserId = text;
				CheckInit();
				if (user != null)
				{
					AnalyticsInterface.SetUserId(user.id);
					AnalyticsInterface.SetUserProperties(null);
					AnalyticsInterface.Send("SDK_LoginSuccess", null, AnalyticsEventOptions.MarkOutOfSession);
				}
				else
				{
					AnalyticsInterface.SetUserId(null);
					AnalyticsInterface.SetUserProperties(null);
				}
			}
		}

		public static void AvatarUploaded(ApiModel model, bool isUpdate)
		{
			CheckInit();
			string value = (model == null) ? "null" : model.id;
			AnalyticsInterface.Send((!isUpdate) ? "SDK_CreateAvatar" : "SDK_UpdateAvatar", new Dictionary<string, object>
			{
				{
					"modelId",
					value
				}
			}, AnalyticsEventOptions.MarkOutOfSession);
		}

		public static void WorldUploaded(ApiModel model, bool isUpdate)
		{
			CheckInit();
			string value = (model == null) ? "null" : model.id;
			AnalyticsInterface.Send((!isUpdate) ? "SDK_CreateWorld" : "SDK_UpdateWorld", new Dictionary<string, object>
			{
				{
					"modelId",
					value
				}
			}, AnalyticsEventOptions.MarkOutOfSession);
		}

		private static void CheckInit()
		{
			if (!_isInitialized)
			{
				Initialize(string.Empty);
			}
		}
	}
}
