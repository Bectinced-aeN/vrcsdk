using UnityEngine;

namespace VRCSDK2
{
	public class VRC_LightmapReference : MonoBehaviour
	{
		public int lightmapIndex;

		public Vector4 lightmapScaleOffset;

		public VRC_LightmapReference()
			: this()
		{
		}

		public void Apply()
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			Renderer component = this.GetComponent<Renderer>();
			if (component != null)
			{
				component.set_lightmapIndex(lightmapIndex);
				component.set_lightmapScaleOffset(lightmapScaleOffset);
			}
		}
	}
}
