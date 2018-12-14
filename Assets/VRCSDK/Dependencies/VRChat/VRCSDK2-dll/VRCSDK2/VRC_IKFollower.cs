using UnityEngine;

namespace VRCSDK2
{
	public class VRC_IKFollower : MonoBehaviour
	{
		public delegate void InitializationDelegate(VRC_IKFollower obj);

		public static InitializationDelegate Initialize;

		public VRC_IKFollower()
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
	}
}
