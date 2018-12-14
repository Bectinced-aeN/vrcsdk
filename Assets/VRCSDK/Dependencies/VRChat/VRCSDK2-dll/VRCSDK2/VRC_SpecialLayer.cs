using UnityEngine;

namespace VRCSDK2
{
	public class VRC_SpecialLayer : VRC_Behaviour
	{
		public enum VRCLayer
		{
			Environment,
			Interactive,
			Walkthrough,
			Pickup
		}

		public VRCLayer specialLayer;

		public void Apply()
		{
			int num = -1;
			switch (specialLayer)
			{
			case VRCLayer.Environment:
				num = LayerMask.NameToLayer("Environment");
				break;
			case VRCLayer.Interactive:
				num = LayerMask.NameToLayer("Interactive");
				break;
			case VRCLayer.Walkthrough:
				num = ((!(this.get_gameObject().GetComponent<VRC_Pickup>() != null)) ? LayerMask.NameToLayer("Walkthrough") : LayerMask.NameToLayer("Pickup"));
				break;
			case VRCLayer.Pickup:
				num = LayerMask.NameToLayer("Pickup");
				break;
			}
			if (num > 0)
			{
				this.get_gameObject().set_layer(num);
			}
		}
	}
}
