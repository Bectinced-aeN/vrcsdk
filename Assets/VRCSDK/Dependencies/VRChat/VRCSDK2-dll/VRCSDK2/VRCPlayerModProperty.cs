using System;
using UnityEngine;

namespace VRCSDK2
{
	[Serializable]
	public class VRCPlayerModProperty
	{
		public string name;

		public int intValue;

		public float floatValue;

		public string stringValue;

		public bool boolValue;

		public GameObject gameObjectValue;

		public KeyCode keyCodeValue;

		public RuntimeAnimatorController animationController;

		public VRC_EventHandler.VrcBroadcastType broadcastValue;

		public VRCPlayerModFactory.HealthOnDeathAction onDeathActionValue;

		public VRCSerializableSystemType type;

		public VRCPlayerModProperty(string propName, int propValue)
		{
			name = propName;
			intValue = propValue;
			type = new VRCSerializableSystemType(typeof(int));
		}

		public VRCPlayerModProperty(string propName, float propValue)
		{
			name = propName;
			floatValue = propValue;
			type = new VRCSerializableSystemType(typeof(float));
		}

		public VRCPlayerModProperty(string propName, string propValue)
		{
			name = propName;
			stringValue = propValue;
			type = new VRCSerializableSystemType(typeof(string));
		}

		public VRCPlayerModProperty(string propName, bool propValue)
		{
			name = propName;
			boolValue = propValue;
			type = new VRCSerializableSystemType(typeof(bool));
		}

		public VRCPlayerModProperty(string propName, GameObject propValue)
		{
			name = propName;
			gameObjectValue = propValue;
			type = new VRCSerializableSystemType(typeof(GameObject));
		}

		public VRCPlayerModProperty(string propName, KeyCode propValue)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			name = propName;
			keyCodeValue = propValue;
			type = new VRCSerializableSystemType(typeof(KeyCode));
		}

		public VRCPlayerModProperty(string propName, VRC_EventHandler.VrcBroadcastType propValue)
		{
			name = propName;
			broadcastValue = propValue;
			type = new VRCSerializableSystemType(typeof(VRC_EventHandler.VrcBroadcastType));
		}

		public VRCPlayerModProperty(string propName, VRCPlayerModFactory.HealthOnDeathAction propValue)
		{
			name = propName;
			onDeathActionValue = propValue;
			type = new VRCSerializableSystemType(typeof(VRCPlayerModFactory.HealthOnDeathAction));
		}

		public VRCPlayerModProperty(string propName, RuntimeAnimatorController propValue)
		{
			name = propName;
			animationController = propValue;
			type = new VRCSerializableSystemType(typeof(RuntimeAnimatorController));
		}

		public object value()
		{
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			if (type.SystemType == typeof(int))
			{
				return intValue;
			}
			if (type.SystemType == typeof(float))
			{
				return floatValue;
			}
			if (type.SystemType == typeof(string))
			{
				return stringValue;
			}
			if (type.SystemType == typeof(bool))
			{
				return boolValue;
			}
			if (type.SystemType == typeof(GameObject))
			{
				return gameObjectValue;
			}
			if (type.SystemType == typeof(KeyCode))
			{
				return keyCodeValue;
			}
			if (type.SystemType == typeof(VRC_EventHandler.VrcBroadcastType))
			{
				return broadcastValue;
			}
			if (type.SystemType == typeof(VRCPlayerModFactory.HealthOnDeathAction))
			{
				return onDeathActionValue;
			}
			if (type.SystemType == typeof(RuntimeAnimatorController))
			{
				return animationController;
			}
			return null;
		}
	}
}
