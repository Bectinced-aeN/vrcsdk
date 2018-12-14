using System;
using UnityEngine;

namespace VRCSDK2
{
	[RequireComponent(typeof(Rigidbody))]
	public class VRC_Pickup : VRC_Behaviour
	{
		public enum PickupOrientation
		{
			Any,
			Grip,
			Gun
		}

		public enum AutoHoldMode
		{
			AutoDetect,
			Yes,
			No
		}

		public enum PickupHand
		{
			None,
			Left,
			Right
		}

		public delegate void AwakeDelegate(VRC_Pickup obj);

		public delegate void ForceDropDelegate(VRC_Pickup obj);

		public delegate void OnDestroyedDelegate(VRC_Pickup obj);

		public delegate void HapticEventDelegate(VRC_Pickup obj, float duration, float amplitude, float frequency);

		public Rigidbody physicalRoot;

		public Transform ExactGun;

		public Transform ExactGrip;

		public bool allowManipulationWhenEquipped;

		public PickupOrientation orientation;

		public AutoHoldMode AutoHold;

		public string UseText = "Use";

		[Obsolete("Please use a VRC_Trigger", false)]
		[HideInInspector]
		public VRC_EventHandler.VrcBroadcastType useEventBroadcastType;

		[Obsolete("Please use a VRC_Trigger", false)]
		[HideInInspector]
		public string UseDownEventName;

		[HideInInspector]
		[Obsolete("Please use a VRC_Trigger", false)]
		public string UseUpEventName;

		[Obsolete("Please use a VRC_Trigger", false)]
		[HideInInspector]
		public VRC_EventHandler.VrcBroadcastType pickupDropEventBroadcastType;

		[HideInInspector]
		[Obsolete("Please use a VRC_Trigger", false)]
		public string PickupEventName;

		[HideInInspector]
		[Obsolete("Please use a VRC_Trigger", false)]
		public string DropEventName;

		public float ThrowVelocityBoostMinSpeed = 1f;

		public float ThrowVelocityBoostScale = 1f;

		[HideInInspector]
		public Component currentlyHeldBy;

		[HideInInspector]
		public VRC_PlayerApi currentLocalPlayer;

		public bool pickupable = true;

		public static AwakeDelegate OnAwake;

		public static ForceDropDelegate ForceDrop;

		public static OnDestroyedDelegate OnDestroyed;

		public static HapticEventDelegate HapticEvent;

		[HideInInspector]
		public bool originalKinematic;

		[HideInInspector]
		public bool originalGravity = true;

		[HideInInspector]
		public bool originalTrigger;

		[HideInInspector]
		public Transform originalParent;

		public static Func<VRC_Pickup, PickupHand> _GetPickupHand;

		public static Func<VRC_Pickup, VRC_PlayerApi> _GetCurrentPlayer;

		public VRC_PlayerApi currentPlayer => _GetCurrentPlayer(this);

		public PickupHand currentHand => _GetPickupHand(this);

		private void Awake()
		{
			if (OnAwake != null)
			{
				OnAwake(this);
			}
		}

		private void Reset()
		{
			this.get_gameObject().set_layer(LayerMask.NameToLayer("Pickup"));
		}

		public void Drop()
		{
			if (ForceDrop != null)
			{
				ForceDrop(this);
			}
		}

		public void GenerateHapticEvent(float duration = 0.25f, float amplitude = 0.5f, float frequency = 0.5f)
		{
			if (HapticEvent != null)
			{
				HapticEvent(this, duration, amplitude, frequency);
			}
		}

		private void OnDestroy()
		{
			if (OnDestroyed != null)
			{
				OnDestroyed(this);
			}
		}

		public void RevertPhysics()
		{
			this.get_transform().SetParent(originalParent);
			if (physicalRoot != null)
			{
				physicalRoot.set_isKinematic(originalKinematic);
				physicalRoot.set_useGravity(originalGravity);
				Collider component = physicalRoot.GetComponent<Collider>();
				if (component != null)
				{
					component.set_isTrigger(originalTrigger);
				}
			}
		}
	}
}
