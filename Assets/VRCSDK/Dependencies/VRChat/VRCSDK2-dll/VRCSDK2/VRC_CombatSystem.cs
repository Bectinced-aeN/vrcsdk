using System;
using UnityEngine;

namespace VRCSDK2
{
	public class VRC_CombatSystem : MonoBehaviour
	{
		private static VRC_CombatSystem instance;

		public float maxPlayerHealth = 100f;

		public bool respawnOnDeath = true;

		public Transform respawnPoint;

		public float respawnTime = 5f;

		public bool resetHealthOnRespawn;

		public GameObject visualDamagePrefab;

		public VRC_Trigger.CustomTriggerTarget onPlayerKilledTrigger;

		public VRC_Trigger.CustomTriggerTarget onPlayerHealedTrigger;

		public VRC_Trigger.CustomTriggerTarget onPlayerDamagedTrigger;

		public Action<VRC_PlayerApi> onPlayerKilled;

		public Action<VRC_PlayerApi> onPlayerHealed;

		public Action<VRC_PlayerApi> onPlayerDamaged;

		public Action<VRC_PlayerApi> onSetupPlayer;

		public VRC_CombatSystem()
			: this()
		{
		}

		public static VRC_CombatSystem GetInstance()
		{
			return instance;
		}

		private void OnDestroy()
		{
			instance = null;
		}

		private void Awake()
		{
			if (instance != null)
			{
				Debug.LogError((object)"Multiple Combat systems present");
			}
			instance = this;
		}

		[RPC(new VRC_EventHandler.VrcTargetType[]
		{
			VRC_EventHandler.VrcTargetType.Master,
			VRC_EventHandler.VrcTargetType.Local
		})]
		public void RespawnPlayer(int playerId)
		{
			if (Networking.LocalPlayer.playerId == playerId)
			{
				Networking.RPC(VRC_EventHandler.VrcTargetType.Local, Networking.LocalPlayer.get_gameObject(), "Respawn");
			}
			else if (Networking.IsMaster)
			{
				Networking.RPC(VRC_PlayerApi.GetPlayerById(playerId), this.get_gameObject(), "RespawnPlayerRPC");
			}
		}

		[RPC(new VRC_EventHandler.VrcTargetType[]
		{
			VRC_EventHandler.VrcTargetType.TargetPlayer
		})]
		private void RespawnPlayerRPC(int instigatorId)
		{
			if (VRC_PlayerApi.GetPlayerById(instigatorId).isMaster)
			{
				GameObject gameObject = Networking.LocalPlayer.get_gameObject();
				Networking.RPC(VRC_EventHandler.VrcTargetType.Local, gameObject, "Respawn");
			}
		}
	}
}
