using System;
using UnityEngine;

namespace VRCSDK2
{
	public static class InputManager
	{
		public static Func<bool> _IsUsingHandController;

		public static Func<VRCInputMethod> _GetLastUsedInputMethod;

		public static Action<Renderer, bool> _EnableObjectHighlight;

		public static bool IsUsingHandController()
		{
			if (_IsUsingHandController != null)
			{
				return _IsUsingHandController();
			}
			return false;
		}

		public static VRCInputMethod GetLastUsedInputMethod()
		{
			if (_GetLastUsedInputMethod != null)
			{
				return _GetLastUsedInputMethod();
			}
			return VRCInputMethod.Count;
		}

		public static void EnableObjectHighlight(GameObject obj, bool enable)
		{
			if (obj != null)
			{
				EnableObjectHighlight(obj.GetComponent<Renderer>(), enable);
			}
		}

		public static void EnableObjectHighlight(Renderer r, bool enable)
		{
			if (_EnableObjectHighlight != null)
			{
				_EnableObjectHighlight(r, enable);
			}
		}
	}
}
