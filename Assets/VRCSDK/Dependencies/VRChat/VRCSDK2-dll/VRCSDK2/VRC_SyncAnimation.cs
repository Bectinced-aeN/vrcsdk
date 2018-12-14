using UnityEngine;

namespace VRCSDK2
{
	public class VRC_SyncAnimation : MonoBehaviour, INetworkID
	{
		public delegate void InitializationDelegate(VRC_SyncAnimation obj);

		public float AnimationStartPosition;

		public static InitializationDelegate Initialize;

		[HideInInspector]
		public int NetworkID
		{
			get;
			set;
		}

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
