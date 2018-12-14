using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using VRC.Core.BestHTTP;

namespace VRC.Core
{
	public class ApiPlayerModeration : ApiModel
	{
		public enum ModerationType
		{
			None,
			Block,
			Mute,
			Unmute
		}

		public ModerationType moderationType;

		public string moderatorUserId;

		public string moderatorDisplayName;

		public string targetUserId;

		public string targetDisplayName;

		public void Init(Dictionary<string, object> jsonObject)
		{
			mId = (jsonObject["id"] as string);
			if (jsonObject.ContainsKey("type"))
			{
				string value = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase((jsonObject["type"] as string).ToLower());
				moderationType = (ModerationType)(int)Enum.Parse(typeof(ModerationType), value);
			}
			if (jsonObject.ContainsKey("sourceUserId"))
			{
				moderatorUserId = (jsonObject["sourceUserId"] as string);
			}
			if (jsonObject.ContainsKey("sourceDisplayName"))
			{
				moderatorDisplayName = (jsonObject["sourceDisplayName"] as string);
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
		}

		public void Init(ApiPlayerModeration from)
		{
			mId = from.mId;
			moderationType = from.moderationType;
			moderatorUserId = from.moderatorUserId;
			moderatorDisplayName = from.moderatorDisplayName;
			targetUserId = from.targetUserId;
			targetDisplayName = from.targetDisplayName;
		}

		public static void SendModeration(string targetUserId, ModerationType mType, Action<ApiPlayerModeration> successCallback = null, Action<string> errorCallback = null)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["type"] = mType.ToString().ToLower();
			dictionary["moderated"] = targetUserId.ToString();
			ApiModel.SendPostRequest("auth/user/playermoderations", dictionary, delegate(Dictionary<string, object> obj)
			{
				ApiPlayerModeration apiPlayerModeration = new ApiPlayerModeration();
				apiPlayerModeration.Init(obj);
				if (successCallback != null)
				{
					successCallback(apiPlayerModeration);
				}
			}, delegate(string obj)
			{
				if (errorCallback != null)
				{
					errorCallback(obj);
				}
			});
		}

		public static void DeleteModeration(string moderationId, Action successCallback, Action<string> errorCallback)
		{
			ApiModel.SendRequest("auth/user/playermoderations/" + moderationId, HTTPMethods.Delete, (Dictionary<string, string>)null, (Action<Dictionary<string, object>>)delegate
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

		public static void FetchAllAgainstMe(Action<List<ApiPlayerModeration>> successCallback, Action<string> errorCallback)
		{
			FetchList("auth/user/playermoderated", successCallback, errorCallback);
		}

		public static void FetchAllMine(Action<List<ApiPlayerModeration>> successCallback, Action<string> errorCallback)
		{
			FetchList("auth/user/playermoderations", successCallback, errorCallback);
		}

		private static void FetchList(string endpoint, Action<List<ApiPlayerModeration>> successCallback, Action<string> errorCallback)
		{
			ApiModel.SendGetRequest(endpoint, delegate(List<object> objects)
			{
				List<ApiPlayerModeration> list = new List<ApiPlayerModeration>();
				if (objects != null)
				{
					foreach (object @object in objects)
					{
						Dictionary<string, object> jsonObject = @object as Dictionary<string, object>;
						ApiPlayerModeration apiPlayerModeration = new ApiPlayerModeration();
						apiPlayerModeration.Init(jsonObject);
						list.Add(apiPlayerModeration);
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
	}
}
