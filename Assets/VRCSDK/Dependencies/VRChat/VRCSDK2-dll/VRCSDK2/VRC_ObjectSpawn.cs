using System.Collections.Generic;
using UnityEngine;

namespace VRCSDK2
{
	public class VRC_ObjectSpawn : MonoBehaviour, IVRCEventProvider
	{
		public delegate void InitializationDelegate(VRC_ObjectSpawn obj);

		public delegate void InstantiationDelegate(Vector3 position, Quaternion rotation);

		public delegate void ObjectReaperDelegate();

		public GameObject ObjectPrefab;

		public static InitializationDelegate Initialize;

		public InstantiationDelegate Instantiate;

		public ObjectReaperDelegate ReapObjects;

		public VRC_ObjectSpawn()
			: this()
		{
		}

		[RPC(new VRC_EventHandler.VrcTargetType[]
		{

		})]
		public void SpawnObject()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			SpawnObject(this.get_transform().get_position(), this.get_transform().get_rotation());
		}

		[RPC(new VRC_EventHandler.VrcTargetType[]
		{

		})]
		public void SpawnObject(Vector3 position, Quaternion rotation)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			if (Instantiate != null)
			{
				Instantiate(position, rotation);
			}
			else
			{
				Debug.LogError((object)"Spawner not initialized.", this.get_gameObject());
			}
		}

		[RPC(new VRC_EventHandler.VrcTargetType[]
		{

		})]
		public void DestroySpawnedObjects()
		{
			if (ReapObjects != null)
			{
				ReapObjects();
			}
		}

		private void Start()
		{
			if (Initialize != null)
			{
				Initialize(this);
			}
		}

		public IEnumerable<VRC_EventHandler.VrcEvent> ProvideEvents()
		{
			List<VRC_EventHandler.VrcEvent> list = new List<VRC_EventHandler.VrcEvent>();
			VRC_EventHandler.VrcEvent vrcEvent = new VRC_EventHandler.VrcEvent();
			vrcEvent.Name = "SpawnObject";
			vrcEvent.EventType = VRC_EventHandler.VrcEventType.SendRPC;
			vrcEvent.ParameterInt = 3;
			vrcEvent.ParameterString = "SpawnObject";
			vrcEvent.ParameterObjects = (GameObject[])new GameObject[1]
			{
				this.get_gameObject()
			};
			list.Add(vrcEvent);
			vrcEvent = new VRC_EventHandler.VrcEvent();
			vrcEvent.Name = "DestroySpawnedObjects";
			vrcEvent.EventType = VRC_EventHandler.VrcEventType.SendRPC;
			vrcEvent.ParameterInt = 0;
			vrcEvent.ParameterString = "DestroySpawnedObjects";
			vrcEvent.ParameterObjects = (GameObject[])new GameObject[1]
			{
				this.get_gameObject()
			};
			list.Add(vrcEvent);
			return list;
		}
	}
}
