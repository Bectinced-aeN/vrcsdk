using System;
using System.Collections.Generic;
using UnityEngine;

namespace VRC.Core
{
	public class ApiAnalyticEvent
	{
		public enum EventType
		{
			startsVrChat,
			leavesVrChat,
			logout,
			deviceInfo,
			joinsWorld,
			leavesWorld,
			userActivity,
			error
		}

		public class EventInfo : ApiModel
		{
			[ApiField]
			public string worldId = string.Empty;

			[ApiField]
			public string userId = string.Empty;

			[ApiField]
			public EventType eventType = EventType.error;

			[ApiField]
			public Vector3? location;

			public Dictionary<string, string> parameters = new Dictionary<string, string>();

			public Action<bool> completeCallback;

			public EventInfo()
				: base("eventstream")
			{
			}

			public override void Save(Action<ApiContainer> onSuccess, Action<ApiContainer> onFailure)
			{
				Dictionary<string, object> dictionary = ExtractApiFields();
				foreach (KeyValuePair<string, string> parameter in parameters)
				{
					if (dictionary.ContainsKey(parameter.Key))
					{
						dictionary[parameter.Key] = parameter.Value;
					}
					else
					{
						dictionary.Add(parameter.Key, parameter.Value);
					}
				}
				ApiContainer apiContainer = new ApiContainer();
				apiContainer.OnSuccess = delegate(ApiContainer c)
				{
					if (completeCallback != null)
					{
						completeCallback(obj: true);
					}
					if (onSuccess != null)
					{
						onSuccess(c);
					}
				};
				apiContainer.OnError = delegate(ApiContainer c)
				{
					if (completeCallback != null)
					{
						completeCallback(obj: false);
					}
					if (onFailure != null)
					{
						onFailure(c);
					}
				};
				apiContainer.Model = this;
				ApiContainer responseContainer = apiContainer;
				API.SendPostRequest("eventstream/" + eventType.ToString(), responseContainer, dictionary);
			}

			protected override bool ReadField(string fieldName, ref object data)
			{
				//IL_0031: Unknown result type (might be due to invalid IL or missing references)
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				//IL_0059: Unknown result type (might be due to invalid IL or missing references)
				//IL_005e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0081: Unknown result type (might be due to invalid IL or missing references)
				//IL_0086: Unknown result type (might be due to invalid IL or missing references)
				if (fieldName == "location")
				{
					if (!location.HasValue)
					{
						return false;
					}
					string[] obj = new string[5];
					Vector3 value = location.Value;
					obj[0] = value.x.ToString("0.00");
					obj[1] = ",";
					Vector3 value2 = location.Value;
					obj[2] = value2.y.ToString("0.00");
					obj[3] = ",";
					Vector3 value3 = location.Value;
					obj[4] = value3.z.ToString("0.00");
					data = string.Concat(obj);
					return true;
				}
				return base.ReadField(fieldName, ref data);
			}
		}

		public static void Send(IEnumerable<EventInfo> events)
		{
			foreach (EventInfo @event in events)
			{
				@event.Save(null, null);
			}
		}
	}
}
