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
			Hidden,
			Broadcast
		}

		public DateTime? localCeationTime;

		[ApiField]
		public string senderUserId
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public string receiverUserId
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public string senderUsername
		{
			get;
			set;
		}

		[ApiField(Name = "type")]
		public NotificationType notificationType
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public string message
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public DateTime created_at
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
		public bool seen
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public string jobName
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public string jobColor
		{
			get;
			set;
		}

		public bool isLocal => localCeationTime.HasValue;

		public ApiNotification()
			: base(null)
		{
		}

		public ApiNotification(string targetUserId)
			: base("user/" + targetUserId + "/notification")
		{
		}

		public ApiNotification(string notificationId, string subEndpoint, bool doNull = false)
			: base("auth/user/notifications/" + notificationId + "/" + subEndpoint)
		{
			base.id = ((!doNull) ? notificationId : null);
		}

		public override bool ShouldCache()
		{
			return false;
		}

		public override void Delete(Action<ApiContainer> onSuccess = null, Action<ApiContainer> onFailure = null)
		{
			ApiNotification apiNotification = new ApiNotification(base.id, "hide", doNull: true);
			apiNotification.Put(onSuccess, onFailure);
		}

		protected override bool ReadField(string fieldName, ref object data)
		{
			switch (fieldName)
			{
			case "type":
				data = notificationType.ToString().ToLower();
				return true;
			default:
				return base.ReadField(fieldName, ref data);
			}
		}

		protected override bool WriteField(string fieldName, object data)
		{
			switch (fieldName)
			{
			case "type":
			{
				string value = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase((data as string).ToLower());
				notificationType = (NotificationType)(int)Enum.Parse(typeof(NotificationType), value);
				return true;
			}
			default:
				return base.WriteField(fieldName, data);
			}
		}

		public void LocalInit(string _senderUserId, string _senderUsername, NotificationType _type, string _message, Dictionary<string, object> _details)
		{
			base.id = Tools.GetRandomDigits(8);
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
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["type"] = value;
			dictionary["message"] = message;
			if (details != null)
			{
				dictionary["details"] = Json.Encode(details);
			}
			ApiModelContainer<ApiNotification> apiModelContainer = new ApiModelContainer<ApiNotification>();
			apiModelContainer.OnSuccess = delegate(ApiContainer c)
			{
				if (successCallback != null)
				{
					successCallback(c.Model as ApiNotification);
				}
			};
			apiModelContainer.OnError = delegate(ApiContainer c)
			{
				if (errorCallback != null)
				{
					errorCallback(c.Error);
				}
			};
			ApiModelContainer<ApiNotification> responseContainer = apiModelContainer;
			API.SendPostRequest("user/" + targetUserId + "/notification", responseContainer, dictionary);
		}

		public static void MarkNotificationAsSeen(string notificationId, Action<ApiNotification> successCallback, Action<string> errorCallback)
		{
			ApiNotification apiNotification = new ApiNotification(notificationId, "see", doNull: true);
			apiNotification.Put(delegate(ApiContainer c)
			{
				if (successCallback != null)
				{
					successCallback(c.Model as ApiNotification);
				}
			}, delegate(ApiContainer c)
			{
				if (errorCallback != null)
				{
					errorCallback(c.Error);
				}
			});
		}

		public static void DeleteNotification(string notificationId, Action successCallback, Action<string> errorCallback)
		{
			ApiNotification apiNotification = new ApiNotification(notificationId, "hide");
			apiNotification.Delete(delegate
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

		public static void FetchAll(NotificationType t, bool sentMessages, string afterString, Action<IEnumerable<ApiNotification>> successCallback, Action<string> errorCallback)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
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
			ApiModelListContainer<ApiNotification> apiModelListContainer = new ApiModelListContainer<ApiNotification>();
			apiModelListContainer.OnSuccess = delegate(ApiContainer c)
			{
				if (successCallback != null)
				{
					successCallback((c as ApiModelListContainer<ApiNotification>).ResponseModels);
				}
			};
			apiModelListContainer.OnError = delegate(ApiContainer c)
			{
				Debug.LogError((object)("Could not fetch notifications with error - " + c.Error));
				if (errorCallback != null)
				{
					errorCallback(c.Error);
				}
			};
			ApiModelListContainer<ApiNotification> responseContainer = apiModelListContainer;
			API.SendRequest("auth/user/notifications", HTTPMethods.Get, responseContainer, dictionary, authenticationRequired: true, disableCache: true);
		}
	}
}
