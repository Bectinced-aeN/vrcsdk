using System;
using System.Collections.Generic;
using UnityEngine;

namespace VRCSDK2
{
	public class VRC_PlayerApi : MonoBehaviour
	{
		public enum TrackingDataType
		{
			Head,
			LeftHand,
			RightHand
		}

		public struct TrackingData
		{
			public Vector3 position;

			public Quaternion rotation;

			public TrackingData(Vector3 pos, Quaternion rot)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				position = pos;
				rotation = rot;
			}
		}

		public delegate void InitializeDelegate(VRC_PlayerApi obj);

		public delegate void UpdateDelegate(VRC_PlayerApi obj);

		public delegate void SetAnimatorBoolDelegate(VRC_PlayerApi player, string name, bool value);

		public delegate void ClaimNetworkControlDelegate(VRC_PlayerApi player, VRC_ObjectApi obj);

		public delegate Ray GetLookRayDelegate(VRC_PlayerApi player);

		public delegate bool BoolDelegate(VRC_PlayerApi player);

		public delegate void Action<T1, T2, T3, T4, T5>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5);

		public bool isLocal = true;

		public string name = "<uninitialized>";

		public static InitializeDelegate Initialize;

		public static UpdateDelegate UpdateNow;

		public static SetAnimatorBoolDelegate SetAnimatorBool;

		public static ClaimNetworkControlDelegate ClaimNetworkControl;

		public static GetLookRayDelegate GetLookRay;

		public static BoolDelegate IsGrounded;

		public static List<VRC_PlayerApi> sPlayers = new List<VRC_PlayerApi>();

		private int mPlayerId = -1;

		public static Func<VRC_PlayerApi, int> _GetPlayerId = null;

		public static Func<GameObject, VRC_PlayerApi> _GetPlayerByGameObject = null;

		public static Func<int, VRC_PlayerApi> _GetPlayerById = null;

		public static Func<VRC_PlayerApi, GameObject, bool> _IsOwner = null;

		public static Action<VRC_PlayerApi, GameObject> _TakeOwnership = null;

		public static Func<VRC_PlayerApi, TrackingDataType, TrackingData> _GetTrackingData = null;

		public static Func<VRC_PlayerApi, HumanBodyBones, Transform> _GetBoneTransform = null;

		public static Func<VRC_PlayerApi, VRC_Pickup.PickupHand, VRC_Pickup> _GetPickupInHand = null;

		public static Action<VRC_PlayerApi, VRC_Pickup.PickupHand, float, float, float> _PlayHapticEventInHand = null;

		public static Action<VRC_PlayerApi, Vector3, Quaternion> _TeleportTo = null;

		public static Action<VRC_PlayerApi, bool> _EnablePickups = null;

		public static Action<VRC_PlayerApi, RuntimeAnimatorController> _PushAnimations = null;

		public static Action<VRC_PlayerApi> _PopAnimations = null;

		public static Action<VRC_PlayerApi, bool> _Immobilize = null;

		public static Action<VRC_PlayerApi, Vector3> _SetVelocity = null;

		public static Func<VRC_PlayerApi, Vector3> _GetVelocity = null;

		public static List<VRC_PlayerApi> AllPlayers => sPlayers;

		public int playerId
		{
			get
			{
				if (mPlayerId == -1)
				{
					mPlayerId = GetPlayerId(this);
				}
				return mPlayerId;
			}
		}

		public VRC_PlayerApi()
			: this()
		{
		}

		public bool IsPlayerGrounded()
		{
			if (IsGrounded != null)
			{
				return IsGrounded(this);
			}
			return false;
		}

		public static int GetPlayerId(VRC_PlayerApi player)
		{
			return _GetPlayerId(player);
		}

		public static VRC_PlayerApi GetPlayerByGameObject(GameObject playerGameObject)
		{
			return _GetPlayerByGameObject(playerGameObject);
		}

		public static VRC_PlayerApi GetPlayerById(int playerId)
		{
			return _GetPlayerById(playerId);
		}

		public bool IsOwner(GameObject obj)
		{
			return _IsOwner(this, obj);
		}

		public void TakeOwnership(GameObject obj)
		{
			_TakeOwnership(this, obj);
		}

		public TrackingData GetTrackingData(TrackingDataType tt)
		{
			return _GetTrackingData(this, tt);
		}

		public Transform GetBoneTransform(HumanBodyBones tt)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return _GetBoneTransform(this, tt);
		}

		public VRC_Pickup GetPickupInHand(VRC_Pickup.PickupHand hand)
		{
			return _GetPickupInHand(this, hand);
		}

		public void SetPickupInHand(VRC_Pickup pickup, VRC_Pickup.PickupHand hand)
		{
		}

		public void PlayHapticEventInHand(VRC_Pickup.PickupHand hand, float duration, float amplitude, float frequency)
		{
			_PlayHapticEventInHand(this, hand, duration, amplitude, frequency);
		}

		public void TeleportTo(Vector3 teleportPos, Quaternion teleportRot)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_TeleportTo(this, teleportPos, teleportRot);
		}

		public void EnablePickups(bool enable)
		{
			_EnablePickups(this, enable);
		}

		private void Awake()
		{
			sPlayers.Add(this);
			if (Initialize != null)
			{
				Initialize(this);
			}
		}

		private void OnDestroy()
		{
			sPlayers.Remove(this);
		}

		private void Update()
		{
			if (UpdateNow != null)
			{
				UpdateNow(this);
			}
		}

		public void PushAnimations(RuntimeAnimatorController animations)
		{
			_PushAnimations(this, animations);
		}

		public void PopAnimations()
		{
			_PopAnimations(this);
		}

		public void Immobilize(bool immobile)
		{
			_Immobilize(this, immobile);
		}

		public void SetVelocity(Vector3 velocity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			_SetVelocity(this, velocity);
		}

		public Vector3 GetVelocity()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return _GetVelocity(this);
		}
	}
}
