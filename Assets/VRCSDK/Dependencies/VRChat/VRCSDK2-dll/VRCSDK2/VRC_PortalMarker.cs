using UnityEngine;

namespace VRCSDK2
{
	public class VRC_PortalMarker : MonoBehaviour
	{
		public enum SortHeading
		{
			Featured,
			Trending,
			Updated,
			Created,
			Active
		}

		public enum SortOrder
		{
			Ascending,
			Descending
		}

		public enum VRChatWorld
		{
			None,
			Hub
		}

		public delegate void InitializationDelegate(VRC_PortalMarker obj);

		public VRChatWorld world;

		public string roomId;

		public SortHeading sortHeading;

		public SortOrder sortOrder;

		public int offset;

		public string searchTerm;

		public string tag;

		public bool useDefaultPresentation;

		public string effectPrefabName;

		public static InitializationDelegate Initialize;

		public VRC_PortalMarker()
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
