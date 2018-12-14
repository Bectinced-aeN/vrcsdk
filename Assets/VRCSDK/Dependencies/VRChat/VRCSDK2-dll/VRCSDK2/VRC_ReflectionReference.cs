using UnityEngine;

namespace VRCSDK2
{
	public class VRC_ReflectionReference : MonoBehaviour
	{
		public Texture bakedTexture;

		public VRC_ReflectionReference()
			: this()
		{
		}

		public void Apply()
		{
			ReflectionProbe component = this.GetComponent<ReflectionProbe>();
			if (component != null)
			{
				component.set_bakedTexture(bakedTexture);
			}
		}
	}
}
