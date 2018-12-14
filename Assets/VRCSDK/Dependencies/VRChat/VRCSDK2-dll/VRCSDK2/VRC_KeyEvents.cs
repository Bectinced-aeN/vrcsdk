using System;
using System.Linq;
using UnityEngine;

namespace VRCSDK2
{
	[Obsolete("Please use VRC_Trigger instead", false)]
	public class VRC_KeyEvents : MonoBehaviour
	{
		public delegate void InitializationDelegate(VRC_KeyEvents obj);

		public KeyCode Key;

		public string DownEventName;

		public string UpEventName;

		private bool LocalOnly;

		public VRC_EventHandler.VrcBroadcastType BroadcastType;

		private VRC_EventHandler Handler;

		public static InitializationDelegate Initialize;

		public VRC_KeyEvents()
			: this()
		{
		}

		private void Start()
		{
			if (Initialize != null)
			{
				Initialize(this);
			}
			Handler = this.GetComponent<VRC_EventHandler>();
			if (Handler == null)
			{
				Handler = this.GetComponentInParent<VRC_EventHandler>();
			}
		}

		private void Update()
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			if (LocalOnly)
			{
				BroadcastType = VRC_EventHandler.VrcBroadcastType.Local;
			}
			if (Input.GetKeyDown(Key) && DownEventName != string.Empty && Handler.Events.FirstOrDefault((VRC_EventHandler.VrcEvent e) => e.Name == DownEventName) != null)
			{
				foreach (VRC_EventHandler.VrcEvent item in from e in Handler.Events
				where e.Name == DownEventName
				select e)
				{
					Handler.TriggerEvent(item, BroadcastType, Networking.LocalPlayer.get_gameObject());
				}
			}
			if (Input.GetKeyUp(Key) && UpEventName != string.Empty && Handler.Events.FirstOrDefault((VRC_EventHandler.VrcEvent e) => e.Name == UpEventName) != null)
			{
				foreach (VRC_EventHandler.VrcEvent item2 in from e in Handler.Events
				where e.Name == UpEventName
				select e)
				{
					Handler.TriggerEvent(item2, BroadcastType, Networking.LocalPlayer.get_gameObject());
				}
			}
		}
	}
}
