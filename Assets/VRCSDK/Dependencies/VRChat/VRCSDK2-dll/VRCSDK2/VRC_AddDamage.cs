using System.Collections.Generic;
using UnityEngine;

namespace VRCSDK2
{
	public class VRC_AddDamage : MonoBehaviour, IVRCEventProvider
	{
		public delegate void InitializationDelegate(VRC_AddDamage obj);

		public float damageAmount = 1f;

		public static InitializationDelegate Initialize;

		public VRC_AddDamage()
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

		public IEnumerable<VRC_EventHandler.VrcEvent> ProvideEvents()
		{
			List<VRC_EventHandler.VrcEvent> list = new List<VRC_EventHandler.VrcEvent>();
			VRC_EventHandler.VrcEvent vrcEvent = new VRC_EventHandler.VrcEvent();
			vrcEvent.Name = "AddDamage";
			vrcEvent.EventType = VRC_EventHandler.VrcEventType.SendRPC;
			vrcEvent.ParameterString = "AddDamage";
			vrcEvent.ParameterObjects = (GameObject[])new GameObject[1]
			{
				this.get_gameObject()
			};
			list.Add(vrcEvent);
			return list;
		}
	}
}
