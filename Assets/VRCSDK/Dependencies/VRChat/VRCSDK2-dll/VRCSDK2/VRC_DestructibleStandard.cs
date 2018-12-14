using System.Linq;
using UnityEngine;

namespace VRCSDK2
{
	[RequireComponent(typeof(AudioSource))]
	public class VRC_DestructibleStandard : MonoBehaviour, IVRC_Destructible
	{
		public float maxHealth = 100f;

		public float currentHealth = 100f;

		public bool healable;

		public string damageEvent;

		public string destroyEvent;

		public string healEvent;

		public string fullHealthEvent;

		public VRC_Trigger onDamagedTrigger;

		public VRC_EventHandler.VrcEvent onDamagedEvent;

		public VRC_Trigger onDestructedTrigger;

		public VRC_EventHandler.VrcEvent onDestructedEvent;

		public VRC_Trigger onHealedTrigger;

		public VRC_EventHandler.VrcEvent onHealedEvent;

		public VRC_Trigger onFullHealedTrigger;

		public VRC_EventHandler.VrcEvent onFullHealedEvent;

		public VRC_Trigger spawnerOnDamagedTrigger;

		public VRC_EventHandler.VrcEvent spawnerOnDamagedEvent;

		public VRC_Trigger spawnerOnDestructedTrigger;

		public VRC_EventHandler.VrcEvent spawnerOnDestructedEvent;

		public VRC_Trigger spawnerOnHealedTrigger;

		public VRC_EventHandler.VrcEvent spawnerOnHealedEvent;

		public VRC_Trigger spawnerOnFullHealedTrigger;

		public VRC_EventHandler.VrcEvent spawnerOnFullHealedEvent;

		private VRC_EventHandler eventHandler;

		public VRC_DestructibleStandard()
			: this()
		{
		}

		public float GetMaxHealth()
		{
			return maxHealth;
		}

		public float GetCurrentHealth()
		{
			return currentHealth;
		}

		private void Awake()
		{
			eventHandler = this.GetComponent<VRC_EventHandler>();
			if (eventHandler == null)
			{
				eventHandler = this.GetComponentInParent<VRC_EventHandler>();
			}
		}

		public void ApplyDamage(float damage)
		{
			if (currentHealth > 0f)
			{
				currentHealth -= damage;
				if (currentHealth <= 0f)
				{
					currentHealth = 0f;
				}
				if (!string.IsNullOrEmpty(damageEvent) && eventHandler != null)
				{
					foreach (VRC_EventHandler.VrcEvent item in from e in eventHandler.Events
					where e.Name == damageEvent
					select e)
					{
						eventHandler.TriggerEvent(item, VRC_EventHandler.VrcBroadcastType.AlwaysUnbuffered);
					}
				}
				HandleCallbackTrigger(onDamagedTrigger, onDamagedEvent);
				HandleCallbackTrigger(spawnerOnDamagedTrigger, spawnerOnDamagedEvent);
				if (currentHealth == 0f)
				{
					if (!string.IsNullOrEmpty(destroyEvent) && eventHandler != null)
					{
						foreach (VRC_EventHandler.VrcEvent item2 in from e in eventHandler.Events
						where e.Name == destroyEvent
						select e)
						{
							eventHandler.TriggerEvent(item2, VRC_EventHandler.VrcBroadcastType.AlwaysUnbuffered);
						}
					}
					HandleCallbackTrigger(onDestructedTrigger, onDestructedEvent);
					HandleCallbackTrigger(spawnerOnDestructedTrigger, spawnerOnDestructedEvent);
				}
			}
		}

		public void ApplyHealing(float healing)
		{
			Debug.Log((object)"ApplyHealing");
			if (healable && currentHealth < maxHealth)
			{
				currentHealth += healing;
				if (currentHealth >= maxHealth)
				{
					currentHealth = maxHealth;
				}
				if (!string.IsNullOrEmpty(healEvent) && eventHandler != null)
				{
					foreach (VRC_EventHandler.VrcEvent item in from e in eventHandler.Events
					where e.Name == destroyEvent
					select e)
					{
						eventHandler.TriggerEvent(item, VRC_EventHandler.VrcBroadcastType.AlwaysUnbuffered);
					}
				}
				HandleCallbackTrigger(onHealedTrigger, onHealedEvent);
				HandleCallbackTrigger(spawnerOnHealedTrigger, spawnerOnHealedEvent);
				if (currentHealth == maxHealth)
				{
					if (!string.IsNullOrEmpty(fullHealthEvent) && eventHandler != null)
					{
						foreach (VRC_EventHandler.VrcEvent item2 in from e in eventHandler.Events
						where e.Name == fullHealthEvent
						select e)
						{
							eventHandler.TriggerEvent(item2, VRC_EventHandler.VrcBroadcastType.AlwaysUnbuffered);
						}
					}
					HandleCallbackTrigger(onFullHealedTrigger, onFullHealedEvent);
					HandleCallbackTrigger(spawnerOnFullHealedTrigger, spawnerOnFullHealedEvent);
				}
			}
		}

		public object[] GetState()
		{
			return new object[1]
			{
				currentHealth
			};
		}

		public void SetState(object[] state)
		{
			currentHealth = (float)state[0];
		}

		private void HandleCallbackTrigger(VRC_Trigger trigger, VRC_EventHandler.VrcEvent e)
		{
			if (trigger != null && !string.IsNullOrEmpty(e.ParameterString))
			{
				VRC_Trigger.TriggerCustom(trigger.get_gameObject(), e.ParameterString);
			}
		}
	}
}
