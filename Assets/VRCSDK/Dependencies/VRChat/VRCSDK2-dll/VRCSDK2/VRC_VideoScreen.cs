using System;
using UnityEngine;

namespace VRCSDK2
{
	[RequireComponent(typeof(Renderer))]
	public class VRC_VideoScreen : MonoBehaviour
	{
		public VRC_SyncVideoStream _videoStream;

		public int _materialIndex;

		public string _textureProperty = "_MainTex";

		public bool _useSharedMaterial;

		public static Action<VRC_VideoScreen> Initialize;

		public VRC_VideoScreen()
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
	}
}
