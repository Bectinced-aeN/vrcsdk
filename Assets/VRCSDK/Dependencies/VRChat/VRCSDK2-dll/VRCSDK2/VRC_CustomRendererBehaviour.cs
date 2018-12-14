using UnityEngine;

namespace VRCSDK2
{
	public class VRC_CustomRendererBehaviour : MonoBehaviour
	{
		public bool UpdateGIMaterialsEveryTick = true;

		private Renderer _renderer;

		private Renderer Renderer
		{
			get
			{
				if (_renderer == null)
				{
					_renderer = this.get_gameObject().GetComponent<Renderer>();
				}
				return _renderer;
			}
		}

		public VRC_CustomRendererBehaviour()
			: this()
		{
		}

		private void Update()
		{
			if (!(Renderer == null) && UpdateGIMaterialsEveryTick)
			{
				RendererExtensions.UpdateGIMaterials(Renderer);
			}
		}
	}
}
