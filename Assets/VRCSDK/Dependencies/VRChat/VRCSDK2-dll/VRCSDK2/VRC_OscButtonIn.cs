using UnityEngine;

namespace VRCSDK2
{
	public class VRC_OscButtonIn : MonoBehaviour
	{
		public delegate void InitializationDelegate(VRC_OscButtonIn obj);

		public string address;

		public VRC_Trigger.CustomTriggerTarget OnButtonOn;

		public VRC_Trigger.CustomTriggerTarget OnButtonOff;

		public static InitializationDelegate Initialize;

		public VRC_OscButtonIn()
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

		private void Start()
		{
		}

		private void Update()
		{
		}
	}
}
