using System;
using System.Collections.Generic;
using UnityEngine;
using VRC.Core.BestHTTP.JSON;

namespace VRC.Core
{
	public class ApiModeration : ApiModel
	{
		public enum ModerationType
		{
			None,
			Warn,
			Kick,
			Ban,
			BanPublicOnly
		}

		public enum ModerationTimeRange
		{
			None,
			OneHour,
			OneDay
		}

		public const float ListCacheTime = 120f;

		public ModerationTimeRange? expiresRange;

		[ApiField(Required = false, Name = "type")]
		public ModerationType moderationType
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public string moderatorUserId
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public string moderatorDisplayName
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public string targetUserId
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public string targetDisplayName
		{
			get;
			set;
		}

		[ApiField(Required = false, Name = "reason")]
		public string reasonMessage
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public Dictionary<string, object> details
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public DateTime created
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public DateTime expires
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public string worldId
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public string instanceId
		{
			get;
			set;
		}

		public ApiModeration(string targetUserId, string moderationSubRequest = "moderations")
			: base("user/" + targetUserId + "/" + moderationSubRequest)
		{
		}

		public ApiModeration()
			: base(null)
		{
		}

		public override bool ShouldCache()
		{
			return false;
		}

		protected override bool ReadField(string fieldName, ref object data)
		{
			switch (fieldName)
			{
			case "type":
				data = moderationType.ToString().ToLower();
				return true;
			case "expires":
				if (expiresRange.HasValue)
				{
					data = ModerationTimeRangeToString(expiresRange.Value);
					return !string.IsNullOrEmpty(data.ToString());
				}
				return false;
			default:
				return base.ReadField(fieldName, ref data);
			}
		}

		protected override bool WriteField(string fieldName, object data)
		{
			switch (fieldName)
			{
			case "details":
				details = (Json.Decode(data as string) as Dictionary<string, object>);
				return true;
			default:
				return base.WriteField(fieldName, data);
			}
		}

		protected override ApiContainer MakeModelContainer(Action<ApiContainer> onSuccess = null, Action<ApiContainer> onFailure = null)
		{
			ApiContainer apiContainer = new ApiContainer();
			apiContainer.OnSuccess = onSuccess;
			apiContainer.OnError = onFailure;
			return apiContainer;
		}

		public static void SendModeration(string targetUserId, ModerationType mType, string reason, ModerationTimeRange expires, string worldId = "", string worldInstanceId = "", Action<ApiModelContainer<ApiModeration>> successCallback = null, Action<ApiModelContainer<ApiModeration>> errorCallback = null)
		{
			ApiModeration apiModeration = new ApiModeration(targetUserId);
			apiModeration.moderationType = mType;
			apiModeration.reasonMessage = reason;
			apiModeration.worldId = worldId;
			apiModeration.instanceId = worldInstanceId;
			apiModeration.expiresRange = expires;
			ApiModeration apiModeration2 = apiModeration;
			apiModeration2.Save(delegate(ApiContainer c)
			{
				if (successCallback != null)
				{
					successCallback(c as ApiModelContainer<ApiModeration>);
				}
			}, delegate(ApiContainer c)
			{
				if (errorCallback != null)
				{
					errorCallback(c as ApiModelContainer<ApiModeration>);
				}
			});
		}

		public static void SendVoteKick(string targetUserId, string worldId = "", string worldInstanceId = "", Dictionary<string, object> details = null, Action successCallback = null, Action<string> errorCallback = null)
		{
			ApiModeration apiModeration = new ApiModeration(targetUserId, "votekick");
			apiModeration.details = details;
			apiModeration.worldId = worldId;
			apiModeration.instanceId = worldInstanceId;
			ApiModeration apiModeration2 = apiModeration;
			apiModeration2.Save(delegate
			{
				if (successCallback != null)
				{
					successCallback();
				}
			}, delegate(ApiContainer c)
			{
				if (errorCallback != null)
				{
					errorCallback(c.Error);
				}
			});
		}

		public static void DeleteModeration(string targetUserId, string moderationId, Action successCallback, Action<string> errorCallback)
		{
			ApiModeration apiModeration = new ApiModeration(targetUserId, "moderations/" + moderationId);
			apiModeration.Delete(delegate
			{
				if (successCallback != null)
				{
					successCallback();
				}
			}, delegate(ApiContainer c)
			{
				if (errorCallback != null)
				{
					errorCallback(c.Error);
				}
			});
		}

		public static void LocalFetchAll(Action<IEnumerable<ApiModeration>> successCallback, Action<string> errorCallback)
		{
			ApiModelListContainer<ApiModeration> apiModelListContainer = new ApiModelListContainer<ApiModeration>();
			apiModelListContainer.OnSuccess = delegate(ApiContainer c)
			{
				if (successCallback != null)
				{
					successCallback((c as ApiModelListContainer<ApiModeration>).ResponseModels);
				}
			};
			apiModelListContainer.OnError = delegate(ApiContainer c)
			{
				Debug.LogError((object)("Could not fetch moderations with error - " + c.Error));
				if (errorCallback != null)
				{
					errorCallback(c.Error);
				}
			};
			ApiModelListContainer<ApiModeration> responseContainer = apiModelListContainer;
			API.SendGetRequest("auth/user/moderations", responseContainer, null, disableCache: false, 120f);
		}

		public static string ModerationTimeRangeToString(ModerationTimeRange timeRange)
		{
			switch (timeRange)
			{
			case ModerationTimeRange.None:
				return string.Empty;
			case ModerationTimeRange.OneHour:
				return "1_hour_ahead";
			case ModerationTimeRange.OneDay:
				return "1_day_ahead";
			default:
				return "<unknown_ModerationTimeRange>";
			}
		}
	}
}
