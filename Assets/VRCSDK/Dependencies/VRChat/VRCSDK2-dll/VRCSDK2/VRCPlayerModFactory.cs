using System.Collections.Generic;
using UnityEngine;

namespace VRCSDK2
{
	public class VRCPlayerModFactory
	{
		public enum PlayerModType
		{
			Jump = 0,
			Speed = 1,
			Voice = 2,
			Health = 4
		}

		public enum HealthOnDeathAction
		{
			Respawn,
			Kick
		}

		public static VRCPlayerMod Create(PlayerModType modType)
		{
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			List<VRCPlayerModProperty> list = new List<VRCPlayerModProperty>();
			switch (modType)
			{
			case PlayerModType.Jump:
				list.Add(new VRCPlayerModProperty("jumpPower", 3f));
				return new VRCPlayerMod("jump", list, "PlayerModComponentJump");
			case PlayerModType.Speed:
				list.Add(new VRCPlayerModProperty("runSpeed", 4f));
				list.Add(new VRCPlayerModProperty("walkSpeed", 2f));
				list.Add(new VRCPlayerModProperty("strafeSpeed", 2f));
				return new VRCPlayerMod("speed", list, "PlayerModComponentSpeed");
			case PlayerModType.Voice:
				list.Add(new VRCPlayerModProperty("talkDistance", 20f));
				list.Add(new VRCPlayerModProperty("is3DMode", propValue: true));
				return new VRCPlayerMod("voice", list, "PlayerModComponentVoice");
			case PlayerModType.Health:
				list.Add(new VRCPlayerModProperty("totalHealth", 100f));
				list.Add(new VRCPlayerModProperty("onDeathAction", HealthOnDeathAction.Respawn));
				return new VRCPlayerMod("health", list, "PlayerModComponentHealth");
			default:
				throw new UnityException("[ERROR] Unknown PlayerModType. Either add the modtype to PlayerModType enum or use PlayerModFactory.Create with the correct params.");
			}
		}
	}
}
