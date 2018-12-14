using System;
using System.Collections.Generic;
using UnityEngine;

namespace VRC.Core
{
	public class ApiNotification : ApiModel
	{
		public string senderUserId;

		public string receiverUserId;

		public string senderUsername;

		public string notificationType;

		public string message;

		public Dictionary<string, string> details;

		public bool seen;

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
			notificationType = (jsonObject["type"] as string);
			if (jsonObject.ContainsKey("message"))
			{
				message = (jsonObject["message"] as string);
			}
			if (jsonObject.ContainsKey("seen"))
			{
				seen = ((jsonObject["seen"] == "true") ? true : false);
			}
		}

		public static void Fetch(Action<List<ApiNotification>> successCallback, Action<string> errorCallback)
		{
			ApiModel.SendGetRequest("auth/user/notifications", delegate(List<object> objects)
			{
				List<ApiNotification> list = new List<ApiNotification>();
				if (objects != null)
				{
					foreach (object @object in objects)
					{
						Dictionary<string, object> jsonObject = @object as Dictionary<string, object>;
						ApiNotification apiNotification = ScriptableObject.CreateInstance<ApiNotification>();
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
