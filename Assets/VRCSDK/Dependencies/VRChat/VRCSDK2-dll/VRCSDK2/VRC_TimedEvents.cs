using System;
using UnityEngine;

namespace VRCSDK2
{
	[Obsolete("Please use VRC_Trigger", false)]
	public class VRC_TimedEvents : MonoBehaviour, IVRCEventSender
	{
		public bool Repeat = true;

		public float LowPeriodTime = 5f;

		public float HighPeriodTime = 10f;

		public bool ResetOnEnable = true;

		public string EventName;

		public VRC_EventHandler.VrcBroadcastType BroadcastType = VRC_EventHandler.VrcBroadcastType.Master;

		private bool EventFired;

		private float Duration;

		private float Timer;

		private VRC_EventHandler Handler;

		public VRC_TimedEvents()
			: this()
		{
		}

		private void Start()
		{
			Handler = this.GetComponent<VRC_EventHandler>();
			if (Handler == null)
			{
				Handler = this.GetComponentInParent<VRC_EventHandler>();
			}
			ResetClock();
		}

		private void OnEnable()
		{
			if (ResetOnEnable)
			{
				ResetClock();
			}
		}

		private void Update()
		{
			Timer += Time.get_deltaTime();
			if (Timer > Duration && !EventFired && Handler != null)
			{
				Handler.TriggerEvent(EventName, BroadcastType);
				if (Repeat)
				{
					ResetClock();
				}
				else
				{
					EventFired = true;
				}
			}
		}

		private void ResetClock()
		{
			Duration = LowPeriodTime + Random.get_value() * (HighPeriodTime - LowPeriodTime);
			Timer = 0f;
			EventFired = false;
		}
	}
}
