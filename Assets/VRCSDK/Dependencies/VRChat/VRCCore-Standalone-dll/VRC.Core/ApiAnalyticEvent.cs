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
				//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
				//IL_011c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0121: Unknown result type (might be due to invalid IL or missing references)
				Dictionary<string, object> dictionary = new Dictionary<string, object>(evt.parameters)
				{
					["eventType"] = evt.eventType.ToString()
				};
				if (!string.IsNullOrEmpty(evt.worldId))
				{
					dictionary["worldId"] = evt.worldId;
				}
				if (!string.IsNullOrEmpty(evt.userId))
				{
					dictionary["userId"] = evt.userId;
				}
				if (evt.location.HasValue)
				{
					Dictionary<string, object> dictionary2 = dictionary;
					string[] obj2 = new string[5];
					Vector3 value = evt.location.Value;
					obj2[0] = value.x.ToString("0.00");
					obj2[1] = ",";
					Vector3 value2 = evt.location.Value;
					obj2[2] = value2.y.ToString("0.00");
					obj2[3] = ",";
					Vector3 value3 = evt.location.Value;
					obj2[4] = value3.z.ToString("0.00");
					dictionary2["position"] = string.Concat(obj2);
				}
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
