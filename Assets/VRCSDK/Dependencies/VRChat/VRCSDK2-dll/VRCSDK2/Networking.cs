using System;
using System.Collections;
using UnityEngine;

namespace VRCSDK2
{
	public static class Networking
	{
		public static Func<GameObject, string> _GetUniqueName;

		public static Action<VRC_EventHandler.VrcTargetType, GameObject, string, object[]> _RPC;

		public static Action<VRC_PlayerApi, GameObject, string, object[]> _RPCtoPlayer;

		public static Action<VRC_EventHandler.VrcBroadcastType, GameObject, string> _Message;

		public static Func<bool> _IsNetworkSettled;

		public static Func<bool> _IsMaster;

		public static Func<VRC_PlayerApi> _LocalPlayer;

		public static Func<VRC_PlayerApi, GameObject, bool> _IsOwner;

		public static Action<VRC_PlayerApi, GameObject> _SetOwner;

		public static Func<GameObject, bool> _IsObjectReady;

		public static Func<VRC_EventHandler.VrcBroadcastType, string, Vector3, Quaternion, GameObject> _Instantiate;

		public static Func<GameObject, VRC_PlayerApi> _GetOwner;

		public static Func<object[], byte[]> _ParameterEncoder;

		public static Func<byte[], object[]> _ParameterDecoder;

		public static Action<GameObject> _Destroy;

		public static Func<VRC_EventHandler> _SceneEventHandler;

		public static Func<DateTime> _GetNetworkDateTime;

		public static Func<double> _GetServerTimeInSeconds;

		public static Func<int> _GetServerTimeInMilliseconds;

		public static Func<double, double, double> _CalculateServerDeltaTime;

		public static Func<IEnumerator, Coroutine> _SafeStartCoroutine;

		public static Func<VRC_EventDispatcher> _GetEventDispatcher;

		public static VRC_EventHandler SceneEventHandler => (_SceneEventHandler != null) ? _SceneEventHandler() : null;

		public static bool IsNetworkSettled => _IsNetworkSettled == null || _IsNetworkSettled();

		public static bool IsMaster => _IsMaster == null || _IsMaster();

		public static VRC_PlayerApi LocalPlayer => (_LocalPlayer != null) ? _LocalPlayer() : null;

		public static bool IsOwner(VRC_PlayerApi player, GameObject obj)
		{
			return _IsOwner == null || _IsOwner(player, obj);
		}

		public static bool IsOwner(GameObject obj)
		{
			return _IsOwner == null || _IsOwner(LocalPlayer, obj);
		}

		public static VRC_PlayerApi GetOwner(GameObject obj)
		{
			if (_GetOwner != null)
			{
				return _GetOwner(obj);
			}
			return null;
		}

		public static void SetOwner(VRC_PlayerApi player, GameObject obj)
		{
			if (_SetOwner != null)
			{
				_SetOwner(player, obj);
			}
		}

		public static bool IsObjectReady(GameObject obj)
		{
			return _IsObjectReady == null || _IsObjectReady(obj);
		}

		public static GameObject Instantiate(VRC_EventHandler.VrcBroadcastType broadcast, string prefabPathOrDynamicPrefabName, Vector3 position, Quaternion rotation)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			if (_Instantiate == null)
			{
				return null;
			}
			return _Instantiate(broadcast, prefabPathOrDynamicPrefabName, position, rotation);
		}

		public static void RPC(VRC_EventHandler.VrcTargetType targetClients, GameObject targetObject, string methodName, params object[] parameters)
		{
			if (_RPC != null)
			{
				_RPC(targetClients, targetObject, methodName, parameters);
			}
		}

		public static void RPC(VRC_PlayerApi targetPlayer, GameObject targetObject, string methodName, params object[] parameters)
		{
			if (_RPCtoPlayer != null)
			{
				_RPCtoPlayer(targetPlayer, targetObject, methodName, parameters);
			}
		}

		public static void SendMessage(VRC_EventHandler.VrcBroadcastType broadcast, GameObject target, string methodName)
		{
			if (_Message != null)
			{
				_Message(broadcast, target, methodName);
			}
		}

		public static byte[] EncodeParameters(params object[] parameters)
		{
			if (_ParameterEncoder != null)
			{
				return _ParameterEncoder(parameters);
			}
			return null;
		}

		public static object[] DecodeParameters(byte[] encodedData)
		{
			if (_ParameterDecoder != null)
			{
				return _ParameterDecoder(encodedData);
			}
			return null;
		}

		public static void Destroy(GameObject obj)
		{
			if (_Destroy != null)
			{
				_Destroy(obj);
			}
		}

		public static string GetUniqueName(GameObject obj)
		{
			if (_GetUniqueName != null)
			{
				return _GetUniqueName(obj);
			}
			return null;
		}

		public static DateTime GetNetworkDateTime()
		{
			if (_GetNetworkDateTime != null)
			{
				return _GetNetworkDateTime();
			}
			return DateTime.UtcNow;
		}

		public static double GetServerTimeInSeconds()
		{
			if (_GetServerTimeInSeconds != null)
			{
				return _GetServerTimeInSeconds();
			}
			return (double)Time.get_time();
		}

		public static int GetServerTimeInMilliseconds()
		{
			if (_GetServerTimeInMilliseconds != null)
			{
				return _GetServerTimeInMilliseconds();
			}
			return (int)(Time.get_time() * 1000f);
		}

		public static double CalculateServerDeltaTime(double timeInSeconds, double previousTimeInSeconds)
		{
			if (_CalculateServerDeltaTime != null)
			{
				return _CalculateServerDeltaTime(timeInSeconds, previousTimeInSeconds);
			}
			return timeInSeconds - previousTimeInSeconds;
		}

		public static Coroutine SafeStartCoroutine(IEnumerator target)
		{
			if (_SafeStartCoroutine != null)
			{
				return _SafeStartCoroutine(target);
			}
			return null;
		}

		public static VRC_EventDispatcher GetEventDispatcher()
		{
			if (_GetEventDispatcher != null)
			{
				return _GetEventDispatcher();
			}
			return null;
		}
	}
}
