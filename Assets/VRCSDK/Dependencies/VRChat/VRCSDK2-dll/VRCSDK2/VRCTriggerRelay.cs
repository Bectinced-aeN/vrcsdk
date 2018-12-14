using System.Collections.Generic;
using UnityEngine;

namespace VRCSDK2
{
	public class VRCTriggerRelay : MonoBehaviour
	{
		private HashSet<Collider>[] triggersInside = new HashSet<Collider>[2]
		{
			new HashSet<Collider>(),
			new HashSet<Collider>()
		};

		private int currentIdx;

		public VRCTriggerRelay()
			: this()
		{
		}

		private void OnTriggerEnter(Collider col)
		{
			triggersInside[currentIdx].Add(col);
		}

		private void OnTriggerExit(Collider col)
		{
			triggersInside[currentIdx].Remove(col);
		}

		private void Update()
		{
			triggersInside[0].RemoveWhere((Collider c) => c == null || c.get_gameObject() == null);
			triggersInside[1].RemoveWhere((Collider c) => c == null || c.get_gameObject() == null);
			int num = (currentIdx + 1) % 2;
			bool flag = false;
			foreach (Collider item in triggersInside[num])
			{
				if (!triggersInside[currentIdx].Contains(item))
				{
					Exit(item);
					flag = true;
				}
			}
			foreach (Collider item2 in triggersInside[currentIdx])
			{
				if (!triggersInside[num].Contains(item2))
				{
					Enter(item2);
					flag = true;
				}
			}
			if (flag)
			{
				triggersInside[num] = new HashSet<Collider>(triggersInside[currentIdx]);
			}
			currentIdx = num;
		}

		private void Enter(Collider col)
		{
			if (this.get_gameObject() != null && col != null)
			{
				this.get_gameObject().SendMessage("OnTriggerRelayEnter", (object)col, 1);
			}
			Collider val = this.GetComponent<Collider>();
			if (val == null)
			{
				val = this.GetComponentInParent<Collider>();
			}
			if (val != null && col != null && col.get_gameObject() != null)
			{
				col.get_gameObject().SendMessage("OnTriggerRelayEnter", (object)val, 1);
			}
		}

		private void Exit(Collider col)
		{
			if (this.get_gameObject() != null && col != null)
			{
				this.get_gameObject().SendMessage("OnTriggerRelayExit", (object)col, 1);
			}
			Collider val = this.GetComponent<Collider>();
			if (val == null)
			{
				val = this.GetComponentInParent<Collider>();
			}
			if (val != null && col != null && col.get_gameObject() != null)
			{
				col.get_gameObject().SendMessage("OnTriggerRelayExit", (object)val, 1);
			}
		}
	}
}
