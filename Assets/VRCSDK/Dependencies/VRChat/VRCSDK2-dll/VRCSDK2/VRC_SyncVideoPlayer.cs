using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace VRCSDK2
{
	[RequireComponent(typeof(VideoPlayer))]
	public class VRC_SyncVideoPlayer : MonoBehaviour, IVRCEventProvider
	{
		[Serializable]
		public class VideoEntry
		{
			public VideoSource Source;

			public VideoAspectRatio AspectRatio;

			[Range(0f, 10f)]
			public float PlaybackSpeed;

			public VideoClip VideoClip;

			public string URL;

			public VideoEntry()
			{
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				PlaybackSpeed = 1f;
				Source = 0;
				AspectRatio = 3;
				VideoClip = null;
				URL = null;
			}
		}

		public VideoEntry[] Videos = new VideoEntry[1];

		public static Action<VRC_SyncVideoPlayer> _Play;

		public static Action<VRC_SyncVideoPlayer, int> _PlayIndex;

		public static Action<VRC_SyncVideoPlayer> _Stop;

		public static Action<VRC_SyncVideoPlayer> _Pause;

		public static Action<VRC_SyncVideoPlayer> _Next;

		public static Action<VRC_SyncVideoPlayer> _Previous;

		public static Action<VRC_SyncVideoPlayer> _Shuffle;

		public static Action<VRC_SyncVideoPlayer> _Clear;

		public static Action<VRC_SyncVideoPlayer, string> _AddURL;

		public static Action<VRC_SyncVideoPlayer> _SpeedUp;

		public static Action<VRC_SyncVideoPlayer> _SpeedDown;

		public static Action<VRC_SyncVideoPlayer> Initialize;

		public VRC_SyncVideoPlayer()
			: this()
		{
		}

		[RPC(new VRC_EventHandler.VrcTargetType[]
		{
			VRC_EventHandler.VrcTargetType.Owner
		})]
		public void Play()
		{
			if (_Play != null)
			{
				_Play(this);
			}
		}

		[RPC(new VRC_EventHandler.VrcTargetType[]
		{
			VRC_EventHandler.VrcTargetType.Owner
		})]
		public void PlayIndex(int index)
		{
			if (_PlayIndex != null)
			{
				_PlayIndex(this, index);
			}
		}

		[RPC(new VRC_EventHandler.VrcTargetType[]
		{
			VRC_EventHandler.VrcTargetType.Owner
		})]
		public void Stop()
		{
			if (_Stop != null)
			{
				_Stop(this);
			}
		}

		[RPC(new VRC_EventHandler.VrcTargetType[]
		{
			VRC_EventHandler.VrcTargetType.Owner
		})]
		public void Pause()
		{
			if (_Pause != null)
			{
				_Pause(this);
			}
		}

		[RPC(new VRC_EventHandler.VrcTargetType[]
		{
			VRC_EventHandler.VrcTargetType.Owner
		})]
		public void Next()
		{
			if (_Next != null)
			{
				_Next(this);
			}
		}

		[RPC(new VRC_EventHandler.VrcTargetType[]
		{
			VRC_EventHandler.VrcTargetType.Owner
		})]
		public void Previous()
		{
			if (_Previous != null)
			{
				_Previous(this);
			}
		}

		[RPC(new VRC_EventHandler.VrcTargetType[]
		{
			VRC_EventHandler.VrcTargetType.Owner
		})]
		public void Shuffle()
		{
			if (_Shuffle != null)
			{
				_Shuffle(this);
			}
		}

		[RPC(new VRC_EventHandler.VrcTargetType[]
		{
			VRC_EventHandler.VrcTargetType.Owner
		})]
		public void Clear()
		{
			if (_Clear != null)
			{
				_Clear(this);
			}
		}

		[RPC(new VRC_EventHandler.VrcTargetType[]
		{
			VRC_EventHandler.VrcTargetType.Owner
		})]
		public void AddURL(string url)
		{
			if (_AddURL != null)
			{
				_AddURL(this, url);
			}
		}

		[RPC(new VRC_EventHandler.VrcTargetType[]
		{
			VRC_EventHandler.VrcTargetType.Owner
		})]
		public void SpeedUp()
		{
			if (_SpeedUp != null)
			{
				_SpeedUp(this);
			}
		}

		[RPC(new VRC_EventHandler.VrcTargetType[]
		{
			VRC_EventHandler.VrcTargetType.Owner
		})]
		public void SpeedDown()
		{
			if (_SpeedDown != null)
			{
				_SpeedDown(this);
			}
		}

		private void Awake()
		{
			if (Initialize != null)
			{
				Initialize(this);
			}
		}

		public IEnumerable<VRC_EventHandler.VrcEvent> ProvideEvents()
		{
			List<VRC_EventHandler.VrcEvent> list = new List<VRC_EventHandler.VrcEvent>();
			list.Add(new VRC_EventHandler.VrcEvent
			{
				Name = "Play",
				EventType = VRC_EventHandler.VrcEventType.SendRPC,
				ParameterInt = 2,
				ParameterString = "Play",
				ParameterObjects = (GameObject[])new GameObject[1]
				{
					this.get_gameObject()
				}
			});
			list.Add(new VRC_EventHandler.VrcEvent
			{
				Name = "Pause",
				EventType = VRC_EventHandler.VrcEventType.SendRPC,
				ParameterInt = 2,
				ParameterString = "Pause",
				ParameterObjects = (GameObject[])new GameObject[1]
				{
					this.get_gameObject()
				}
			});
			list.Add(new VRC_EventHandler.VrcEvent
			{
				Name = "Stop",
				EventType = VRC_EventHandler.VrcEventType.SendRPC,
				ParameterInt = 2,
				ParameterString = "Stop",
				ParameterObjects = (GameObject[])new GameObject[1]
				{
					this.get_gameObject()
				}
			});
			list.Add(new VRC_EventHandler.VrcEvent
			{
				Name = "Next",
				EventType = VRC_EventHandler.VrcEventType.SendRPC,
				ParameterInt = 2,
				ParameterString = "Next",
				ParameterObjects = (GameObject[])new GameObject[1]
				{
					this.get_gameObject()
				}
			});
			list.Add(new VRC_EventHandler.VrcEvent
			{
				Name = "Previous",
				EventType = VRC_EventHandler.VrcEventType.SendRPC,
				ParameterInt = 2,
				ParameterString = "Previous",
				ParameterObjects = (GameObject[])new GameObject[1]
				{
					this.get_gameObject()
				}
			});
			list.Add(new VRC_EventHandler.VrcEvent
			{
				Name = "Shuffle",
				EventType = VRC_EventHandler.VrcEventType.SendRPC,
				ParameterInt = 2,
				ParameterString = "Shuffle",
				ParameterObjects = (GameObject[])new GameObject[1]
				{
					this.get_gameObject()
				}
			});
			list.Add(new VRC_EventHandler.VrcEvent
			{
				Name = "SpeedUp",
				EventType = VRC_EventHandler.VrcEventType.SendRPC,
				ParameterInt = 2,
				ParameterString = "SpeedUp",
				ParameterObjects = (GameObject[])new GameObject[1]
				{
					this.get_gameObject()
				}
			});
			list.Add(new VRC_EventHandler.VrcEvent
			{
				Name = "SpeedDown",
				EventType = VRC_EventHandler.VrcEventType.SendRPC,
				ParameterInt = 2,
				ParameterString = "SpeedDown",
				ParameterObjects = (GameObject[])new GameObject[1]
				{
					this.get_gameObject()
				}
			});
			return list;
		}
	}
}
