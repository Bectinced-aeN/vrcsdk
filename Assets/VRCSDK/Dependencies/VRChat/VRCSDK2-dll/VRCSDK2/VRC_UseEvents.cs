using System;
using UnityEngine;

namespace VRCSDK2
{
	[Obsolete("Please use VRC_Trigger", false)]
	public class VRC_UseEvents : VRC_Interactable, IVRCEventSender
	{
		public delegate void UpdateDelegate(VRC_UseEvents obj);

		public string EventName = "Use";

		public bool isHidden;

		public VRC_EventHandler.VrcBroadcastType BroadcastType;

		public static UpdateDelegate UpdateUse;

		public override void Awake()
		{
			base.Awake();
			if (isHidden)
			{
				this.get_gameObject().set_layer(LayerMask.NameToLayer("Default"));
			}
		}

		public override void Start()
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			if (interactTextPlacement != null && interactTextGO != null)
			{
				interactTextGO.get_transform().set_position(interactTextPlacement.get_position());
			}
		}

		public override void Interact()
		{
			VRC_EventHandler vRC_EventHandler = this.GetComponent<VRC_EventHandler>();
			if (vRC_EventHandler == null)
			{
				vRC_EventHandler = this.GetComponentInParent<VRC_EventHandler>();
			}
			if (vRC_EventHandler != null)
			{
				vRC_EventHandler.TriggerEvent(EventName, BroadcastType, Networking.LocalPlayer.get_gameObject());
			}
		}
	}
}
