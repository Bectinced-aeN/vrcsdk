using System;
using UnityEngine;

namespace VRCSDK2
{
	[RequireComponent(typeof(Rigidbody))]
	public class VRC_Pickup : VRC_Behaviour, INetworkID
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

		public bool DisallowTheft;

		public Rigidbody physicalRoot;

		public Transform ExactGun;

		public Transform ExactGrip;

		public bool allowManipulationWhenEquipped;

		public PickupOrientation orientation;

		public AutoHoldMode AutoHold;

		public string UseText = "Use";

		[HideInInspector]
		[Obsolete("Please use a VRC_Trigger", false)]
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

		[Obsolete("Please use a VRC_Trigger", false)]
		[HideInInspector]
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

		[Range(0f, 100f)]
		public float proximity = 2f;

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

		public int NetworkID
		{
			get;
			set;
		}

		public VRC_PlayerApi currentPlayer => (_GetCurrentPlayer == null) ? null : _GetCurrentPlayer(this);

		public bool IsHeld => currentPlayer != null && currentHand != PickupHand.None;

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
			if (this.get_transform().get_parent() != originalParent)
			{
				this.get_transform().SetParent(originalParent);
			}
			if (physicalRoot != null)
			{
				if (physicalRoot.get_isKinematic() != originalKinematic)
				{
					physicalRoot.set_isKinematic(originalKinematic);
				}
				if (physicalRoot.get_useGravity() != originalGravity)
				{
					physicalRoot.set_useGravity(originalGravity);
				}
				Collider component = physicalRoot.GetComponent<Collider>();
				if (component != null && component.get_isTrigger() != originalTrigger)
				{
					component.set_isTrigger(originalTrigger);
				}
			}
		}
	}
}