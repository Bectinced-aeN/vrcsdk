using System.Collections.Generic;
using UnityEngine;

namespace VRCSDK2
{
	[RequireComponent(typeof(VRC_EventHandler))]
	public class VRC_PlayerMods : MonoBehaviour, IVRCEventProvider
	{
		public delegate void InitializationDelegate(VRC_PlayerMods obj);

		public bool isRoomPlayerMods;

		[HideInInspector]
		public List<VRCPlayerMod> playerMods = new List<VRCPlayerMod>();

		public static InitializationDelegate Initialize;

		public VRC_PlayerMods()
			: this()
		{
		}

		private void Awake()
		{
			if (Initialize != null)
			{
				Initialize(this);
			}
		}

		public void AddMod(VRCPlayerMod mod)
		{
			bool flag = false;
			foreach (VRCPlayerMod playerMod in playerMods)
			{
				if (playerMod.Equals(mod))
				{
					flag = true;
				}
			}
			if (!flag)
			{
				playerMods.Add(mod);
			}
		}

		public void RemoveMod(VRCPlayerMod mod)
		{
			foreach (VRCPlayerMod playerMod in playerMods)
			{
				if (playerMod.name == mod.name)
				{
					playerMods.Remove(playerMod);
					break;
				}
			}
		}

		public IEnumerable<VRC_EventHandler.VrcEvent> ProvideEvents()
		{
			List<VRC_EventHandler.VrcEvent> list = new List<VRC_EventHandler.VrcEvent>();
			VRC_EventHandler.VrcEvent vrcEvent = new VRC_EventHandler.VrcEvent();
			vrcEvent.Name = "AddPlayerMods";
			vrcEvent.ParameterInt = 7;
			vrcEvent.EventType = VRC_EventHandler.VrcEventType.SendRPC;
			vrcEvent.ParameterString = "AddPlayerMods";
			vrcEvent.ParameterObjects = (GameObject[])new GameObject[1]
			{
				this.get_gameObject()
			};
			list.Add(vrcEvent);
			vrcEvent = new VRC_EventHandler.VrcEvent();
			vrcEvent.Name = "RemovePlayerMods";
			vrcEvent.ParameterInt = 7;
			vrcEvent.EventType = VRC_EventHandler.VrcEventType.SendRPC;
			vrcEvent.ParameterString = "RemovePlayerMods";
			vrcEvent.ParameterObjects = (GameObject[])new GameObject[1]
			{
				this.get_gameObject()
			};
			list.Add(vrcEvent);
			return list;
		}
	}
}
