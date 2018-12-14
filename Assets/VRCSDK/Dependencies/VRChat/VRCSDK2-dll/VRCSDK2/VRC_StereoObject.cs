using UnityEngine;

namespace VRCSDK2
{
	public class VRC_StereoObject : MonoBehaviour
	{
		public enum Eye
		{
			Left,
			Right
		}

		public Eye eye;

		public VRC_StereoObject()
			: this()
		{
		}
	}
}
