using System;
using System.Collections;
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
			OnStationExited
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
		}

		private class OccupantInfo
		{
			public Collider collider;

			public float time;
		}

		public bool isHidden;

		public bool UsesAdvancedOptions;

		public bool ShowHelp = true;

		[SerializeField]
		public List<TriggerEvent> Triggers = new List<TriggerEvent>();

		private VRC_EventHandler _handler;

		private VRC_DataStorage dataStorage;

		private Collider collider;

		private HashSet<OccupantInfo> occupants = new HashSet<OccupantInfo>();

		private HashSet<Collider> stayOccupants = new HashSet<Collider>();

		private GameObject _localUser;

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

		public VRC_EventHandler Handler
		{
			get
			{
				if (_handler == null)
				{
					_handler = this.GetComponent<VRC_EventHandler>();
					if (_handler == null)
					{
						_handler = this.GetComponentInParent<VRC_EventHandler>();
					}
				}
				return _handler;
			}
		}

		private GameObject LocalUser
		{
			get
			{
				if (_localUser == null && Networking.LocalPlayer != null)
				{
					_localUser = Networking.LocalPlayer.get_gameObject();
				}
				return _localUser;
			}
		}

		private event Action deferredForReady;

		public static void TriggerCustom(CustomTriggerTarget target)
		{
			if (target != null && target.TriggerObject != null && !string.IsNullOrEmpty(target.CustomName))
			{
				TriggerCustom(target.TriggerObject, target.CustomName);
			}
		}

		public static void Trigger(GameObject obj, TriggerType type)
		{
			if (!(obj == null))
			{
				VRC_Trigger component = obj.GetComponent<VRC_Trigger>();
				if (component != null)
				{
					component.ExecuteTriggerType(type);
				}
			}
		}

		public static void TriggerCustom(GameObject obj, string name)
		{
			if (!(obj == null))
			{
				VRC_Trigger component = obj.GetComponent<VRC_Trigger>();
				if (component != null)
				{
					component.ExecuteCustomTrigger(name);
				}
			}
		}

		public void FixWeirdActions()
		{
			foreach (TriggerEvent trigger in Triggers)
			{
				foreach (VRC_EventHandler.VrcEvent @event in trigger.Events)
				{
					if (@event.ParameterObject != null)
					{
						if (@event.ParameterObjects != null)
						{
							if (!@event.ParameterObjects.Contains(@event.ParameterObject))
							{
								GameObject[] array = (GameObject[])new GameObject[@event.ParameterObjects.Length + 1];
								for (int i = 0; i < @event.ParameterObjects.Length; i++)
								{
									array[i] = @event.ParameterObjects[i];
								}
								array[array.Length - 1] = @event.ParameterObject;
								@event.ParameterObjects = array;
							}
						}
						else
						{
							@event.ParameterObjects = (GameObject[])new GameObject[1]
							{
								@event.ParameterObject
							};
						}
						@event.ParameterObject = null;
					}
					if (@event.ParameterObjects == null && @event.EventType == VRC_EventHandler.VrcEventType.SpawnObject)
					{
						@event.ParameterObjects = (GameObject[])new GameObject[1]
						{
							this.get_gameObject()
						};
					}
				}
			}
		}

		public override void Awake()
		{
			base.Awake();
			if (isHidden)
			{
				this.get_gameObject().set_layer(LayerMask.NameToLayer("Default"));
			}
			FixWeirdActions();
		}

		public override void Start()
		{
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			collider = this.GetComponent<Collider>();
			if (Handler == null)
			{
				Debug.LogError((object)("VRC_Trigger on " + this.get_gameObject().get_name() + " requires an Event Handler; destroying it."));
				Object.Destroy(this);
			}
			else
			{
				Handler.Events = new List<VRC_EventHandler.VrcEvent>();
				if (interactTextPlacement != null && interactTextGO != null)
				{
					interactTextGO.get_transform().set_position(interactTextPlacement.get_position());
				}
				foreach (TriggerEvent item in from t in Triggers
				where t.TriggerType == TriggerType.OnTimer
				select t)
				{
					ResetClock(item);
				}
				foreach (TriggerEvent item2 in from t in Triggers
				where t.TriggerType == TriggerType.Custom
				select t)
				{
					foreach (VRC_EventHandler.VrcEvent @event in item2.Events)
					{
						@event.Name = item2.Name;
					}
					if (item2.DataStorageShadowValues == null)
					{
						item2.DataStorageShadowValues = new List<DataStorageShadow>();
					}
					while (item2.DataStorageShadowValues.Count < item2.Events.Count)
					{
						item2.DataStorageShadowValues.Add(null);
					}
					while (item2.DataStorageShadowValues.Count > item2.Events.Count)
					{
						item2.DataStorageShadowValues.RemoveAt(item2.DataStorageShadowValues.Count - 1);
					}
				}
				dataStorage = this.get_gameObject().GetComponent<VRC_DataStorage>();
				if (dataStorage != null)
				{
					dataStorage.ElementChanged += OnDataStorageChanged;
					dataStorage.ElementAdded += OnDataStorageAdded;
					dataStorage.ElementRemoved += OnDataStorageRemoved;
				}
			}
		}

		private void OnDataStorageChanged(VRC_DataStorage ds, int idx)
		{
			foreach (TriggerEvent item in from t in Triggers
			where t.TriggerType == TriggerType.OnDataStorageChange && (t.DataElementIdx == idx || t.DataElementIdx == -1)
			select t)
			{
				ExecuteTrigger(item);
			}
		}

		private void OnDataStorageAdded(VRC_DataStorage ds, int idx)
		{
			foreach (TriggerEvent item in from t in Triggers
			where t.TriggerType == TriggerType.OnDataStorageAdd && (t.DataElementIdx == idx || t.DataElementIdx == -1)
			select t)
			{
				ExecuteTrigger(item);
			}
		}

		private void OnDataStorageRemoved(VRC_DataStorage ds, int idx)
		{
			foreach (TriggerEvent item in from t in Triggers
			where t.TriggerType == TriggerType.OnDataStorageRemove && (t.DataElementIdx == idx || t.DataElementIdx == -1)
			select t)
			{
				ExecuteTrigger(item);
			}
		}

		private void OnNetworkReady()
		{
			ExecuteTriggerType(TriggerType.OnNetworkReady);
			if (this.deferredForReady != null)
			{
				this.deferredForReady();
			}
		}

		private void OnSpawn()
		{
			ExecuteTriggerType(TriggerType.OnSpawn);
		}

		private void OnDestroy()
		{
			ExecuteTriggerType(TriggerType.OnDestroy);
		}

		private void OnPlayerJoined(VRC_PlayerApi player)
		{
			ExecuteTriggerType(TriggerType.OnPlayerJoined);
		}

		private void OnPlayerLeft(VRC_PlayerApi player)
		{
			ExecuteTriggerType(TriggerType.OnPlayerLeft);
		}

		private void OnEnable()
		{
			foreach (TriggerEvent item in from t in Triggers
			where t.TriggerType == TriggerType.OnTimer && t.ResetOnEnable
			select t)
			{
				ResetClock(item);
			}
			ExecuteTriggerType(TriggerType.OnEnable);
		}

		private void OnDrawGizmos()
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
					VRC_Trigger component = obj.GetComponent<VRC_Trigger>();
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

		private void Update()
		{
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			bool flag = false;
			for (int i = 0; i < Triggers.Count; i++)
			{
				TriggerEvent triggerEvent = Triggers[i];
				switch (triggerEvent.TriggerType)
				{
				case TriggerType.OnKeyDown:
					if (Input.GetKeyDown(triggerEvent.Key))
					{
						ExecuteTrigger(triggerEvent);
					}
					break;
				case TriggerType.OnKeyUp:
					if (Input.GetKeyUp(triggerEvent.Key))
					{
						ExecuteTrigger(triggerEvent);
					}
					break;
				case TriggerType.OnTimer:
					if (!triggerEvent.EventFired)
					{
						triggerEvent.Timer += Time.get_deltaTime();
						if (triggerEvent.Timer > triggerEvent.Duration)
						{
							ExecuteTrigger(triggerEvent);
							if (triggerEvent.Repeat)
							{
								ResetClock(triggerEvent);
							}
							else
							{
								triggerEvent.EventFired = true;
							}
						}
					}
					break;
				case TriggerType.OnEnterTrigger:
				case TriggerType.OnExitTrigger:
				case TriggerType.OnEnterCollider:
				case TriggerType.OnExitCollider:
					flag = true;
					break;
				}
			}
			if (flag)
			{
				CheckTriggerStay();
			}
		}

		private void CheckTriggerStay()
		{
			if (!(collider == null) && ((occupants != null && occupants.Count != 0) || (stayOccupants != null && stayOccupants.Count != 0)))
			{
				using (HashSet<OccupantInfo>.Enumerator enumerator = occupants.GetEnumerator())
				{
					OccupantInfo occupant;
					while (enumerator.MoveNext())
					{
						occupant = enumerator.Current;
						if (occupant.collider == null)
						{
							if (collider.get_isTrigger())
							{
								ExecuteTriggerType(TriggerType.OnExitTrigger);
							}
							else
							{
								ExecuteTriggerType(TriggerType.OnExitCollider);
							}
						}
						else if (stayOccupants.Contains(occupant.collider))
						{
							occupant.time = Time.get_time();
							stayOccupants.RemoveWhere((Collider o) => o == occupant.collider);
						}
						else if (!(Time.get_time() - occupant.time < 0.5f))
						{
							if (collider.get_isTrigger())
							{
								ExecuteTriggerType(TriggerType.OnExitTrigger);
							}
							else
							{
								ExecuteTriggerType(TriggerType.OnExitCollider);
							}
							occupant.collider = null;
						}
					}
				}
				occupants.RemoveWhere((OccupantInfo i) => i.collider == null);
				stayOccupants.Clear();
			}
		}

		public override void Interact()
		{
			ExecuteTriggerType(TriggerType.OnInteract);
		}

		public void OnStationEntered()
		{
			ExecuteTriggerType(TriggerType.OnStationEntered);
		}

		public void OnStationExited()
		{
			ExecuteTriggerType(TriggerType.OnStationExited);
		}

		private void OnTriggerEnter(Collider other)
		{
			int maskable = 1 << other.get_gameObject().get_layer();
			if (Triggers.Any((TriggerEvent t) => t.TriggerType == TriggerType.OnEnterTrigger && 0 != (LayerMask.op_Implicit(t.Layers) & maskable)))
			{
				occupants.Add(new OccupantInfo
				{
					collider = other,
					time = Time.get_time()
				});
				stayOccupants.RemoveWhere((Collider c) => c == other);
				int currentOccupants = occupants.Count;
				ExecuteTriggers(from t in Triggers
				where (t.TriggerIndividuals || currentOccupants == 1) && t.TriggerType == TriggerType.OnEnterTrigger && 0 != (LayerMask.op_Implicit(t.Layers) & maskable)
				select t);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			int maskable = 1 << other.get_gameObject().get_layer();
			if (Triggers.Any((TriggerEvent t) => t.TriggerType == TriggerType.OnExitTrigger && 0 != (LayerMask.op_Implicit(t.Layers) & maskable)))
			{
				occupants.RemoveWhere((OccupantInfo o) => o.collider == other);
				stayOccupants.RemoveWhere((Collider c) => c == other);
				int currentOccupants = occupants.Count;
				ExecuteTriggers(from t in Triggers
				where (t.TriggerIndividuals || currentOccupants == 0) && t.TriggerType == TriggerType.OnExitTrigger && 0 != (LayerMask.op_Implicit(t.Layers) & maskable)
				select t);
			}
		}

		private IEnumerator OnTriggerStay(Collider other)
		{
			yield return (object)new WaitForFixedUpdate();
			stayOccupants.Add(other);
		}

		private void OnControllerColliderHit(ControllerColliderHit hit)
		{
			ExecuteTriggers(from t in Triggers
			where t.TriggerType == TriggerType.OnAvatarHit
			select t);
		}

		private void OnCollisionEnter(Collision other)
		{
			int maskable = 1 << other.get_gameObject().get_layer();
			if (Triggers.Any((TriggerEvent t) => t.TriggerType == TriggerType.OnEnterCollider && 0 != (LayerMask.op_Implicit(t.Layers) & maskable)))
			{
				occupants.Add(new OccupantInfo
				{
					collider = other.get_collider(),
					time = Time.get_time()
				});
				stayOccupants.RemoveWhere((Collider c) => c == other.get_collider());
				int currentOccupants = occupants.Count;
				ExecuteTriggers(from t in Triggers
				where (t.TriggerIndividuals || currentOccupants == 1) && t.TriggerType == TriggerType.OnEnterCollider && 0 != (LayerMask.op_Implicit(t.Layers) & maskable)
				select t);
			}
		}

		private void OnCollisionExit(Collision other)
		{
			int maskable = 1 << other.get_gameObject().get_layer();
			if (Triggers.Any((TriggerEvent t) => t.TriggerType == TriggerType.OnExitCollider && 0 != (LayerMask.op_Implicit(t.Layers) & maskable)))
			{
				occupants.RemoveWhere((OccupantInfo o) => o.collider == other.get_collider());
				stayOccupants.RemoveWhere((Collider c) => c == other.get_collider());
				int currentOccupants = occupants.Count;
				ExecuteTriggers(from t in Triggers
				where (t.TriggerIndividuals || currentOccupants == 0) && t.TriggerType == TriggerType.OnExitCollider && 0 != (LayerMask.op_Implicit(t.Layers) & maskable)
				select t);
			}
		}

		private IEnumerator OnCollisionStay(Collision other)
		{
			yield return (object)new WaitForFixedUpdate();
			stayOccupants.Add(other.get_collider());
		}

		private void ResetClock(TriggerEvent timer)
		{
			timer.Duration = timer.LowPeriodTime + Random.get_value() * (timer.HighPeriodTime - timer.LowPeriodTime);
			timer.Timer = 0f;
			timer.EventFired = false;
		}

		private void ExecuteTriggers(IEnumerable<TriggerEvent> triggers)
		{
			foreach (TriggerEvent trigger in triggers)
			{
				ExecuteTrigger(trigger);
			}
		}

		private void ExecuteTriggerType(TriggerType triggerType)
		{
			foreach (TriggerEvent item in from t in Triggers
			where t.TriggerType == triggerType
			select t)
			{
				ExecuteTrigger(item);
			}
		}

		private void ExecuteCustomTrigger(string name)
		{
			foreach (TriggerEvent item in from t in Triggers
			where t.TriggerType == TriggerType.Custom && t.Name == name
			select t)
			{
				ExecuteTrigger(item);
			}
		}

		private void ExecuteTrigger(TriggerEvent trigger)
		{
			if (trigger != null && !(this == null))
			{
				if ((trigger.TriggerType == TriggerType.OnKeyDown || trigger.TriggerType == TriggerType.OnKeyUp) && (this.GetComponentInParent<VRC_PlayerApi>() != null || this.GetComponent<VRC_PlayerApi>() != null))
				{
					Debug.LogError((object)"Cannot execute key triggers on non-player objects.");
				}
				else if (!Networking.IsObjectReady(this.get_gameObject()) || Networking.SceneEventHandler == null)
				{
					this.deferredForReady = (Action)Delegate.Combine(this.deferredForReady, (Action)delegate
					{
						ExecuteTrigger(trigger);
					});
				}
				else
				{
					Networking.SceneEventHandler.StartCoroutine(ExecutionIterator(trigger));
				}
			}
		}

		private IEnumerator ExecutionIterator(TriggerEvent trigger)
		{
			if (trigger.AfterSeconds > 0f)
			{
				yield return (object)new WaitForSeconds(trigger.AfterSeconds);
			}
			float[] probabilities = trigger.Probabilities;
			float randomVal = Random.Range(0f, 0.99f);
			float randomAccum = 0f;
			for (int idx = 0; idx < trigger.Events.Count; idx++)
			{
				if (probabilities != null && probabilities.Length == trigger.Events.Count && probabilities[idx] + randomAccum < randomVal)
				{
					randomAccum += probabilities[idx];
				}
				else
				{
					VRC_EventHandler.VrcEvent evt = trigger.Events[idx];
					if (dataStorage != null && trigger.DataStorageShadowValues != null && trigger.DataStorageShadowValues.Count == trigger.Events.Count)
					{
						DataStorageShadow shadow = trigger.DataStorageShadowValues[idx];
						if (shadow.ParameterBoolOp != null)
						{
							VRC_DataStorage.VrcDataElement el4 = dataStorage.data.FirstOrDefault((VRC_DataStorage.VrcDataElement e) => e.name == ((_003CExecutionIterator_003Ec__Iterator4)/*Error near IL_019f: stateMachine*/)._003Cshadow_003E__5.ParameterBoolOp);
							if (el4 != null)
							{
								evt.ParameterBoolOp = (el4.valueBool ? VRC_EventHandler.VrcBooleanOp.True : VRC_EventHandler.VrcBooleanOp.False);
							}
						}
						if (shadow.ParameterFloat != null)
						{
							VRC_DataStorage.VrcDataElement el3 = dataStorage.data.FirstOrDefault((VRC_DataStorage.VrcDataElement e) => e.name == ((_003CExecutionIterator_003Ec__Iterator4)/*Error near IL_0203: stateMachine*/)._003Cshadow_003E__5.ParameterFloat);
							if (el3 != null)
							{
								evt.ParameterFloat = el3.valueFloat;
							}
						}
						if (shadow.ParameterInt != null)
						{
							VRC_DataStorage.VrcDataElement el2 = dataStorage.data.FirstOrDefault((VRC_DataStorage.VrcDataElement e) => e.name == ((_003CExecutionIterator_003Ec__Iterator4)/*Error near IL_025b: stateMachine*/)._003Cshadow_003E__5.ParameterInt);
							if (el2 != null)
							{
								evt.ParameterInt = el2.valueInt;
							}
						}
						if (shadow.ParameterString != null)
						{
							VRC_DataStorage.VrcDataElement el = dataStorage.data.FirstOrDefault((VRC_DataStorage.VrcDataElement e) => e.name == ((_003CExecutionIterator_003Ec__Iterator4)/*Error near IL_02b3: stateMachine*/)._003Cshadow_003E__5.ParameterString);
							if (el != null)
							{
								evt.ParameterString = el.valueString;
							}
						}
					}
					if (evt.ParameterObject == null && (evt.ParameterObjects == null || evt.ParameterObjects.Length == 0))
					{
						evt.ParameterObject = this.get_gameObject();
					}
					Debug.LogFormat("{0} triggered {1}", new object[2]
					{
						this.get_gameObject().get_name(),
						evt.EventType
					});
					Handler.TriggerEvent(evt, trigger.BroadcastType, LocalUser);
					if (probabilities != null && probabilities.Length == trigger.Events.Count)
					{
						break;
					}
				}
			}
			RelayTrigger(trigger);
		}

		private void RelayTrigger(TriggerEvent trigger)
		{
			foreach (TriggerEvent item in from t in Triggers
			where t.TriggerType == TriggerType.Relay
			select t)
			{
				foreach (VRC_Trigger item2 in item.Others.Where((VRC_Trigger t) => t != this && t != null))
				{
					if (trigger.TriggerType == TriggerType.Custom)
					{
						item2.ExecuteCustomTrigger(trigger.Name);
					}
					else
					{
						item2.ExecuteTriggerType(trigger.TriggerType);
					}
				}
			}
		}
	}
}
