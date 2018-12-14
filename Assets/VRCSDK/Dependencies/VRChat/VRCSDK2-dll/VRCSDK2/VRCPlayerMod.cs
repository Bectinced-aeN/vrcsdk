using System;
using System.Collections.Generic;
using UnityEngine;

namespace VRCSDK2
{
	[Serializable]
	public class VRCPlayerMod : IEquatable<VRCPlayerMod>
	{
		[SerializeField]
		private string mName;

		[SerializeField]
		private List<VRCPlayerModProperty> mProperties;

		[SerializeField]
		private string mModComponentName;

		[SerializeField]
		private bool mAllowNameEdit;

		public string name
		{
			get
			{
				return mName;
			}
			set
			{
				if (mAllowNameEdit)
				{
					mName = value;
				}
			}
		}

		public List<VRCPlayerModProperty> properties => mProperties;

		public string modComponentName => mModComponentName;

		public bool allowNameEdit => mAllowNameEdit;

		public VRCPlayerMod(string modName, List<VRCPlayerModProperty> defaultProperties, string modComponentName)
		{
			mName = modName;
			if (mName == "prop")
			{
				mAllowNameEdit = true;
			}
			mProperties = defaultProperties;
			mModComponentName = modComponentName;
		}

		public IPlayerModComponent AddOrUpdateModComponentOn(GameObject go)
		{
			IPlayerModComponent playerModComponent = null;
			if (go != null)
			{
				playerModComponent = (IPlayerModComponent)go.GetComponent(mModComponentName);
				if (playerModComponent == null)
				{
					string typeName = mModComponentName + ", Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
					Type type = Type.GetType(typeName);
					if (type != null)
					{
						playerModComponent = (IPlayerModComponent)go.AddComponent(type);
					}
				}
				playerModComponent?.SetProperties(mProperties);
			}
			return playerModComponent;
		}

		public bool Equals(VRCPlayerMod other)
		{
			return mName == other.name && mProperties == other.properties && mModComponentName == other.modComponentName;
		}
	}
}
