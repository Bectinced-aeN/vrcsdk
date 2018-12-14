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

		private List<Collider> collidersToRemoveTemp = new List<Collider>(5);

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
			RemoveNullFromSet(triggersInside[0]);
			RemoveNullFromSet(triggersInside[1]);
			int num = (currentIdx + 1) % 2;
			bool flag = false;
			HashSet<Collider> hashSet = triggersInside[currentIdx];
			HashSet<Collider> hashSet2 = triggersInside[num];
			HashSet<Collider>.Enumerator enumerator = hashSet2.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Collider current = enumerator.Current;
				if (!hashSet.Contains(current))
				{
					Exit(current);
					flag = true;
				}
			}
			HashSet<Collider>.Enumerator enumerator2 = hashSet.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				Collider current2 = enumerator2.Current;
				if (!hashSet2.Contains(current2))
				{
					Enter(current2);
					flag = true;
				}
			}
			if (flag)
			{
				hashSet2.Clear();
				hashSet2.UnionWith(hashSet);
			}
			currentIdx = num;
		}

		private void RemoveNullFromSet(HashSet<Collider> colliders)
		{
			collidersToRemoveTemp.Clear();
			HashSet<Collider>.Enumerator enumerator = colliders.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Collider current = enumerator.Current;
				if (current == null || current.get_gameObject() == null)
				{
					collidersToRemoveTemp.Add(current);
				}
			}
			for (int i = 0; i < collidersToRemoveTemp.Count; i++)
			{
				colliders.Remove(collidersToRemoveTemp[i]);
			}
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
