using System.Collections.Generic;
using UnityEngine;

namespace VRCSDK2
{
	[RequireComponent(typeof(VRC_EventHandler))]
	public class VRC_Station : MonoBehaviour, IVRCEventProvider, IVRCEventReceiver
	{
		public delegate void InitializationDelegate(VRC_Station obj);

		public bool shouldImmobolizePlayer;

		public bool canUseStationFromStation;

		public RuntimeAnimatorController animatorController;

		public bool disableStationExit;

		public Transform stationEnterPlayerLocation;

		public Transform stationExitPlayerLocation;

		public VRC_ObjectApi controlsObject;

		public static InitializationDelegate Initialize;

		public VRC_Station()
			: this()
		{
		}

		private void Awake()
		{
			if (Initialize != null)
			{
				Initialize(this);
			}
		}

		public IEnumerable<VRC_EventHandler.VrcEvent> ProvideEvents()
		{
			List<VRC_EventHandler.VrcEvent> list = new List<VRC_EventHandler.VrcEvent>();
			VRC_EventHandler.VrcEvent vrcEvent = new VRC_EventHandler.VrcEvent();
			vrcEvent.Name = "UseStation";
			vrcEvent.EventType = VRC_EventHandler.VrcEventType.SendRPC;
			vrcEvent.ParameterString = "UseStation";
			vrcEvent.ParameterObjects = (GameObject[])new GameObject[1]
			{
				this.get_gameObject()
			};
			list.Add(vrcEvent);
			VRC_EventHandler.VrcEvent vrcEvent2 = new VRC_EventHandler.VrcEvent();
			vrcEvent2.Name = "ExitStation";
			vrcEvent2.EventType = VRC_EventHandler.VrcEventType.SendRPC;
			vrcEvent2.ParameterString = "ExitStation";
			vrcEvent2.ParameterObjects = (GameObject[])new GameObject[1]
			{
				this.get_gameObject()
			};
			list.Add(vrcEvent2);
			return list;
		}
	}
}
