using System;
using UnityEngine;

namespace VRCSDK2
{
	[Obsolete("Please use VRC_Trigger", false)]
	public class VRC_TriggerColliderEventTrigger : MonoBehaviour
	{
		public delegate void CollisionEnterDelegate(VRC_TriggerColliderEventTrigger obj, Collider other);

		public delegate void CollisionExitDelegate(VRC_TriggerColliderEventTrigger obj, Collider other);

		public string EnterEventName;

		public string ExitEventName;

		public bool TriggerIndividuals = true;

		public LayerMask layers = LayerMask.op_Implicit(512);

		[HideInInspector]
		public VRC_EventHandler Handler;

		public static CollisionEnterDelegate CollisionEnter;

		public static CollisionExitDelegate CollisionExit;

		private int currentOccupants;

		public VRC_TriggerColliderEventTrigger()
			: this()
		{
		}//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)


		private void Start()
		{
			Handler = this.GetComponent<VRC_EventHandler>();
			if (Handler == null)
			{
				Handler = this.GetComponentInParent<VRC_EventHandler>();
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			int num = 1 << other.get_gameObject().get_layer();
			if ((layers.get_value() & num) != 0)
			{
				currentOccupants++;
				if ((currentOccupants == 1 || TriggerIndividuals) && CollisionEnter != null)
				{
					CollisionEnter(this, other);
				}
			}
		}

		private void OnTriggerExit(Collider other)
		{
			int num = 1 << other.get_gameObject().get_layer();
			if ((layers.get_value() & num) != 0)
			{
				if (currentOccupants > 0)
				{
					currentOccupants--;
				}
				if ((currentOccupants == 0 || TriggerIndividuals) && CollisionExit != null)
				{
					CollisionExit(this, other);
				}
			}
		}
	}
}
