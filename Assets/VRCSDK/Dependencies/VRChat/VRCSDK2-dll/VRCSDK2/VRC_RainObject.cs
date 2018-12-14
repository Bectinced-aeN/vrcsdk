using UnityEngine;

namespace VRCSDK2
{
	public class VRC_RainObject : MonoBehaviour
	{
		public delegate void InitializationDelegate(VRC_RainObject obj);

		public static InitializationDelegate Initialize;

		public MonoBehaviour attackComponent;

		public VRC_RainObject()
			: this()
		{
		}

		private void Start()
		{
			if (Initialize != null)
			{
				Initialize(this);
			}
		}

		private void VrcAnimationEvent(string eventName)
		{
			if (eventName == "Attack" && attackComponent != null)
			{
				attackComponent.SendMessage("GenerateAttack", (object)this.get_transform(), 0);
			}
		}
	}
}
