using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRCSDK2
{
	[ExecuteInEditMode]
	public class VRC_EventHandler : VRC_Behaviour, INetworkID
	{
		public enum VrcEventType
		{
			MeshVisibility = 0,
			AnimationFloat = 1,
			AnimationBool = 2,
			AnimationTrigger = 3,
			AudioTrigger = 4,
			PlayAnimation = 5,
			SendMessage = 6,
			SetParticlePlaying = 7,
			TeleportPlayer = 8,
			RunConsoleCommand = 9,
			SetGameObjectActive = 10,
			SetWebPanelURI = 11,
			SetWebPanelVolume = 12,
			SpawnObject = 13,
			SendRPC = 14,
			ActivateCustomTrigger = 0xF,
			DestroyObject = 0x10,
			SetLayer = 17,
			SetMaterial = 18,
			AddHealth = 19,
			AddDamage = 20,
			SetComponentActive = 21,
			AnimationInt = 22,
			AnimationIntAdd = 24,
			AnimationIntSubtract = 25,
			AnimationIntMultiply = 26,
			AnimationIntDivide = 27,
			AddVelocity = 28,
			SetVelocity = 29,
			AddAngularVelocity = 30,
			SetAngularVelocity = 0x1F,
			AddForce = 0x20,
			SetUIText = 33
		}

		public enum VrcBroadcastType
		{
			Always,
			Master,
			Local,
			Owner,
			AlwaysUnbuffered,
			MasterUnbuffered,
			OwnerUnbuffered,
			AlwaysBufferOne,
			MasterBufferOne,
			OwnerBufferOne
		}

		public enum VrcTargetType
		{
			All,
			Others,
			Owner,
			Master,
			AllBuffered,
			OthersBuffered,
			Local,
			AllBufferOne,
			OthersBufferOne,
			TargetPlayer
		}

		public enum VrcBooleanOp
		{
			Unused = -1,
			False,
			True,
			Toggle
		}

		[Serializable]
		public class VrcEvent
		{
			public string Name = string.Empty;

			public VrcEventType EventType = VrcEventType.AudioTrigger;

			public string ParameterString = string.Empty;

			public VrcBooleanOp ParameterBoolOp = VrcBooleanOp.Unused;

			[HideInInspector]
			public bool ParameterBool;

			public float ParameterFloat;

			public int ParameterInt = 1;

			[HideInInspector]
			public GameObject ParameterObject;

			[SerializeField]
			public GameObject[] ParameterObjects;

			[HideInInspector]
			[SerializeField]
			public byte[] ParameterBytes;

			[HideInInspector]
			[SerializeField]
			public int? ParameterBytesVersion;

			public bool TakeOwnershipOfTarget;
		}

		public class EventInfo
		{
			public VrcEvent evt;

			public VrcBroadcastType broadcast;

			public GameObject instagator;

			public float fastForward;
		}

		public delegate int GetNetworkIdDelegate(GameObject obj);

		public delegate void LogEventDelegate(VRC_EventHandler eventHandler, VrcEvent vrcEvent, VrcBroadcastType broadcast, int instagatorId, float fastForward);

		[SerializeField]
		public List<VrcEvent> Events = new List<VrcEvent>();

		private VRC_EventDispatcher _dispatcher;

		public static GetNetworkIdDelegate GetInstigatorId;

		public static LogEventDelegate LogEvent;

		public List<EventInfo> deferredEvents = new List<EventInfo>();

		private Coroutine DeferredEventProcessor;

		public int NetworkID
		{
			get;
			set;
		}

		private VRC_EventDispatcher Dispatcher
		{
			get
			{
				if (_dispatcher == null)
				{
					_dispatcher = Networking.GetEventDispatcher();
				}
				return _dispatcher;
			}
		}

		public static GetNetworkIdDelegate GetInsitgatorId
		{
			get
			{
				return GetInstigatorId;
			}
			set
			{
				GetInstigatorId = value;
			}
		}

		public static bool BooleanOp(VrcBooleanOp Op, bool Current)
		{
			switch (Op)
			{
			case VrcBooleanOp.False:
				return false;
			case VrcBooleanOp.True:
				return true;
			case VrcBooleanOp.Toggle:
				return !Current;
			case VrcBooleanOp.Unused:
				return Current;
			default:
				return false;
			}
		}

		private void Awake()
		{
			VRC_EventHandler[] components = this.get_gameObject().GetComponents<VRC_EventHandler>();
			if (components.Length > 1 || components[0] != this)
			{
				Debug.LogError((object)("Multiple event handlers found on " + this.get_gameObject().get_name() + ", bad things will happen."));
			}
			foreach (VrcEvent @event in Events)
			{
				if (@event.ParameterBoolOp == VrcBooleanOp.Unused)
				{
					if (@event.ParameterBool)
					{
						@event.ParameterBoolOp = VrcBooleanOp.True;
					}
					else
					{
						@event.ParameterBoolOp = VrcBooleanOp.False;
					}
				}
			}
		}

		public void VrcAnimationEvent(AnimationEvent aEvent)
		{
			foreach (VrcEvent @event in Events)
			{
				if (!(@event.Name != aEvent.get_stringParameter()))
				{
					TriggerEvent(@event, VrcBroadcastType.Local);
				}
			}
		}

		public static bool IsReceiverRequiredForEventType(VrcEventType eventType)
		{
			return eventType != VrcEventType.AddDamage && eventType != VrcEventType.AddHealth;
		}

		public void TriggerEvent(VrcEvent e, VrcBroadcastType broadcast, GameObject instagator = null, float fastForward = 0f)
		{
			if (e == null)
			{
				throw new ArgumentException("Event was null");
			}
			if (instagator == null)
			{
				Debug.LogErrorFormat("Cancelling event {0} because the Instagator was null.", new object[1]
				{
					e.Name
				});
			}
			else
			{
				VRC_EventDispatcher dispatcher = Dispatcher;
				if (dispatcher == null)
				{
					Debug.LogFormat("Deferring event {0} of type {1} because dispatcher is unavailable.", new object[2]
					{
						e.Name,
						e.EventType
					});
					DeferEvent(e, broadcast, instagator, fastForward);
				}
				else if ((e.ParameterObjects == null || e.ParameterObjects.Length == 0) && e.ParameterObject == null)
				{
					Debug.LogError((object)("No object to receive event " + e.Name + " of type " + e.EventType));
				}
				else
				{
					int instagatorId = GetInstigatorId(instagator);
					if (e.ParameterObjects != null)
					{
						GameObject[] parameterObjects = e.ParameterObjects;
						foreach (GameObject parameterObject in parameterObjects)
						{
							GameObject parameterObject2 = e.ParameterObject;
							e.ParameterObject = parameterObject;
							try
							{
								if (null == e.ParameterObject && IsReceiverRequiredForEventType(e.EventType))
								{
									Debug.LogWarning((object)("Null object in parameter objects to receive event " + e.Name + " of type " + e.EventType + ", trigger event ignored."));
								}
								else
								{
									InternalTriggerEvent(e, broadcast, instagatorId, fastForward);
								}
							}
							finally
							{
								e.ParameterObject = parameterObject2;
							}
						}
					}
					if (e.ParameterObject != null)
					{
						InternalTriggerEvent(e, broadcast, instagatorId, fastForward);
					}
				}
			}
		}

		private void InternalTriggerEvent(VrcEvent e, VrcBroadcastType broadcast, int instagatorId, float fastForward)
		{
			if (instagatorId <= 0)
			{
				Debug.LogErrorFormat("Cancelling event because the Instigator was invalid: {0}/{1}", new object[2]
				{
					e.EventType,
					broadcast
				});
			}
			if (LogEvent != null)
			{
				LogEvent(this, e, broadcast, instagatorId, fastForward);
			}
			else
			{
				Dispatcher.TriggerEvent(this, e, broadcast, instagatorId, fastForward);
			}
		}

		[Obsolete("Use the player object as the instigator", false)]
		public void TriggerEvent(VrcEvent e, VrcBroadcastType broadcast, int instagatorId, float fastForward)
		{
			if (e == null)
			{
				Debug.LogErrorFormat("Cancelling event because it was not valid", new object[0]);
			}
			else
			{
				VRC_PlayerApi playerById = VRC_PlayerApi.GetPlayerById(instagatorId);
				if (playerById == null)
				{
					Debug.LogErrorFormat("Cancelling event because instagator was not valid", new object[0]);
				}
				else
				{
					TriggerEvent(e, broadcast, playerById.get_gameObject(), fastForward);
				}
			}
		}

		[Obsolete("Do not trigger events by name", false)]
		public void TriggerEvent(string eventName, VrcBroadcastType broadcast, GameObject instagator = null, int instagatorId = 0)
		{
			foreach (VrcEvent @event in Events)
			{
				if (!(@event.Name != eventName))
				{
					TriggerEvent(@event, broadcast, instagator);
				}
			}
		}

		[Obsolete("Do not trigger events by name", false)]
		public void TriggerEvent(string eventName, VrcBroadcastType broadcast, GameObject instagator, int instagatorId, float fastForward)
		{
			foreach (VrcEvent @event in Events)
			{
				if (!(@event.Name != eventName))
				{
					TriggerEvent(@event, broadcast, instagator, fastForward);
				}
			}
		}

		private void OnValidate()
		{
			foreach (VrcEvent @event in Events)
			{
				if (@event.ParameterBoolOp == VrcBooleanOp.Unused)
				{
					if (@event.ParameterBool)
					{
						@event.ParameterBoolOp = VrcBooleanOp.True;
					}
					else
					{
						@event.ParameterBoolOp = VrcBooleanOp.False;
					}
				}
			}
		}

		private void OnDestroy()
		{
			if (deferredEvents.Count > 0)
			{
				Debug.LogError((object)"Not all events were triggered prior to the handler being destroyed.");
			}
			if (Dispatcher != null)
			{
				Dispatcher.UnregisterEventHandler(this);
			}
		}

		[Obsolete("Event Handler Combined ID is no longer used")]
		public long GetCombinedNetworkId()
		{
			return 0L;
		}

		public static bool HasEventTrigger(GameObject go)
		{
			return go.GetComponent<VRC_Interactable>() != null;
		}

		[Obsolete("They will defer.")]
		public bool IsReadyForEvents()
		{
			return true;
		}

		public void DeferEvent(VrcEvent e, VrcBroadcastType broadcast, GameObject instagator, float fastForward)
		{
			if (deferredEvents == null)
			{
				deferredEvents = new List<EventInfo>();
			}
			deferredEvents.Add(new EventInfo
			{
				evt = e,
				broadcast = broadcast,
				instagator = instagator,
				fastForward = fastForward
			});
			if (DeferredEventProcessor == null)
			{
				DeferredEventProcessor = Networking.SafeStartCoroutine(ProcessDeferredEvents());
			}
		}

		private IEnumerator ProcessDeferredEvents()
		{
			yield return (object)null;
			while (this != null && deferredEvents != null && deferredEvents.Count > 0)
			{
				if (Dispatcher == null)
				{
					yield return (object)null;
				}
				else
				{
					new List<EventInfo>(deferredEvents);
					deferredEvents = new List<EventInfo>();
					foreach (EventInfo deferredEvent in deferredEvents)
					{
						if (deferredEvent != null)
						{
							int instigatorId = GetInstigatorId(deferredEvent.instagator);
							if (instigatorId <= 0)
							{
								deferredEvents.Add(deferredEvent);
							}
							else
							{
								TriggerEvent(deferredEvent.evt, deferredEvent.broadcast, instigatorId, deferredEvent.fastForward);
							}
						}
					}
					yield return (object)null;
				}
			}
			DeferredEventProcessor = null;
		}
	}
}
