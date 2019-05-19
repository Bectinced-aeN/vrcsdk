using System;
using System.Collections.Generic;
using UnityEngine;

namespace VRCSDK2
{
	public class VRCTriggerRelay : MonoBehaviour
	{
		public static Action<VRCTriggerRelay> Initialize;

		[HideInInspector]
		public HashSet<Collider>[] triggersInside = new HashSet<Collider>[2]
		{
			new HashSet<Collider>(),
			new HashSet<Collider>()
		};

		[HideInInspector]
		public int currentIdx;

		public VRCTriggerRelay()
			: this()
		{
		}

		private void Awake()
		{
			if (Initialize != null)
			{
				Initialize(this);
			}
		}

		private void OnTriggerEnter(Collider col)
		{
			triggersInside[currentIdx].Add(col);
		}

		private void OnTriggerExit(Collider col)
		{
			triggersInside[currentIdx].Remove(col);
		}
	}
}
