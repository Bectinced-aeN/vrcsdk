using UnityEngine;

namespace VRCSDK2
{
	public class VRC_TutorialAction : MonoBehaviour
	{
		public VRC_TutorialAction()
			: this()
		{
		}

		public void ActivateAreaMarkerLabel(Transform targetObject, string text)
		{
			Tutorial.ActivateObjectLabel(targetObject, TutorialLabelType.AreaMarker, ControllerHand.None, text, -1f, 0, AttachMode.PositionOnly, showOffscreen: true);
		}

		public void ActivateObjectLabel(Transform targetObject, TutorialLabelType type, ControllerHand hand, string text, float duration = 0.1f, int priority = 0, AttachMode attachMode = AttachMode.PositionOnly, bool showOffscreen = false)
		{
			Tutorial.ActivateObjectLabel(targetObject, type, hand, text, ControllerActionUI.None, string.Empty, ControllerActionUI.None, duration, priority, attachMode, showOffscreen);
		}

		public void ActivateObjectLabel(Transform targetObject, TutorialLabelType type, ControllerHand hand, string text, ControllerActionUI action, float duration = 0.1f, int priority = 0, AttachMode attachMode = AttachMode.PositionOnly, bool showOffscreen = false)
		{
			Tutorial.ActivateObjectLabel(targetObject, type, hand, text, action, string.Empty, ControllerActionUI.None, duration, priority, attachMode, showOffscreen);
		}

		public void ActivateObjectLabel(Transform targetObject, TutorialLabelType type, ControllerHand hand, string text, ControllerActionUI action, string textSecondary, ControllerActionUI actionSecondary, float duration, int priority, AttachMode attachMode, bool showOffscreen)
		{
			Tutorial.ActivateObjectLabel(targetObject, type, hand, text, action, textSecondary, actionSecondary, duration, priority, attachMode, showOffscreen);
		}

		public void DeactivateObjectLabel(Transform targetObject)
		{
			Tutorial.DeactivateObjectLabel(targetObject);
		}

		public void ActivateControllerLabel(ControllerHand hand, ControllerInputUI controllerPart, string text, float duration, int priority)
		{
			Tutorial.ActivateControllerLabel(hand, controllerPart, text, duration, priority);
		}

		public void DeactivateControllerLabel(ControllerHand hand, ControllerInputUI controllerPart)
		{
			Tutorial.DeactivateControllerLabel(hand, controllerPart);
		}
	}
}
