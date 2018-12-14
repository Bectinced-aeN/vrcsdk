using UnityEngine;

namespace VRCSDK2
{
	public static class Tutorial
	{
		public delegate void ActivateObjectLabelDelegate(Transform targetObject, TutorialLabelType type, ControllerHand hand, string text, ControllerActionUI action, string textSecondary, ControllerActionUI actionSecondary, float duration, int priority, AttachMode attachMode, bool showOffscreen);

		public delegate void DeactivateObjectLabelDelegate(Transform targetObject);

		public delegate void ActivateControllerLabelDelegate(ControllerHand hand, ControllerInputUI controllerPart, string text, float duration, int priority);

		public delegate void DeactivateControllerLabelDelegate(ControllerHand hand, ControllerInputUI controllerPart);

		public static ActivateObjectLabelDelegate _ActivateObjectLabel;

		public static DeactivateObjectLabelDelegate _DeactivateObjectLabel;

		public static ActivateControllerLabelDelegate _ActivateControllerLabel;

		public static DeactivateControllerLabelDelegate _DeactivateControllerLabel;

		public static void ActivateAreaMarkerLabel(Transform targetObject, string text)
		{
			ActivateObjectLabel(targetObject, TutorialLabelType.AreaMarker, ControllerHand.None, text, -1f, 0, AttachMode.PositionOnly, showOffscreen: true);
		}

		public static void ActivateObjectLabel(Transform targetObject, TutorialLabelType type, ControllerHand hand, string text, float duration = 0.1f, int priority = 0, AttachMode attachMode = AttachMode.PositionOnly, bool showOffscreen = false)
		{
			ActivateObjectLabel(targetObject, type, hand, text, ControllerActionUI.None, string.Empty, ControllerActionUI.None, duration, priority, attachMode, showOffscreen);
		}

		public static void ActivateObjectLabel(Transform targetObject, TutorialLabelType type, ControllerHand hand, string text, ControllerActionUI action, float duration = 0.1f, int priority = 0, AttachMode attachMode = AttachMode.PositionOnly, bool showOffscreen = false)
		{
			ActivateObjectLabel(targetObject, type, hand, text, action, string.Empty, ControllerActionUI.None, duration, priority, attachMode, showOffscreen);
		}

		public static void ActivateObjectLabel(Transform targetObject, TutorialLabelType type, ControllerHand hand, string text, ControllerActionUI action, string textSecondary, ControllerActionUI actionSecondary, float duration, int priority, AttachMode attachMode, bool showOffscreen)
		{
			if (_ActivateObjectLabel != null)
			{
				_ActivateObjectLabel(targetObject, type, hand, text, action, textSecondary, actionSecondary, duration, priority, attachMode, showOffscreen);
			}
		}

		public static void DeactivateObjectLabel(Transform targetObject)
		{
			if (_DeactivateObjectLabel != null)
			{
				_DeactivateObjectLabel(targetObject);
			}
		}

		public static void ActivateControllerLabel(ControllerHand hand, ControllerInputUI controllerPart, string text, float duration, int priority)
		{
			if (_ActivateControllerLabel != null)
			{
				_ActivateControllerLabel(hand, controllerPart, text, duration, priority);
			}
		}

		public static void DeactivateControllerLabel(ControllerHand hand, ControllerInputUI controllerPart)
		{
			if (_DeactivateControllerLabel != null)
			{
				_DeactivateControllerLabel(hand, controllerPart);
			}
		}
	}
}
