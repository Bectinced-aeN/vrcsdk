using UnityEngine;

namespace VRCSDK2
{
	public abstract class VRC_EventDispatcher : MonoBehaviour
	{
		protected VRC_EventDispatcher()
			: this()
		{
		}

		public abstract void TriggerEvent(VRC_EventHandler handler, VRC_EventHandler.VrcEvent e, VRC_EventHandler.VrcBroadcastType broadcast, int instagatorId, float fastForward);

		public abstract void RegisterEventHandler(VRC_EventHandler handler);

		public abstract void UnregisterEventHandler(VRC_EventHandler handler);

		public virtual GameObject FindGameObject(string path)
		{
			return FindGameObject(path, suppressErrors: false);
		}

		public abstract GameObject FindGameObject(string path, bool suppressErrors);

		public abstract string GetGameObjectPath(GameObject go);
	}
}
