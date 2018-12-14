using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VRCSDK2
{
	public class VRC_EventDispatcherLocal : VRC_EventDispatcher
	{
		private Dictionary<Type, MethodInfo[]> cachedMethodInfo = new Dictionary<Type, MethodInfo[]>();

		public override void ActivateCustomTrigger(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, GameObject triggerObject, string customName)
		{
			_ActivateCustomTrigger(CombinedNetworkId, GetGameObjectPath(triggerObject), customName);
		}

		public void _ActivateCustomTrigger(long CombinedNetworkId, string triggerObjectName, string customName)
		{
			Transform val = FindTransform(triggerObjectName);
			if (val != null)
			{
				VRC_Trigger.TriggerCustom(val.get_gameObject(), customName);
			}
		}

		public override void SetMeshVisibility(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, GameObject MeshObject, VRC_EventHandler.VrcBooleanOp Vis)
		{
			string gameObjectPath = GetGameObjectPath(MeshObject);
			_SetMeshVisibility(CombinedNetworkId, gameObjectPath, (int)Vis);
		}

		public void _SetMeshVisibility(long CombinedNetworkId, string MeshObjectName, int Vis)
		{
			Transform val = FindTransform(MeshObjectName);
			if (val != null)
			{
				if (val.GetComponent<MeshRenderer>() != null)
				{
					val.GetComponent<MeshRenderer>().set_enabled(VRC_EventHandler.BooleanOp((VRC_EventHandler.VrcBooleanOp)Vis, val.GetComponent<MeshRenderer>().get_enabled()));
				}
				if (val.GetComponent<SkinnedMeshRenderer>() != null)
				{
					val.GetComponent<SkinnedMeshRenderer>().set_enabled(VRC_EventHandler.BooleanOp((VRC_EventHandler.VrcBooleanOp)Vis, val.GetComponent<SkinnedMeshRenderer>().get_enabled()));
				}
			}
		}

		public override void SetAnimatorBool(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, string Name, GameObject destObject, VRC_EventHandler.VrcBooleanOp Value)
		{
			_SetAnimatorBool(CombinedNetworkId, Name, GetGameObjectPath(destObject), (int)Value);
		}

		public void _SetAnimatorBool(long CombinedNetworkId, string Name, string destObject, int Value)
		{
			Transform val = FindTransform(destObject);
			if (val != null && val.get_gameObject().GetComponent<Animator>() != null)
			{
				bool @bool = val.get_gameObject().GetComponent<Animator>().GetBool(Name);
				val.get_gameObject().GetComponent<Animator>().SetBool(Name, VRC_EventHandler.BooleanOp((VRC_EventHandler.VrcBooleanOp)Value, @bool));
			}
		}

		public override void SetAnimatorTrigger(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, string Name, GameObject destObject)
		{
			_SetAnimatorTrigger(CombinedNetworkId, Name, GetGameObjectPath(destObject));
		}

		public void _SetAnimatorTrigger(long CombinedNetworkId, string Name, string destObject)
		{
			Transform val = FindTransform(destObject);
			if (val != null && val.get_gameObject().GetComponent<Animator>() != null)
			{
				val.get_gameObject().GetComponent<Animator>().SetTrigger(Name);
			}
		}

		public override void SetAnimatorFloat(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, string Name, GameObject destObject, float Value)
		{
			_SetAnimatorFloat(CombinedNetworkId, Name, GetGameObjectPath(destObject), Value);
		}

		public void _SetAnimatorFloat(long CombinedNetworkId, string Name, string destObject, float Value)
		{
			Transform val = FindTransform(destObject);
			if (val != null && val.get_gameObject().GetComponent<Animator>() != null)
			{
				val.get_gameObject().GetComponent<Animator>().SetFloat(Name, Value);
			}
		}

		public override void SetAnimatorBool(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, string Name, VRC_EventHandler.VrcBooleanOp Value)
		{
			_SetAnimatorBool(CombinedNetworkId, Name, (int)Value);
		}

		public void _SetAnimatorBool(long CombinedNetworkId, string Name, int Value)
		{
			if (this.GetComponent<Animator>() != null)
			{
				bool @bool = this.GetComponent<Animator>().GetBool(Name);
				this.GetComponent<Animator>().SetBool(Name, VRC_EventHandler.BooleanOp((VRC_EventHandler.VrcBooleanOp)Value, @bool));
			}
		}

		public override void SetAnimatorTrigger(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, string Name)
		{
			_SetAnimatorTrigger(CombinedNetworkId, Name);
		}

		public void _SetAnimatorTrigger(long CombinedNetworkId, string Name)
		{
			if (this.GetComponent<Animator>() != null)
			{
				this.GetComponent<Animator>().SetTrigger(Name);
			}
		}

		public override void SetAnimatorFloat(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, string Name, float Value)
		{
			_SetAnimatorFloat(CombinedNetworkId, Name, Value);
		}

		public void _SetAnimatorFloat(long CombinedNetworkId, string Name, float Value)
		{
			if (this.GetComponent<Animator>() != null)
			{
				this.GetComponent<Animator>().SetFloat(Name, Value);
			}
		}

		public override void TriggerAudioSource(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, GameObject AudioSource, float fastForward = 0)
		{
			TriggerAudioSource(CombinedNetworkId, Broadcast, Instigator, AudioSource, null, fastForward);
		}

		public override void TriggerAudioSource(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, GameObject AudioSource, string clipName, float fastForward = 0)
		{
			string gameObjectPath = GetGameObjectPath(AudioSource);
			_TriggerAudioSource(CombinedNetworkId, gameObjectPath, clipName, fastForward);
		}

		public void _TriggerAudioSource(long CombinedNetworkId, string AudioSourceName, float fastForward)
		{
			_TriggerAudioSource(CombinedNetworkId, AudioSourceName, null, fastForward);
		}

		public void _TriggerAudioSource(long CombinedNetworkId, string AudioSourceName, string clipName, float fastForward)
		{
			Transform val = FindTransform(AudioSourceName);
			if (val != null)
			{
				AudioSource[] components = val.GetComponents<AudioSource>();
				if (components != null && components.Length > 0)
				{
					if (!string.IsNullOrEmpty(clipName))
					{
						foreach (AudioSource item in from s in components
						where s.get_clip() != null && s.get_clip().get_name() == clipName
						select s)
						{
							item.Play();
						}
					}
					else
					{
						AudioSource[] array = components;
						foreach (AudioSource val2 in array)
						{
							val2.Play();
						}
					}
				}
			}
		}

		public override void PlayAnimation(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, string AnimationName, GameObject destObject, float fastForward = 0)
		{
			string gameObjectPath = GetGameObjectPath(destObject);
			_PlayAnimation(CombinedNetworkId, AnimationName, gameObjectPath, fastForward);
		}

		public void _PlayAnimation(long CombinedNetworkId, string AnimationName, string destObjectName, float fastForward)
		{
			Transform val = FindTransform(destObjectName);
			GameObject val2 = (!(val != null)) ? this.get_gameObject() : val.get_gameObject();
			val2.GetComponent<Animation>().Play(AnimationName);
		}

		public override void SendMessage(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, GameObject DestObject, string MessageName)
		{
			string gameObjectPath = GetGameObjectPath(DestObject);
			_SendMessage(CombinedNetworkId, Instigator, gameObjectPath, MessageName);
		}

		public void _SendMessage(long CombinedNetworkId, int Instigator, string DestObjectName, string MessageName)
		{
			Transform val = FindTransform(DestObjectName);
			if (val != null)
			{
				val.get_gameObject().SendMessage(MessageName, (object)Instigator);
			}
		}

		public override void SetParticlePlaying(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, GameObject MeshObject, VRC_EventHandler.VrcBooleanOp Vis)
		{
			string gameObjectPath = GetGameObjectPath(MeshObject);
			_SetParticlePlaying(CombinedNetworkId, gameObjectPath, (int)Vis);
		}

		public void _SetParticlePlaying(long CombinedNetworkId, string MeshObjectName, int Vis)
		{
			Transform val = FindTransform(MeshObjectName);
			if (val != null)
			{
				ParticleSystem component = val.GetComponent<ParticleSystem>();
				if (component != null)
				{
					if (VRC_EventHandler.BooleanOp((VRC_EventHandler.VrcBooleanOp)Vis, component.get_isPlaying()))
					{
						component.Play();
					}
					else
					{
						component.Stop();
					}
				}
			}
		}

		public override void TeleportPlayer(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, GameObject Destination)
		{
			string gameObjectPath = GetGameObjectPath(Destination);
			_TeleportPlayer(CombinedNetworkId, Instigator, gameObjectPath);
		}

		public void _TeleportPlayer(long CombinedNetworkId, int Instigator, string DestinationName)
		{
			Transform val = FindTransform(DestinationName);
			if (!(val == null))
			{
				Debug.Log((object)"Teleport is unimplemented in the SDK.  Suffice to say you would have teleported just now.");
			}
		}

		public override void RunConsoleCommand(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, string ConsoleCommand)
		{
			_RunConsoleCommand(CombinedNetworkId, Instigator, ConsoleCommand);
		}

		public void _RunConsoleCommand(long CombinedNetworkId, int Instigator, string ConsoleCommand)
		{
		}

		public override void SetGameObjectActive(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, GameObject MeshObject, VRC_EventHandler.VrcBooleanOp Vis)
		{
			string gameObjectPath = GetGameObjectPath(MeshObject);
			_SetGameObjectActive(CombinedNetworkId, gameObjectPath, (int)Vis);
		}

		public void _SetGameObjectActive(long CombinedNetworkId, string MeshObjectName, int Vis)
		{
			Transform val = FindTransform(MeshObjectName);
			if (val != null)
			{
				bool activeSelf = val.get_gameObject().get_activeSelf();
				val.get_gameObject().SetActive(VRC_EventHandler.BooleanOp((VRC_EventHandler.VrcBooleanOp)Vis, activeSelf));
			}
		}

		public override void SetWebPanelURI(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, GameObject webPanelObject, string uri)
		{
			string gameObjectPath = GetGameObjectPath(webPanelObject);
			_SetWebPanelURI(CombinedNetworkId, gameObjectPath, uri);
		}

		public void _SetWebPanelURI(long CombinedNetworkId, string webPanelObjectName, string uri)
		{
			Transform val = FindTransform(webPanelObjectName);
			if (val != null)
			{
				val.get_gameObject().SendMessage("NavigateTo", (object)uri);
			}
			else
			{
				Debug.LogError((object)("Could not locate " + webPanelObjectName));
			}
		}

		public override void SetWebPanelVolume(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, GameObject webPanelObject, float volume)
		{
			string gameObjectPath = GetGameObjectPath(webPanelObject);
			_SetWebPanelVolume(CombinedNetworkId, gameObjectPath, volume);
		}

		public void _SetWebPanelVolume(long CombinedNetworkId, string webPanelObjectName, float volume)
		{
			Transform val = FindTransform(webPanelObjectName);
			if (val != null)
			{
				val.get_gameObject().SendMessage("SetVolume", (object)volume);
			}
			else
			{
				Debug.LogError((object)("Could not locate " + webPanelObjectName));
			}
		}

		public override void SpawnObject(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, GameObject objectSpawner, string prefabName, byte[] data)
		{
			string gameObjectPath = GetGameObjectPath(objectSpawner);
			_SpawnObject(CombinedNetworkId, gameObjectPath, prefabName, data);
		}

		public void _SpawnObject(long CombinedNetworkId, string objectSpawnerName, string prefabName, byte[] data)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			object[] array = VRC_Serialization.ParameterDecoder(data);
			Vector3 position = (Vector3)array[0];
			Quaternion rotation = (Quaternion)array[1];
			Networking.Instantiate(VRC_EventHandler.VrcBroadcastType.Local, prefabName, position, rotation);
		}

		public override void SendRPC(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, VRC_EventHandler.VrcTargetType targetType, GameObject targetObject, string rpcMethodName, byte[] parameters)
		{
			string gameObjectPath = GetGameObjectPath(targetObject);
			_SendRPC(CombinedNetworkId, (int)targetType, Instigator, gameObjectPath, rpcMethodName, parameters);
		}

		public void _SendRPC(long CombinedNetworkId, int targetType, int Instigator, string targetObjectName, string rpcMethodName, byte[] dataParameters)
		{
			Debug.LogError((object)"RPCs cannot execute without an active VR Chat connection.");
		}

		public override void DestroyObject(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, GameObject targetObject)
		{
			if (targetObject != null)
			{
				Object.Destroy(targetObject);
			}
		}

		public override void SetLayer(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, GameObject targetObject, int Layer)
		{
			if (targetObject != null)
			{
				targetObject.set_layer(Layer);
			}
		}

		public override void SetMaterial(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, GameObject targetObject, string materialName)
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Expected O, but got Unknown
			if (targetObject != null)
			{
				Material val = VRC_SceneDescriptor.GetMaterial(materialName);
				if (val == null)
				{
					val = Resources.Load(materialName, typeof(Material));
				}
				if (val != null)
				{
					Renderer[] components = targetObject.GetComponents<Renderer>();
					foreach (Renderer val2 in components)
					{
						val2.set_sharedMaterial(val);
					}
				}
			}
		}

		public override void AddHealth(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, GameObject targetObject, float health)
		{
			if (targetObject == null)
			{
				targetObject = Networking.LocalPlayer.get_gameObject();
			}
		}

		public override void AddDamage(long CombinedNetworkId, VRC_EventHandler.VrcBroadcastType Broadcast, int Instigator, GameObject targetObject, float damage)
		{
			if (targetObject == null)
			{
				targetObject = Networking.LocalPlayer.get_gameObject();
			}
		}

		public override void RegisterEventHandler(VRC_EventHandler handler)
		{
		}

		public override void UnregisterEventHandler(VRC_EventHandler handler)
		{
		}

		public override string GetGameObjectPath(GameObject go)
		{
			return GetGameObjectPathFallback(go);
		}

		public static string GetGameObjectPathFallback(GameObject go)
		{
			if (go == null)
			{
				return string.Empty;
			}
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

		public override GameObject FindGameObject(string path)
		{
			return FindGameObjectFallback(path);
		}

		public static GameObject FindChild(GameObject root, IEnumerable<string> path)
		{
			string text = path.FirstOrDefault();
			if (string.IsNullOrEmpty(text))
			{
				return root;
			}
			Transform[] componentsInChildren = root.GetComponentsInChildren<Transform>(true);
			foreach (Transform val in componentsInChildren)
			{
				if (val.get_gameObject().get_name() == text)
				{
					return FindChild(val.get_gameObject(), path.Skip(1));
				}
			}
			return null;
		}

		public static GameObject FindGameObjectFallback(string path)
		{
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			if (string.IsNullOrEmpty(path))
			{
				return null;
			}
			path = path.Trim();
			GameObject val = null;
			if (path.StartsWith("/"))
			{
				List<string> list = path.Substring(1).Split('/').ToList();
				Scene activeScene = SceneManager.GetActiveScene();
				GameObject[] rootGameObjects = activeScene.GetRootGameObjects();
				foreach (GameObject val2 in rootGameObjects)
				{
					if (val2.get_name() == list[0])
					{
						val = FindChild(val2, list.Skip(1));
						break;
					}
				}
			}
			if (val == null)
			{
				val = GameObject.Find(path);
			}
			return val;
		}

		private Transform FindTransform(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return null;
			}
			path = path.Trim();
			if (path.StartsWith("./"))
			{
				path = path.Substring(2);
				if (string.IsNullOrEmpty(path))
				{
					return this.get_transform();
				}
				return this.get_transform().Find(path);
			}
			return this.get_transform().Find(path);
		}
	}
}
