using System.Collections.Generic;
using UnityEngine;

namespace VRCSDK2
{
	public class VRC_JukeBox : MonoBehaviour
	{
		public bool AutoPlay;

		public bool Shuffle;

		public AudioClip[] Songs;

		private int PlayingSong;

		private List<int> SongLog = new List<int>();

		private AudioSource Speakers;

		public VRC_JukeBox()
			: this()
		{
		}

		private void Start()
		{
			Speakers = this.GetComponent<AudioSource>();
			if (AutoPlay)
			{
				PlayNextSong();
			}
		}

		private void Update()
		{
			if (Speakers != null && Speakers.get_clip() != null && (double)Speakers.get_time() >= (double)Speakers.get_clip().get_length() - 0.01)
			{
				PlayNextSong();
			}
		}

		private void PlayNextSong(int Instigator = 0)
		{
			AudioClip val = null;
			if (PlayingSong <= -1)
			{
				PlayingSong++;
				val = Songs[SongLog[SongLog.Count - 1 + PlayingSong]];
			}
			else
			{
				if (Shuffle)
				{
					int num = Random.Range(0, Songs.Length - 1);
					if (num >= PlayingSong)
					{
						num++;
					}
					PlayingSong = num;
				}
				else
				{
					if (Speakers.get_clip() != null)
					{
						PlayingSong++;
					}
					if (PlayingSong >= Songs.Length)
					{
						PlayingSong = 0;
					}
				}
				SongLog.Add(PlayingSong);
				val = Songs[PlayingSong];
			}
			if (Speakers != null)
			{
				Speakers.set_clip(val);
				Speakers.Play();
			}
		}

		private void PlayPreviousSong(int Instigator = 0)
		{
			if (PlayingSong < 0)
			{
				if (SongLog.Count > -(PlayingSong - 1))
				{
					PlayingSong--;
				}
			}
			else if (SongLog.Count > 1)
			{
				PlayingSong = -1;
			}
			if (Speakers != null)
			{
				Speakers.set_clip(Songs[SongLog[SongLog.Count - 1 + PlayingSong]]);
				Speakers.Play();
			}
		}
	}
}
