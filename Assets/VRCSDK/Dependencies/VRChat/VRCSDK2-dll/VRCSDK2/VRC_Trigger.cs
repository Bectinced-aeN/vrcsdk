using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VRCSDK2
{
	[RequireComponent(typeof(VRC_EventHandler))]
	public class VRC_Trigger : VRC_Interactable, INetworkID, IVRCEventSender
	{
		[Serializable]
		public class CustomTriggerTarget
		{
			public GameObject TriggerObject;

			public string CustomName = string.Empty;
		}

		[Serializable]
		public enum TriggerType
		{
			Custom,
			Relay,
			OnEnable,
			OnDestroy,
			OnSpawn,
			OnNetworkReady,
			OnPlayerJoined,
			OnPlayerLeft,
			OnPickupUseDown,
			OnPickupUseUp,
			OnTimer,
			OnEnterTrigger,
			OnExitTrigger,
			OnKeyDown,
			OnKeyUp,
			OnPickup,
			OnDrop,
			OnInteract,
			OnEnterCollider,
			OnExitCollider,
			OnDataStorageChange,
			OnDataStorageRemove,
			OnDataStorageAdd,
			OnAvatarHit,
			OnStationEntered,
			OnStationExited,
			OnVideoStart,
			OnVideoEnd,
			OnVideoPlay,
			OnVideoPause,
			OnDisable,
			OnOwnershipTransfer,
			OnParticleCollision
		}

		public static class TypeCollections
		{
			public static TriggerType[] KeyTypes = new TriggerType[2]
			{
				TriggerType.OnKeyDown,
				TriggerType.OnKeyUp
			};

			public static TriggerType[] TimerTypes = new TriggerType[1]
			{
				TriggerType.OnTimer
			};

			public static TriggerType[] InteractiveTypes = new TriggerType[1]
			{
				TriggerType.OnInteract
			};

			public static TriggerType[] ColliderTypes = new TriggerType[2]
			{
				TriggerType.OnEnterCollider,
				TriggerType.OnExitCollider
			};

			public static TriggerType[] TriggerTypes = new TriggerType[2]
			{
				TriggerType.OnEnterTrigger,
				TriggerType.OnExitTrigger
			};

			public static TriggerType[] PickupTypes = new TriggerType[4]
			{
				TriggerType.OnPickupUseDown,
				TriggerType.OnPickupUseUp,
				TriggerType.OnPickup,
				TriggerType.OnDrop
			};

			public static TriggerType[] DataStorageTypes = new TriggerType[3]
			{
				TriggerType.OnDataStorageAdd,
				TriggerType.OnDataStorageChange,
				TriggerType.OnDataStorageRemove
			};
		}

		[Serializable]
		public class DataStorageShadow
		{
			public string ParameterString;

			public string ParameterBoolOp;

			public string ParameterFloat;

			public string ParameterInt;
		}

		[Serializable]
		public class TriggerEvent
		{
			public TriggerType TriggerType;

			public VRC_EventHandler.VrcBroadcastType BroadcastType;

			[SerializeField]
			public List<VRC_EventHandler.VrcEvent> Events = new List<VRC_EventHandler.VrcEvent>();

			public string Name;

			[SerializeField]
			public List<VRC_Trigger> Others = new List<VRC_Trigger>();

			public KeyCode Key;

			public bool TriggerIndividuals = true;

			public LayerMask Layers = LayerMask.op_Implicit(0);

			public bool Repeat = true;

			public float LowPeriodTime = 5f;

			public float HighPeriodTime = 10f;

			public bool ResetOnEnable = true;

			public bool EventFired;

			public float Duration;

			public float Timer;

			public int DataElementIdx;

			[SerializeField]
			public List<DataStorageShadow> DataStorageShadowValues = new List<DataStorageShadow>();

			public float AfterSeconds;

			[SerializeField]
			public bool[] ProbabilityLock = new bool[0];

			[SerializeField]
			public float[] Probabilities = new float[0];

			[Tooltip("Midi Channel [1-16], 0=any")]
			public int MidiChannel;

			[Tooltip("Midi Note [0-127]")]
			public int MidiNote;

			[Tooltip("OSC Address")]
			public string OscAddr;
		}

		public bool isHidden;

		public bool UsesAdvancedOptions;

		public bool ShowHelp = true;

		public bool TakesOwnershipIfNecessary;

		[SerializeField]
		public List<TriggerEvent> Triggers = new List<TriggerEvent>();

		public static Action<VRC_Trigger> InitializeTrigger;

		public Action<TriggerEvent> ExecuteTrigger;

		public int NetworkID
		{
			get;
			set;
		}

		public bool HasKeyTriggers => Triggers.Any((TriggerEvent t) => TypeCollections.KeyTypes.Contains(t.TriggerType));

		public bool HasTimerTriggers => Triggers.Any((TriggerEvent t) => TypeCollections.TimerTypes.Contains(t.TriggerType));

		public bool HasColliderTriggers => Triggers.Any((TriggerEvent t) => TypeCollections.ColliderTypes.Contains(t.TriggerType));

		public bool HasInteractiveTriggers => Triggers.Any((TriggerEvent t) => TypeCollections.InteractiveTypes.Contains(t.TriggerType));

		public bool HasPickupTriggers => Triggers.Any((TriggerEvent t) => TypeCollections.InteractiveTypes.Contains(t.TriggerType));

		public static void TriggerCustom(CustomTriggerTarget target)
		{
			if (target != null && target.TriggerObject != null && !string.IsNullOrEmpty(target.CustomName))
			{
				TriggerCustom(target.TriggerObject, target.CustomName);
			}
		}

		public static void Trigger(GameObject obj, TriggerType type)
		{
			if (!(obj == null) && InitializeTrigger != null)
			{
				VRC_Trigger component = obj.GetComponent<VRC_Trigger>();
				if (component != null)
				{
					InitializeTrigger(component);
					component.ExecuteTriggerType(type);
				}
			}
		}

		public static void TriggerCustom(GameObject obj, string name)
		{
			if (!(obj == null))
			{
				VRC_Trigger component = obj.GetComponent<VRC_Trigger>();
				if (component != null && InitializeTrigger != null)
				{
					InitializeTrigger(component);
					component.ExecuteCustomTrigger(name);
				}
			}
		}

		public override void Awake()
		{
			if (InitializeTrigger != null)
			{
				InitializeTrigger(this);
				base.Awake();
			}
			else
			{
				Object.Destroy(this);
			}
		}

		private void OnDestroy()
		{
			ExecuteTriggerType(TriggerType.OnDestroy);
		}

		private void OnEnable()
		{
			ExecuteTriggerType(TriggerType.OnEnable);
			ResetClocks();
		}

		private void OnDisable()
		{
			ExecuteTriggerType(TriggerType.OnDisable);
			ResetClocks();
		}

		private void OnDrawGizmosSelected()
		{
			List<VRC_Trigger> list = new List<VRC_Trigger>();
			List<VRC_Trigger> list2 = new List<VRC_Trigger>();
			list2.Add(this);
			List<VRC_Trigger> list3 = list2;
			Action<GameObject> action = delegate(GameObject obj)
			{
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				if (!(obj == null))
				{
					Gizmos.DrawLine(this.get_transform().get_position(), obj.get_transform().get_position());
				}
			};
			while (list3.Count > 0)
			{
				VRC_Trigger vRC_Trigger = list3.First();
				list3.RemoveAt(0);
				if (!(vRC_Trigger == null))
				{
					list.Add(vRC_Trigger);
					foreach (TriggerEvent trigger in vRC_Trigger.Triggers)
					{
						if (trigger != null)
						{
							foreach (VRC_EventHandler.VrcEvent @event in trigger.Events)
							{
								if (@event != null)
								{
									if (@event.ParameterObjects != null)
									{
										GameObject[] parameterObjects = @event.ParameterObjects;
										foreach (GameObject obj2 in parameterObjects)
										{
											action(obj2);
										}
									}
									if (@event.ParameterObject != null)
									{
										action(@event.ParameterObject);
									}
								}
							}
						}
					}
				}
			}
		}

		public override void Interact()
		{
			ExecuteTriggerType(TriggerType.OnInteract);
		}

		public void ExecuteTriggers(IEnumerable<TriggerEvent> triggers)
		{
			foreach (TriggerEvent trigger in triggers)
			{
				ExecuteTrigger(trigger);
			}
		}

		public void ExecuteTriggerType(TriggerType triggerType)
		{
			IEnumerable<TriggerEvent> enumerable = from t in Triggers
			where t != null && t.TriggerType == triggerType
			select t;
			if (enumerable.FirstOrDefault() != null)
			{
				foreach (TriggerEvent item in enumerable)
				{
					ExecuteTrigger(item);
				}
			}
		}

		public void ExecuteCustomTrigger(string name)
		{
			IEnumerable<TriggerEvent> enumerable = from t in Triggers
			where t != null && t.TriggerType == TriggerType.Custom && t.Name == name
			select t;
			if (enumerable.FirstOrDefault() != null)
			{
				foreach (TriggerEvent item in enumerable)
				{
					ExecuteTrigger(item);
				}
			}
		}

		private void ResetClocks()
		{
			foreach (TriggerEvent item in from t in Triggers
			where t.TriggerType == TriggerType.OnTimer && t.ResetOnEnable
			select t)
			{
				ResetClock(item);
			}
		}

		public void ResetClock(TriggerEvent timer)
		{
			timer.Duration = timer.LowPeriodTime + Random.get_value() * (timer.HighPeriodTime - timer.LowPeriodTime);
			timer.Timer = 0f;
			timer.EventFired = false;
		}
	}
}
