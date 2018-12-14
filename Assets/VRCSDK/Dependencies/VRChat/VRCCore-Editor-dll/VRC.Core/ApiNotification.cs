using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using VRC.Core.BestHTTP;
using VRC.Core.BestHTTP.JSON;

namespace VRC.Core
{
	public class ApiNotification : ApiModel
	{
		public enum NotificationType
		{
			All,
			Message,
			Friendrequest,
			Invite,
			Requestinvite,
			VoteToKick,
			Halp,
			Hidden
		}

		public string senderUserId;

		public string receiverUserId;

		public string senderUsername;

		public NotificationType notificationType;

		public string message;

		public Dictionary<string, object> details;

		public bool seen;

		public DateTime? localCeationTime;

		public bool isLocal => localCeationTime.HasValue;

		public void Init(Dictionary<string, object> jsonObject)
		{
			mId = (jsonObject["id"] as string);
			senderUserId = (jsonObject["senderUserId"] as string);
			if (jsonObject.ContainsKey("receiverUserId"))
			{
				receiverUserId = (jsonObject["receiverUserId"] as string);
			}
			if (jsonObject.ContainsKey("senderUsername"))
			{
				senderUsername = (jsonObject["senderUsername"] as string);
			}
			string value = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase((jsonObject["type"] as string).ToLower());
			notificationType = (NotificationType)(int)Enum.Parse(typeof(NotificationType), value);
			if (jsonObject.ContainsKey("message"))
			{
				message = (jsonObject["message"] as string);
			}
			if (jsonObject.ContainsKey("seen"))
			{
				seen = ((jsonObject["seen"] as string== "true") ? true : false);
			}
			if (jsonObject.ContainsKey("details"))
			{
				details = (Json.Decode(jsonObject["details"] as string) as Dictionary<string, object>);
			}
		}

		public void LocalInit(string _senderUserId, string _senderUsername, NotificationType _type, string _message, Dictionary<string, object> _details)
		{
			mId = Tools.GetRandomDigits(8);
			senderUserId = _senderUserId;
			senderUsername = _senderUsername;
			notificationType = _type;
			message = _message;
			details = _details;
			localCeationTime = DateTime.Now;
		}

		public static void SendNotification(string targetUserId, NotificationType nType, string message, Dictionary<string, object> details, Action<ApiNotification> successCallback, Action<string> errorCallback)
		{
			string value = nType.ToString().ToLower();
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["type"] = value;
			dictionary["message"] = message;
			if (details != null)
			{
				dictionary["details"] = Json.Encode(details);
			}
			ApiModel.SendPostRequest("user/" + targetUserId + "/notification", dictionary, delegate(Dictionary<string, object> obj)
			{
				ApiNotification apiNotification = new ApiNotification();
				apiNotification.Init(obj);
				if (successCallback != null)
				{
					successCallback(apiNotification);
				}
			}, delegate(string obj)
			{
				if (errorCallback != null)
				{
					errorCallback(obj);
				}
			});
		}

		public static void MarkNotificationAsSeen(string notificationId, Action<ApiNotification> successCallback, Action<string> errorCallback)
		{
			ApiModel.SendPutRequest("auth/user/notifications/" + notificationId + "/see", delegate(Dictionary<string, object> obj)
			{
				ApiNotification apiNotification = new ApiNotification();
				apiNotification.Init(obj);
				if (successCallback != null)
				{
					successCallback(apiNotification);
				}
			}, delegate(string obj)
			{
				if (errorCallback != null)
				{
					errorCallback(obj);
				}
			});
		}

		public static void DeleteNotification(string notificationId, Action successCallback, Action<string> errorCallback)
		{
			ApiModel.SendPutRequest("/auth/user/notifications/" + notificationId + "/hide", delegate
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

		public static void FetchAll(NotificationType t, bool sentMessages, string afterString, Action<List<ApiNotification>> successCallback, Action<string> errorCallback)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			switch (t)
			{
			case NotificationType.Friendrequest:
				dictionary["type"] = "friendRequest";
				break;
			default:
				dictionary["type"] = t.ToString().ToLower();
				break;
			case NotificationType.All:
				break;
			}
			if (sentMessages)
			{
				dictionary["sent"] = "true";
			}
			if (!string.IsNullOrEmpty(afterString))
			{
				dictionary["after"] = afterString;
			}
			ApiModel.SendRequest("auth/user/notifications", HTTPMethods.Get, dictionary, delegate(List<object> objects)
			{
				List<ApiNotification> list = new List<ApiNotification>();
				if (objects != null)
				{
					foreach (object @object in objects)
					{
						Dictionary<string, object> jsonObject = @object as Dictionary<string, object>;
						ApiNotification apiNotification = new ApiNotification();
						apiNotification.Init(jsonObject);
						list.Add(apiNotification);
					}
				}
				if (successCallback != null)
				{
					successCallback(list);
				}
			}, delegate(string message)
			{
				Debug.LogError((object)("Could not fetch notifications with error - " + message));
				errorCallback(message);
			});
		}
	}
}
