using UnityEngine;

namespace VRCSDK2
{
	public class VRC_AvatarPedestal : MonoBehaviour, INetworkID
	{
		public delegate Object InstantiationDelegate(Object prefab);

		public string blueprintId = string.Empty;

		[HideInInspector]
		public bool grantBlueprintAccess;

		public Transform Placement;

		public bool ChangeAvatarsOnUse;

		public float scale = 1f;

		private GameObject Instance;

		public static InstantiationDelegate Instantiate;

		public int NetworkID
		{
			get;
			set;
		}

		public VRC_AvatarPedestal()
			: this()
		{
		}

		private void Awake()
		{
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			GameObject val = Resources.Load<GameObject>("AvatarPedestal");
			if (val != null && Instantiate != null)
			{
				Instance = (Instantiate(val) as GameObject);
				if (Placement != null)
				{
					Instance.get_transform().set_parent(Placement.get_transform());
				}
				else
				{
					Instance.get_transform().set_parent(this.get_transform());
				}
				Instance.get_transform().set_localPosition(new Vector3(0f, 0f, 0f));
				Instance.get_transform().set_localRotation(Quaternion.get_identity());
				Instance.get_transform().set_localScale(new Vector3(1f, 1f, 1f));
			}
		}

		[RPC(new VRC_EventHandler.VrcTargetType[]
		{

		})]
		public void SwitchAvatar(string id)
		{
			blueprintId = id;
			if (Instance != null)
			{
				Networking.RPC(VRC_EventHandler.VrcTargetType.All, Instance, "RefreshAvatar");
			}
		}

		[RPC(new VRC_EventHandler.VrcTargetType[]
		{
			VRC_EventHandler.VrcTargetType.All
		})]
		public void SetAvatarUse()
		{
			if (Instance != null)
			{
				Networking.RPC(VRC_EventHandler.VrcTargetType.Local, Instance, "SetAvatarUse");
			}
		}
	}
}
