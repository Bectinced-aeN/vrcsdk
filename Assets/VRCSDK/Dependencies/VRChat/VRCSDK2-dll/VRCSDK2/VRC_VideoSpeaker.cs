using System;
using UnityEngine;

namespace VRCSDK2
{
	[RequireComponent(typeof(AudioSource))]
	public class VRC_VideoSpeaker : MonoBehaviour
	{
		[Serializable]
		public enum ChannelType
		{
			Stereo_Mix,
			Mono_Left,
			Mono_Right
		}

		public VRC_SyncVideoStream _videoStream;

		public ChannelType _channelType;

		public static Action<VRC_VideoSpeaker> Initialize;

		public VRC_VideoSpeaker()
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
