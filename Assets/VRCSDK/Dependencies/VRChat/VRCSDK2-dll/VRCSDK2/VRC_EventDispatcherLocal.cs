using UnityEngine;

namespace VRCSDK2
{
	public class VRC_EventDispatcherLocal : VRC_EventDispatcher
	{
		private void Start()
		{
			Object.Destroy(this);
		}

		public override void TriggerEvent(VRC_EventHandler handler, VRC_EventHandler.VrcEvent e, VRC_EventHandler.VrcBroadcastType broadcast, int instagatorId, float fastForward)
		{
		}

		public override void RegisterEventHandler(VRC_EventHandler handler)
		{
		}

		public override void UnregisterEventHandler(VRC_EventHandler handler)
		{
		}

		public override GameObject FindGameObject(string path, bool suppressErrors)
		{
			return GameObject.Find(path);
		}

		public override string GetGameObjectPath(GameObject go)
		{
			string text = string.Empty;
			while (go != null)
			{
				text = ((!(text == string.Empty)) ? (go.get_name() + "/" + text) : go.get_name());
				if (go.get_transform().get_parent() == null)
				{
					text = "/" + text;
					break;
				}
				go = go.get_transform().get_parent().get_gameObject();
			}
			return text;
		}
	}
}
