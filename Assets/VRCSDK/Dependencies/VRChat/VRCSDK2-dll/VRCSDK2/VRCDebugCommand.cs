using UnityEngine;

namespace VRCSDK2
{
	public class VRCDebugCommand : VRC_Behaviour
	{
		public delegate void AwakeDelegate(VRCDebugCommand obj);

		public string Command;

		public int ParamInt;

		public float ParamFloat;

		public string ParamString;

		public Object ParamObject;

		public static AwakeDelegate OnAwake;

		private void Awake()
		{
			if (OnAwake != null)
			{
				OnAwake(this);
			}
		}
	}
}
