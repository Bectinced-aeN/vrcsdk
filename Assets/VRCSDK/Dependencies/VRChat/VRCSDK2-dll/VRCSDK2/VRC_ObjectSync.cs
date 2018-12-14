using System;
using System.Collections.Generic;
using UnityEngine;

namespace VRCSDK2
{
	public class VRC_ObjectSync : MonoBehaviour, INetworkID, IVRCEventProvider
	{
		public delegate void InitializationDelegate(VRC_ObjectSync obj);

		public delegate bool IsLocalDelegate(VRC_ObjectSync obj);

		public delegate void TeleportDelegate(VRC_ObjectSync obj, Vector3 position, Quaternion rotation);

		[HideInInspector]
		public int networkId;

		public static InitializationDelegate Initialize;

		public static IsLocalDelegate IsLocal;

		public bool SynchronizePhysics = true;

		public bool AllowCollisionTransfer = true;

		public static TeleportDelegate TeleportHandler;

		public static Func<VRC_ObjectSync, bool> GetUseGravity;

		public static Func<VRC_ObjectSync, bool> GetIsKinematic;

		public static Action<VRC_ObjectSync, bool> SetUseGravity;

		public static Action<VRC_ObjectSync, bool> SetIsKinematic;

		[HideInInspector]
		public int NetworkID
		{
			get
			{
				return networkId;
			}
			set
			{
				networkId = value;
			}
		}

		public bool useGravity
		{
			get
			{
				if (GetUseGravity != null)
				{
					return GetUseGravity(this);
				}
				return false;
			}
			set
			{
				if (SetUseGravity != null)
				{
					SetUseGravity(this, value);
				}
			}
		}

		public bool isKinematic
		{
			get
			{
				if (GetIsKinematic != null)
				{
					return GetIsKinematic(this);
				}
				return false;
			}
			set
			{
				if (SetIsKinematic != null)
				{
					SetIsKinematic(this, value);
				}
			}
		}

		public VRC_ObjectSync()
			: this()
		{
		}

		public void TeleportTo(Vector3 position, Quaternion rotation)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			if (TeleportHandler != null)
			{
				TeleportHandler(this, position, rotation);
			}
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
			vrcEvent.Name = "EnableKinematic";
			vrcEvent.EventType = VRC_EventHandler.VrcEventType.SendRPC;
			vrcEvent.ParameterInt = 0;
			vrcEvent.ParameterString = "EnableKinematic";
			vrcEvent.ParameterObjects = (GameObject[])new GameObject[1]
			{
				this.get_gameObject()
			};
			list.Add(vrcEvent);
			vrcEvent = new VRC_EventHandler.VrcEvent();
			vrcEvent.Name = "DisableKinematic";
			vrcEvent.EventType = VRC_EventHandler.VrcEventType.SendRPC;
			vrcEvent.ParameterInt = 0;
			vrcEvent.ParameterString = "DisableKinematic";
			vrcEvent.ParameterObjects = (GameObject[])new GameObject[1]
			{
				this.get_gameObject()
			};
			list.Add(vrcEvent);
			vrcEvent = new VRC_EventHandler.VrcEvent();
			vrcEvent.Name = "EnableGravity";
			vrcEvent.EventType = VRC_EventHandler.VrcEventType.SendRPC;
			vrcEvent.ParameterInt = 0;
			vrcEvent.ParameterString = "EnableGravity";
			vrcEvent.ParameterObjects = (GameObject[])new GameObject[1]
			{
				this.get_gameObject()
			};
			list.Add(vrcEvent);
			vrcEvent = new VRC_EventHandler.VrcEvent();
			vrcEvent.Name = "DisableGravity";
			vrcEvent.EventType = VRC_EventHandler.VrcEventType.SendRPC;
			vrcEvent.ParameterInt = 0;
			vrcEvent.ParameterString = "DisableGravity";
			vrcEvent.ParameterObjects = (GameObject[])new GameObject[1]
			{
				this.get_gameObject()
			};
			list.Add(vrcEvent);
			vrcEvent = new VRC_EventHandler.VrcEvent();
			vrcEvent.Name = "ReapObject";
			vrcEvent.EventType = VRC_EventHandler.VrcEventType.SendRPC;
			vrcEvent.ParameterInt = 0;
			vrcEvent.ParameterString = "ReapObject";
			vrcEvent.ParameterObjects = (GameObject[])new GameObject[1]
			{
				this.get_gameObject()
			};
			list.Add(vrcEvent);
			return list;
		}
	}
}
