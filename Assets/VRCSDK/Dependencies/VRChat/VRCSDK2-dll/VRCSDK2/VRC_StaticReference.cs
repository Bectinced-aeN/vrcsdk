using UnityEngine;

namespace VRCSDK2
{
	public class VRC_StaticReference : MonoBehaviour
	{
		public VRC_StaticReference()
			: this()
		{
		}

		public void Apply()
		{
			MarkStatic(this.get_gameObject());
			StaticBatchingUtility.Combine(this.get_gameObject());
		}

		private void MarkStatic(GameObject go)
		{
			go.set_isStatic(true);
			for (int i = 0; i < go.get_transform().get_childCount(); i++)
			{
				MarkStatic(go.get_transform().GetChild(i).get_gameObject());
			}
		}
	}
}
