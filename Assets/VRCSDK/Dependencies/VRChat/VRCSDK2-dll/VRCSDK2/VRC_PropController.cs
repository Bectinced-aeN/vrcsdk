using System;
using System.Collections.Generic;
using UnityEngine;

namespace VRCSDK2
{
	public class VRC_PropController : MonoBehaviour
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

		public delegate void UpdateDelegate(VRC_PropController obj);

		public delegate void InitializeDelegate(VRC_PropController obj);

		public VRC_PlayerApi controllingPlayer;

		public Vector2 inputLeftAnalog;

		public Vector2 inputRightAnalog;

		public bool inputUseButton;

		public static UpdateDelegate UpdateInputs;

		public static InitializeDelegate Initialize;

		public List<InputPairing> Inputs = new List<InputPairing>();

		public VRC_PropController()
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
			for (int i = 0; i < Inputs.Count; i++)
			{
				if (Inputs[i].name == inputName)
				{
					return i;
				}
			}
			return -1;
		}
	}
}
