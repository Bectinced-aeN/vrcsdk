using UnityEngine;

namespace VRCSDK2
{
	public class VRC_SyncAnimation : MonoBehaviour
	{
		public delegate void InitializationDelegate(VRC_SyncAnimation obj);

		public float AnimationStartPosition;

		public static InitializationDelegate Initialize;

		public VRC_SyncAnimation()
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
