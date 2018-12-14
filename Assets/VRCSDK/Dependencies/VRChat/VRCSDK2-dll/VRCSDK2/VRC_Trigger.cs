using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VRCSDK2
{
	[RequireComponent(typeof(VRC_EventHandler))]
	public class VRC_Trigger : VRC_Interactable, IVRCEventSender
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
			OnAvatarHit
		}

		public static class TypeCollections
		{
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
		}

		private class OccupantInfo
		{
			public Collider collider;

			public float time;
		}

		public bool isHidden;

		[SerializeField]
		public List<TriggerEvent> Triggers = new List<TriggerEvent>();

		private List<OccupantInfo> occupants = new List<OccupantInfo>();

		private List<Collider> stayOccupants = new List<Collider>();

		private VRC_EventHandler _handler;

		private GameObject _localUser;

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
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			if (Handler == null)
			{
				Debug.LogError((object)("VRC_Trigger on " + this.get_gameObject().get_name() + " requires an Event Handler; destroying it."));
				Object.Destroy(this);
			}
			else
			{
				if (Handler.Events != null && Handler.Events.Count > 0)
				{
					Debug.LogWarning((object)("VRC_Trigger is clearing events from event handler on " + this.get_gameObject().get_name()));
					Handler.Events.Clear();
				}
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
				VRC_DataStorage component = this.get_gameObject().GetComponent<VRC_DataStorage>();
				if (component != null)
				{
					component.ElementChanged += OnDataStorageChanged;
					component.ElementAdded += OnDataStorageAdded;
					component.ElementRemoved += OnDataStorageRemoved;
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

		private void OnDrawGizmosSelected()
		{
			List<VRC_Trigger> processed = new List<VRC_Trigger>();
			List<VRC_Trigger> toProcess = new List<VRC_Trigger>
			{
				this
			};
			Action<GameObject> action = delegate(GameObject obj)
			{
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				if (!(obj == null))
				{
					Gizmos.DrawLine(this.get_transform().get_position(), obj.get_transform().get_position());
					VRC_Trigger component = obj.GetComponent<VRC_Trigger>();
					if (component != null && !processed.Contains(component))
					{
						toProcess.Add(component);
					}
				}
			};
			while (toProcess.Count > 0)
			{
				VRC_Trigger vRC_Trigger = toProcess.First();
				toProcess.RemoveAt(0);
				if (!(vRC_Trigger == null))
				{
					processed.Add(vRC_Trigger);
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
			if (Input.get_anyKeyDown())
			{
				ExecuteTriggers(from t in Triggers
				where (t.TriggerType == TriggerType.OnKeyDown && Input.GetKeyDown(t.Key)) || (t.TriggerType == TriggerType.OnKeyUp && Input.GetKeyUp(t.Key))
				select t);
			}
			ExecuteTriggers(UpdateTimers());
			CheckTriggerStay();
		}

		private void CheckTriggerStay()
		{
			if ((occupants != null && occupants.Count != 0) || (stayOccupants != null && stayOccupants.Count != 0))
			{
				Collider component = this.GetComponent<Collider>();
				OccupantInfo[] source = occupants.ToArray();
				using (IEnumerator<OccupantInfo> enumerator = (from o in source
				where o.collider != null
				select o).GetEnumerator())
				{
					OccupantInfo occupant;
					while (enumerator.MoveNext())
					{
						occupant = enumerator.Current;
						if (stayOccupants.Contains(occupant.collider))
						{
							occupant.time = Time.get_time();
							stayOccupants.RemoveAll((Collider o) => o == occupant.collider);
						}
						else if (!(Time.get_time() - occupant.time < 0.5f))
						{
							if (component.get_isTrigger())
							{
								ExecuteTriggerType(TriggerType.OnExitTrigger);
							}
							else
							{
								ExecuteTriggerType(TriggerType.OnExitCollider);
							}
							occupants.RemoveAll((OccupantInfo i) => i.collider == occupant.collider);
						}
					}
				}
				stayOccupants.Clear();
			}
		}

		public override void Interact()
		{
			ExecuteTriggerType(TriggerType.OnInteract);
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
				stayOccupants.RemoveAll((Collider c) => c == other);
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
				occupants.RemoveAll((OccupantInfo o) => o.collider == other);
				stayOccupants.RemoveAll((Collider c) => c == other);
				int currentOccupants = occupants.Count;
				ExecuteTriggers(from t in Triggers
				where (t.TriggerIndividuals || currentOccupants == 0) && t.TriggerType == TriggerType.OnExitTrigger && 0 != (LayerMask.op_Implicit(t.Layers) & maskable)
				select t);
			}
		}

		private IEnumerator OnTriggerStay(Collider other)
		{
			yield return (object)new WaitForFixedUpdate();
			if (!stayOccupants.Contains(other))
			{
				stayOccupants.Add(other);
			}
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
				stayOccupants.RemoveAll((Collider c) => c == other.get_collider());
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
				occupants.RemoveAll((OccupantInfo o) => o.collider == other.get_collider());
				stayOccupants.RemoveAll((Collider c) => c == other.get_collider());
				int currentOccupants = occupants.Count;
				ExecuteTriggers(from t in Triggers
				where (t.TriggerIndividuals || currentOccupants == 0) && t.TriggerType == TriggerType.OnExitCollider && 0 != (LayerMask.op_Implicit(t.Layers) & maskable)
				select t);
			}
		}

		private IEnumerator OnCollisionStay(Collision other)
		{
			yield return (object)new WaitForFixedUpdate();
			if (!stayOccupants.Contains(other.get_collider()))
			{
				stayOccupants.Add(other.get_collider());
			}
		}

		private IEnumerable<TriggerEvent> UpdateTimers()
		{
			List<TriggerEvent> list = new List<TriggerEvent>();
			foreach (TriggerEvent item in from t in Triggers
			where t.TriggerType == TriggerType.OnTimer && !t.EventFired
			select t)
			{
				item.Timer += Time.get_deltaTime();
				if (item.Timer > item.Duration)
				{
					list.Add(item);
					if (item.Repeat)
					{
						ResetClock(item);
					}
					else
					{
						item.EventFired = true;
					}
				}
			}
			return list;
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
			VRC_DataStorage component = this.get_gameObject().GetComponent<VRC_DataStorage>();
			for (int i = 0; i < trigger.Events.Count; i++)
			{
				VRC_EventHandler.VrcEvent vrcEvent = trigger.Events[i];
				if (trigger.DataStorageShadowValues != null && trigger.DataStorageShadowValues.Count == trigger.Events.Count && component != null)
				{
					DataStorageShadow shadow = trigger.DataStorageShadowValues[i];
					if (shadow.ParameterBoolOp != null)
					{
						VRC_DataStorage.VrcDataElement vrcDataElement = component.data.FirstOrDefault((VRC_DataStorage.VrcDataElement e) => e.name == shadow.ParameterBoolOp);
						if (vrcDataElement != null)
						{
							vrcEvent.ParameterBoolOp = (vrcDataElement.valueBool ? VRC_EventHandler.VrcBooleanOp.True : VRC_EventHandler.VrcBooleanOp.False);
						}
					}
					if (shadow.ParameterFloat != null)
					{
						VRC_DataStorage.VrcDataElement vrcDataElement2 = component.data.FirstOrDefault((VRC_DataStorage.VrcDataElement e) => e.name == shadow.ParameterFloat);
						if (vrcDataElement2 != null)
						{
							vrcEvent.ParameterFloat = vrcDataElement2.valueFloat;
						}
					}
					if (shadow.ParameterInt != null)
					{
						VRC_DataStorage.VrcDataElement vrcDataElement3 = component.data.FirstOrDefault((VRC_DataStorage.VrcDataElement e) => e.name == shadow.ParameterInt);
						if (vrcDataElement3 != null)
						{
							vrcEvent.ParameterInt = vrcDataElement3.valueInt;
						}
					}
					if (shadow.ParameterString != null)
					{
						VRC_DataStorage.VrcDataElement vrcDataElement4 = component.data.FirstOrDefault((VRC_DataStorage.VrcDataElement e) => e.name == shadow.ParameterString);
						if (vrcDataElement4 != null)
						{
							vrcEvent.ParameterString = vrcDataElement4.valueString;
						}
					}
				}
				Handler.TriggerEvent(vrcEvent, trigger.BroadcastType, LocalUser);
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
