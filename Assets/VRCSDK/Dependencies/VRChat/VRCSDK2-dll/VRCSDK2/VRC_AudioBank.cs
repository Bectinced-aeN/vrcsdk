using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VRCSDK2
{
	public class VRC_AudioBank : MonoBehaviour, IVRCEventProvider
	{
		public enum Order
		{
			InOrder,
			InOrderReversing,
			Shuffle,
			Random
		}

		public enum Style
		{
			OneShot,
			Continuous
		}

		public Order PlaybackOrder;

		public Style PlaybackStyle;

		public AudioSource Source;

		[Range(-3f, 3f)]
		public float MinPitchRange = 1f;

		[Range(-3f, 3f)]
		public float MaxPitchRange = 1f;

		public VRC_Trigger.CustomTriggerTarget OnPlay;

		public VRC_Trigger.CustomTriggerTarget OnStop;

		public VRC_Trigger.CustomTriggerTarget OnChange;

		public AudioClip[] Clips = (AudioClip[])new AudioClip[0];

		private int[] playOrder = new int[0];

		private int current;

		private float remainingTime;

		private Random rng;

		public AudioClip Current
		{
			get
			{
				if (Clips.Length > 0 && Clips[playOrder[current]] != null)
				{
					return Clips[playOrder[current]];
				}
				return null;
			}
		}

		public int CurrentIdx => playOrder[current];

		public VRC_AudioBank()
			: this()
		{
		}

		[RPC(new VRC_EventHandler.VrcTargetType[]
		{

		})]
		public void Play(int idx = -1)
		{
			if (idx >= 0 && idx < Clips.Length)
			{
				idx = findCurrent(idx);
			}
			play(idx);
		}

		[RPC(new VRC_EventHandler.VrcTargetType[]
		{

		})]
		public void Stop()
		{
			remainingTime = 0f;
			if (Source != null)
			{
				Source.Stop();
			}
			VRC_Trigger.TriggerCustom(OnStop);
		}

		[RPC(new VRC_EventHandler.VrcTargetType[]
		{

		})]
		public void PlayNext()
		{
			int num = current;
			switch (PlaybackOrder)
			{
			case Order.InOrder:
			case Order.Shuffle:
				num = ((num < playOrder.Length) ? (num + 1) : 0);
				break;
			case Order.InOrderReversing:
				if (num < playOrder.Length)
				{
					num++;
				}
				else
				{
					for (int j = 0; j < playOrder.Length; j++)
					{
						int num2 = playOrder[j];
						playOrder[j] = playOrder[playOrder.Length - j - 1];
						playOrder[playOrder.Length - j - 1] = num2;
					}
					num = 0;
				}
				break;
			case Order.Random:
				num = (from i in playOrder
				orderby rng.Next()
				select i).First();
				break;
			}
			VRC_Trigger.TriggerCustom(OnChange);
			play(num);
		}

		[RPC(new VRC_EventHandler.VrcTargetType[]
		{
			VRC_EventHandler.VrcTargetType.All
		})]
		public void Shuffle()
		{
			playOrder = (from i in playOrder
			orderby rng.Next()
			select i).ToArray();
		}

		private void play(int idx)
		{
			Stop();
			current = idx;
			AudioClip val = Current;
			if (val != null)
			{
				if (Source != null)
				{
					Source.set_pitch(Random.Range(MinPitchRange, MaxPitchRange));
					Source.PlayOneShot(val);
				}
				remainingTime = val.get_length();
				VRC_Trigger.TriggerCustom(OnPlay);
			}
			else if (Clips.Length > 0 && Clips[playOrder[current]] == null)
			{
				Debug.LogError((object)"Failed to play because clip was null");
			}
		}

		private int findCurrent(int idx)
		{
			for (int i = 0; i < playOrder.Length; i++)
			{
				if (playOrder[i] == idx)
				{
					return i;
				}
			}
			return 0;
		}

		private void reset()
		{
			rng = new Random(Mathf.FloorToInt(Time.get_time()));
			playOrder = new int[Clips.Length];
			for (int i = 0; i < playOrder.Length; i++)
			{
				playOrder[i] = i;
			}
			if (PlaybackOrder == Order.Shuffle)
			{
				Shuffle();
			}
			current = findCurrent(current);
		}

		private void Awake()
		{
			reset();
			if (Source == null)
			{
				Source = this.GetComponent<AudioSource>();
			}
		}

		private void Update()
		{
			if (remainingTime > 0f)
			{
				remainingTime -= Time.get_deltaTime();
				if (remainingTime <= 0f)
				{
					switch (PlaybackStyle)
					{
					case Style.Continuous:
						PlayNext();
						break;
					case Style.OneShot:
						Stop();
						break;
					}
				}
			}
		}

		private void LateUpdate()
		{
			if (playOrder.Length != Clips.Length)
			{
				reset();
			}
		}

		public IEnumerable<VRC_EventHandler.VrcEvent> ProvideEvents()
		{
			List<VRC_EventHandler.VrcEvent> list = new List<VRC_EventHandler.VrcEvent>();
			VRC_EventHandler.VrcEvent vrcEvent = new VRC_EventHandler.VrcEvent();
			vrcEvent.Name = "Play";
			vrcEvent.EventType = VRC_EventHandler.VrcEventType.SendRPC;
			vrcEvent.ParameterString = "Play";
			vrcEvent.ParameterObjects = (GameObject[])new GameObject[1]
			{
				this.get_gameObject()
			};
			vrcEvent.ParameterBytes = VRC_Serialization.ParameterEncoder(-1);
			list.Add(vrcEvent);
			vrcEvent = new VRC_EventHandler.VrcEvent();
			vrcEvent.Name = "Stop";
			vrcEvent.EventType = VRC_EventHandler.VrcEventType.SendRPC;
			vrcEvent.ParameterString = "Stop";
			vrcEvent.ParameterObjects = (GameObject[])new GameObject[1]
			{
				this.get_gameObject()
			};
			list.Add(vrcEvent);
			vrcEvent = new VRC_EventHandler.VrcEvent();
			vrcEvent.Name = "PlayNext";
			vrcEvent.EventType = VRC_EventHandler.VrcEventType.SendRPC;
			vrcEvent.ParameterString = "PlayNext";
			vrcEvent.ParameterObjects = (GameObject[])new GameObject[1]
			{
				this.get_gameObject()
			};
			list.Add(vrcEvent);
			vrcEvent = new VRC_EventHandler.VrcEvent();
			vrcEvent.Name = "Shuffle";
			vrcEvent.EventType = VRC_EventHandler.VrcEventType.SendRPC;
			vrcEvent.ParameterString = "Shuffle";
			vrcEvent.ParameterObjects = (GameObject[])new GameObject[1]
			{
				this.get_gameObject()
			};
			list.Add(vrcEvent);
			return list;
		}
	}
}
