using UnityEngine;

namespace VRCSDK2
{
	public abstract class VRC_EventDispatcher : MonoBehaviour
	{
		protected VRC_EventDispatcher()
			: this()
		{
		}

		public abstract void SetMeshVisibility(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, GameObject MeshObject, VRC_EventHandler.VrcBooleanOp Vis);

		public abstract void SetAnimatorBool(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, string Name, VRC_EventHandler.VrcBooleanOp Value);

		public abstract void SetAnimatorTrigger(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, string Name);

		public abstract void SetAnimatorFloat(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, string Name, float Value);

		public abstract void SetAnimatorBool(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, string Name, GameObject destObject, VRC_EventHandler.VrcBooleanOp Value);

		public abstract void SetAnimatorTrigger(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, string Name, GameObject destObject);

		public abstract void SetAnimatorFloat(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, string Name, GameObject destObject, float Value);

		public abstract void PlayAnimation(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, string AnimationName, GameObject destObject, float fastForward = 0);

		public abstract void SendMessage(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, GameObject DestObject, string MessageName);

		public abstract void SetParticlePlaying(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, GameObject MeshObject, VRC_EventHandler.VrcBooleanOp Vis);

		public abstract void TeleportPlayer(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, GameObject Destination, VRC_EventHandler.VrcBooleanOp alignRoomWithDestination);

		public abstract void RunConsoleCommand(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, string ConsoleCommand);

		public abstract void SetGameObjectActive(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, GameObject MeshObject, VRC_EventHandler.VrcBooleanOp Vis);

		public abstract void SetWebPanelURI(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, GameObject webPanelObject, string uri);

		public abstract void SetWebPanelVolume(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, GameObject webPanelObject, float volume);

		public abstract void ActivateCustomTrigger(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, GameObject triggerObject, string customName);

		public abstract void TriggerAudioSource(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, GameObject AudioSource, float fastForward = 0);

		public abstract void TriggerAudioSource(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, GameObject AudioSource, string clipName, float fastForward = 0);

		public abstract void SpawnObject(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, GameObject objectSpawner, string prefabName, byte[] data);

		public abstract void SendRPC(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, VRC_EventHandler.VrcTargetType targetType, GameObject targetObject, string rpcMethodName, byte[] parameters);

		public abstract void DestroyObject(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, GameObject targetObject);

		public abstract void SetLayer(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, GameObject targetObject, int Layer);

		public abstract void SetMaterial(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, GameObject targetObject, string materialName);

		public abstract void AddHealth(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, GameObject targetObject, float health);

		public abstract void AddDamage(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, GameObject targetObject, float damage);

		public abstract void SetComponentActive(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, GameObject targetObject, string componentTypeName, VRC_EventHandler.VrcBooleanOp enable);

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
