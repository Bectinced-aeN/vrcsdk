using System;
using System.Collections.Generic;
using UnityEngine;

namespace VRCSDK2
{
	public class VRC_Station : MonoBehaviour, IVRCEventProvider, IVRCEventReceiver
	{
		public enum Mobility
		{
			Mobile,
			Immobilize,
			ImmobilizeForVehicle
		}

		public delegate void InitializationDelegate(VRC_Station obj);

		[Obsolete("Please set the PlayerMobility value instead")]
		private bool? shouldImmobolizePlayer;

		public Mobility PlayerMobility = Mobility.Immobilize;

		public bool canUseStationFromStation = true;

		public RuntimeAnimatorController animatorController;

		public bool disableStationExit;

		public bool seated = true;

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
			if (shouldImmobolizePlayer.HasValue)
			{
				PlayerMobility = (shouldImmobolizePlayer.Value ? Mobility.Immobilize : Mobility.Mobile);
			}
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
			vrcEvent.ParameterInt = 6;
			vrcEvent.ParameterString = "UseStation";
			vrcEvent.ParameterObjects = (GameObject[])new GameObject[1]
			{
				this.get_gameObject()
			};
			list.Add(vrcEvent);
			VRC_EventHandler.VrcEvent vrcEvent2 = new VRC_EventHandler.VrcEvent();
			vrcEvent2.Name = "ExitStation";
			vrcEvent2.EventType = VRC_EventHandler.VrcEventType.SendRPC;
			vrcEvent2.ParameterInt = 6;
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
