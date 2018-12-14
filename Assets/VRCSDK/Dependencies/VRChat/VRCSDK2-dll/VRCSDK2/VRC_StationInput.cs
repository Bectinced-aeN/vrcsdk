using System;
using System.Collections.Generic;
using UnityEngine;

namespace VRCSDK2
{
	public class VRC_StationInput : MonoBehaviour
	{
		[Serializable]
		public class InputPairing
		{
			public string name;

			public KeyCode[] unityKeys;

			public string[] cInputKeys;

			[HideInInspector]
			public bool value;

			[HideInInspector]
			public bool lastValue;

			public bool GetKeyDown()
			{
				return value && !lastValue;
			}
		}

		public delegate void UpdateDelegate(VRC_StationInput obj);

		public delegate void InitializeDelegate(VRC_StationInput obj);

		[HideInInspector]
		public VRC_PlayerApi controllingPlayer;

		[HideInInspector]
		public Vector2 inputLeftAnalog;

		[HideInInspector]
		public Vector2 inputRightAnalog;

		[HideInInspector]
		public bool inputUseButton;

		public static UpdateDelegate UpdateInputs;

		public static InitializeDelegate Initialize;

		public List<InputPairing> customInputs = new List<InputPairing>();

		public VRC_StationInput()
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

		private void Update()
		{
			if (controllingPlayer != null && controllingPlayer.isLocal && UpdateInputs != null)
			{
				UpdateInputs(this);
			}
		}

		public int GetInputIndex(string inputName)
		{
			for (int i = 0; i < customInputs.Count; i++)
			{
				if (customInputs[i].name == inputName)
				{
					return i;
				}
			}
			return -1;
		}
	}
}
