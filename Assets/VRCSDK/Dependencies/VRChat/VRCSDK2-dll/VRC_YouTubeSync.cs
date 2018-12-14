using System;
using UnityEngine;
using VRCSDK2;

[RequireComponent(typeof(VRC_SyncVideoPlayer))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(VRC_EventHandler))]
public class VRC_YouTubeSync : MonoBehaviour
{
	[HideInInspector]
	public string videoID;

	[HideInInspector]
	public bool hideUI;

	[HideInInspector]
	public bool loop;

	[HideInInspector]
	public bool autoplay = true;

	[HideInInspector]
	public int volume = 25;

	public string[] playlist;

	public static Action<VRC_YouTubeSync> _init;

	public VRC_YouTubeSync()
		: this()
	{
	}

	public void Awake()
	{
		if (_init != null)
		{
			_init(this);
		}
	}
}
