using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using VRC.Core.BestHTTP;
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

		public ModerationType moderationType;

		public string moderatorUserId;

		public string moderatorDisplayName;

		public string targetUserId;

		public string targetDisplayName;

		public string reason;

		public Dictionary<string, object> details;

		public DateTime created;

		public DateTime expires;

		public string worldId;

		public string instanceId;

		public void Init(Dictionary<string, object> jsonObject)
		{
			mId = (jsonObject["id"] as string);
			if (jsonObject.ContainsKey("type"))
			{
				string value = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase((jsonObject["type"] as string).ToLower());
				moderationType = ModerationType.None;
				try
				{
					moderationType = (ModerationType)(int)Enum.Parse(typeof(ModerationType), value, ignoreCase: true);
				}
				catch (Exception)
				{
					moderationType = ModerationType.None;
				}
			}
			if (jsonObject.ContainsKey("moderatorUserId"))
			{
				moderatorUserId = (jsonObject["moderatorUserId"] as string);
			}
			if (jsonObject.ContainsKey("moderatorDisplayName"))
			{
				moderatorDisplayName = (jsonObject["moderatorDisplayName"] as string);
			}
			if (jsonObject.ContainsKey("targetUserId"))
			{
				targetUserId = (jsonObject["targetUserId"] as string);
			}
			if (jsonObject.ContainsKey("targetDisplayName"))
			{
				targetDisplayName = (jsonObject["targetDisplayName"] as string);
			}
			if (jsonObject.ContainsKey("reason"))
			{
				reason = (jsonObject["reason"] as string);
			}
			if (jsonObject.ContainsKey("details"))
			{
				details = (Json.Decode(jsonObject["details"] as string) as Dictionary<string, object>);
			}
			if (jsonObject.ContainsKey("created"))
			{
				created = DateTime.Parse(jsonObject["created"] as string);
			}
			if (jsonObject.ContainsKey("expires"))
			{
				expires = DateTime.Parse(jsonObject["expires"] as string);
			}
			if (jsonObject.ContainsKey("worldId"))
			{
				worldId = (jsonObject["worldId"] as string);
			}
			if (jsonObject.ContainsKey("instanceId"))
			{
				instanceId = (jsonObject["instanceId"] as string);
			}
		}

		public static void SendModeration(string targetUserId, ModerationType mType, string reason, ModerationTimeRange expires, string worldId = "", string worldInstanceId = "", Action successCallback = null, Action<string> errorCallback = null)
		{
			string value = mType.ToString().ToLower();
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["type"] = value;
			dictionary["reason"] = reason;
			string value2 = ModerationTimeRangeToString(expires);
			if (!string.IsNullOrEmpty(value2))
			{
				dictionary["expires"] = value2;
			}
			if (!string.IsNullOrEmpty(worldId))
			{
				dictionary["worldId"] = worldId;
			}
			if (!string.IsNullOrEmpty(worldInstanceId))
			{
				dictionary["instanceId"] = worldInstanceId;
			}
			ApiModel.SendPostRequest("user/" + targetUserId + "/moderations", dictionary, delegate
			{
				if (successCallback != null)
				{
					successCallback();
				}
			}, delegate(string obj)
			{
				if (errorCallback != null)
				{
					errorCallback(obj);
				}
			});
		}

		public static void SendVoteKick(string targetUserId, string worldId = "", string worldInstanceId = "", Dictionary<string, object> details = null, Action<ApiModeration> successCallback = null, Action<string> errorCallback = null)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (!string.IsNullOrEmpty(worldId))
			{
				dictionary["worldId"] = worldId;
			}
			if (!string.IsNullOrEmpty(worldInstanceId))
			{
				dictionary["instanceId"] = worldInstanceId;
			}
			if (details != null)
			{
				dictionary["details"] = Json.Encode(details);
			}
			ApiModel.SendPostRequest("user/" + targetUserId + "/votekick", dictionary, delegate(Dictionary<string, object> obj)
			{
				ApiModeration apiModeration = new ApiModeration();
				apiModeration.Init(obj);
				if (successCallback != null)
				{
					successCallback(apiModeration);
				}
			}, delegate(string obj)
			{
				if (errorCallback != null)
				{
					errorCallback(obj);
				}
			});
		}

		public static void DeleteModeration(string targetUserId, string moderationId, Action successCallback, Action<string> errorCallback)
		{
			ApiModel.SendRequest("/user/" + targetUserId + "/moderations/" + moderationId, HTTPMethods.Delete, (Dictionary<string, string>)null, (Action<Dictionary<string, object>>)delegate
			{
				if (successCallback != null)
				{
					successCallback();
				}
			}, (Action<string>)delegate(string obj)
			{
				if (errorCallback != null)
				{
					errorCallback(obj);
				}
			}, needsAPIKey: true, authenticationRequired: true, -1f);
		}

		public static void LocalFetchAll(Action<List<ApiModeration>> successCallback, Action<string> errorCallback)
		{
			ApiModel.SendGetRequest("auth/user/moderations", delegate(List<object> objects)
			{
				List<ApiModeration> list = new List<ApiModeration>();
				if (objects != null)
				{
					foreach (object @object in objects)
					{
						Dictionary<string, object> jsonObject = @object as Dictionary<string, object>;
						ApiModeration apiModeration = new ApiModeration();
						apiModeration.Init(jsonObject);
						list.Add(apiModeration);
					}
				}
				if (successCallback != null)
				{
					successCallback(list);
				}
			}, delegate(string message)
			{
				Debug.LogError((object)("Could not fetch moderations with error - " + message));
				errorCallback(message);
			});
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
				return string.Empty;
			}
		}
	}
}
