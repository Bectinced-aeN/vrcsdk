using System;
using UnityEngine;

namespace VRCSDK2
{
	public class VRC_NpcApi : MonoBehaviour
	{
		public delegate void InitializeDelegate(VRC_NpcApi obj);

		public static InitializeDelegate Initialize;

		public static Func<GameObject, VRC_NpcApi> _GetApiByGameObject;

		public static Action<VRC_NpcApi, bool, string, string> _SetNamePlate;

		public static Action<VRC_NpcApi, bool, bool, bool> _SetSocialStatus;

		public static Action<VRC_NpcApi, bool, bool> _SetMuteStatus;

		public static Action<VRC_NpcApi, int, bool> _ActThis;

		public static Action<VRC_NpcApi, AudioClip, float> _SayThis;

		public VRC_NpcApi()
			: this()
		{
		}

		private void Start()
		{
			if (Initialize != null)
			{
				Initialize(this);
			}
		}

		public static VRC_NpcApi GetApiByGameObject(GameObject npcGameObject)
		{
			if (npcGameObject != null)
			{
				return _GetApiByGameObject(npcGameObject);
			}
			return null;
		}

		public void ActThis(int number, bool loop)
		{
			_ActThis(this, number, loop);
		}

		public void SayThis(AudioClip clip, float volume)
		{
			_SayThis(this, clip, volume);
		}

		public void SetNamePlate(bool visible, string playerName, string vipTag)
		{
			_SetNamePlate(this, visible, playerName, vipTag);
		}

		public void SetSocialStatus(bool friend, bool vip, bool blocked)
		{
			_SetSocialStatus(this, friend, vip, blocked);
		}

		public void SetMuteStatus(bool canSpeak, bool canHear)
		{
			_SetMuteStatus(this, canSpeak, canHear);
		}
	}
}
