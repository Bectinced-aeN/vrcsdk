using System;
using UnityEngine;

namespace VRCSDK2
{
	public class VRC_CombatSystem : MonoBehaviour
	{
		private static VRC_CombatSystem instance;

		public float maxPlayerHealth = 100f;

		public Transform respawnPoint;

		public float respawnTime = 5f;

		public bool resetHealthOnRespawn;

		[Obsolete("Property is not used.")]
		[HideInInspector]
		public bool respawnOnDeath = true;

		public GameObject visualDamagePrefab;

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
	}
}
