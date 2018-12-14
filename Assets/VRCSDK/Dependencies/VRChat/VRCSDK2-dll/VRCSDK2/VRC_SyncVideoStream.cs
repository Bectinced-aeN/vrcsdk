using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace VRCSDK2
{
	public class VRC_SyncVideoStream : MonoBehaviour, INetworkID, IVRCEventProvider
	{
		[Serializable]
		public enum VideoSyncType
		{
			Normal,
			LiveStream,
			Karaoke
		}

		[Serializable]
		public class VideoEntry
		{
			public VideoSource Source;

			public VideoClip VideoClip;

			[Range(0f, 10f)]
			public float PlaybackSpeed;

			public string URL;

			public VideoSyncType SyncType;

			public float SyncMinutes;

			public VideoEntry()
			{
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				PlaybackSpeed = 1f;
				URL = null;
				SyncType = VideoSyncType.Normal;
				Source = 0;
				VideoClip = null;
				SyncMinutes = 0f;
			}
		}

		public bool AllowNonOwnerControl = true;

		public string VideoSearchRoot = "youtube.com";

		public bool AutoStart;

		public VideoEntry[] Videos = new VideoEntry[1];

		public static Action<VRC_SyncVideoStream> _Play;

		public static Action<VRC_SyncVideoStream, int> _PlayIndex;

		public static Action<VRC_SyncVideoStream> _Stop;

		public static Action<VRC_SyncVideoStream> _Pause;

		public static Action<VRC_SyncVideoStream> _Next;

		public static Action<VRC_SyncVideoStream> _Previous;

		public static Action<VRC_SyncVideoStream> _Shuffle;

		public static Action<VRC_SyncVideoStream> _Clear;

		public static Action<VRC_SyncVideoStream, string> _AddURL;

		public static Action<VRC_SyncVideoStream, float> _FastForwardSeconds;

		public static Action<VRC_SyncVideoStream, float> _RewindSeconds;

		public static Action<VRC_SyncVideoStream> _LocalResync;

		public static Action<VRC_SyncVideoStream, VideoSyncType> _SetSyncType;

		public static Action<VRC_SyncVideoStream, float> _SetSyncMinutes;

		public static Action<VRC_SyncVideoStream> Initialize;

		[HideInInspector]
		public int NetworkID
		{
			get;
			set;
		}

		public VRC_SyncVideoStream()
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
			VRC_EventHandler.VrcTargetType.Owner,
			VRC_EventHandler.VrcTargetType.Local
		})]
		public void FastForwardSeconds(float secs)
		{
			if (_FastForwardSeconds != null)
			{
				_FastForwardSeconds(this, secs);
			}
		}

		[RPC(new VRC_EventHandler.VrcTargetType[]
		{
			VRC_EventHandler.VrcTargetType.Owner,
			VRC_EventHandler.VrcTargetType.Local
		})]
		public void RewindSeconds(float secs)
		{
			if (_RewindSeconds != null)
			{
				_RewindSeconds(this, secs);
			}
		}

		[RPC(new VRC_EventHandler.VrcTargetType[]
		{
			VRC_EventHandler.VrcTargetType.Local
		})]
		public void Resync()
		{
			if (_LocalResync != null)
			{
				_LocalResync(this);
			}
		}

		[RPC(new VRC_EventHandler.VrcTargetType[]
		{
			VRC_EventHandler.VrcTargetType.Owner
		})]
		public void SetSyncType(VideoSyncType syncType)
		{
			if (_SetSyncType != null)
			{
				_SetSyncType(this, syncType);
			}
		}

		[RPC(new VRC_EventHandler.VrcTargetType[]
		{
			VRC_EventHandler.VrcTargetType.Owner
		})]
		public void SetSyncMinutes(float syncMins)
		{
			if (_SetSyncMinutes != null)
			{
				_SetSyncMinutes(this, syncMins);
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
				Name = "Local Resync",
				EventType = VRC_EventHandler.VrcEventType.SendRPC,
				ParameterInt = 6,
				ParameterString = "Resync",
				ParameterObjects = (GameObject[])new GameObject[1]
				{
					this.get_gameObject()
				}
			});
			list.Add(new VRC_EventHandler.VrcEvent
			{
				Name = "Fast Forward",
				EventType = VRC_EventHandler.VrcEventType.SendRPC,
				ParameterInt = 2,
				ParameterString = "FastForwardSeconds",
				ParameterObjects = (GameObject[])new GameObject[1]
				{
					this.get_gameObject()
				}
			});
			list.Add(new VRC_EventHandler.VrcEvent
			{
				Name = "Rewind",
				EventType = VRC_EventHandler.VrcEventType.SendRPC,
				ParameterInt = 2,
				ParameterString = "RewindSeconds",
				ParameterObjects = (GameObject[])new GameObject[1]
				{
					this.get_gameObject()
				}
			});
			list.Add(new VRC_EventHandler.VrcEvent
			{
				Name = "Set Sync Type",
				EventType = VRC_EventHandler.VrcEventType.SendRPC,
				ParameterInt = 2,
				ParameterString = "SetSyncType",
				ParameterObjects = (GameObject[])new GameObject[1]
				{
					this.get_gameObject()
				}
			});
			list.Add(new VRC_EventHandler.VrcEvent
			{
				Name = "Set AutoSync Period",
				EventType = VRC_EventHandler.VrcEventType.SendRPC,
				ParameterInt = 2,
				ParameterString = "SetSyncMinutes",
				ParameterObjects = (GameObject[])new GameObject[1]
				{
					this.get_gameObject()
				}
			});
			return list;
		}
	}
}
