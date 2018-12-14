using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRC.Core.BestHTTP;

namespace VRC.Core
{
	public class ApiAnalyticEvent : ApiModel
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

		public class EventInfo
		{
			public string worldId;

			public string userId;

			public EventType eventType = EventType.error;

			public Dictionary<string, object> parameters = new Dictionary<string, object>();

			public Vector3? location;

			public Action<bool> completeCallback;
		}

		public static void Send(EventInfo evt)
		{
			Send(new EventInfo[1]
			{
				evt
			});
		}

		public static void Send(IEnumerable<EventInfo> events)
		{
			ApiModel.SendRequestBatch(events.Select(delegate(EventInfo evt)
			{
				//IL_009e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
				Dictionary<string, object> dictionary = new Dictionary<string, object>(evt.parameters)
				{
					["eventType"] = evt.eventType.ToString(),
					["worldId"] = evt.worldId,
					["userId"] = evt.userId
				};
				Dictionary<string, object> dictionary2 = dictionary;
				object value;
				if (!evt.location.HasValue)
				{
					value = null;
				}
				else
				{
					string[] obj2 = new string[5];
					Vector3 value2 = evt.location.Value;
					obj2[0] = value2.x.ToString("0.00");
					obj2[1] = ",";
					Vector3 value3 = evt.location.Value;
					obj2[2] = value3.y.ToString("0.00");
					obj2[3] = ",";
					Vector3 value4 = evt.location.Value;
					obj2[4] = value4.z.ToString("0.00");
					value = string.Concat(obj2);
				}
				dictionary2["position"] = value;
				return new RequestInfo
				{
					endpoint = "eventstream/" + evt.eventType.ToString(),
					method = HTTPMethods.Post,
					requestParams = dictionary,
					successCallbackWithDict = delegate
					{
						if (evt.completeCallback != null)
						{
							evt.completeCallback(obj: true);
						}
					},
					errorCallback = delegate
					{
						if (evt.completeCallback != null)
						{
							evt.completeCallback(obj: false);
						}
					}
				};
			}));
		}
	}
}
