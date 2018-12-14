using UnityEngine;
using VRCSDK2;

[RequireComponent(typeof(VRC_EventHandler))]
[RequireComponent(typeof(VRC_WebPanel))]
public class VRC_YouTubeSync : MonoBehaviour
{
	[HideInInspector]
	public string screenName;

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

	private string urlPrefix = "http://api.vrchat.cloud/public/youtube.html";

	private VRC_WebPanel web;

	public VRC_YouTubeSync()
		: this()
	{
	}

	private void Awake()
	{
		web = this.GetComponent<VRC_WebPanel>();
		if (web == null)
		{
			Object.Destroy(this);
		}
		web.defaultUrl = "http://api.vrchat.cloud/public/blank.html";
	}

	private void SetupYoutube(string roomScreenName)
	{
		screenName = roomScreenName;
		string str = "?";
		string str2 = string.Empty;
		if (screenName != string.Empty)
		{
			str = str + "s=" + screenName;
			str2 = "&";
		}
		if (videoID != string.Empty)
		{
			str = str + str2 + "v=" + videoID;
			str2 = "&";
		}
		string str3 = WWW.EscapeURL(Networking.LocalPlayer.name);
		str = str + str2 + "user=" + str3;
		str2 = "&";
		if (playlist != null && playlist.Length != 0)
		{
			str = str + str2 + "playlist=" + string.Join(",", playlist);
			str2 = "&";
		}
		string str4 = "hide_ui=false";
		if (hideUI)
		{
			str4 = "hide_ui=true";
		}
		str = str + str2 + str4;
		str2 = "&";
		string str5 = "loop=false";
		if (loop)
		{
			str5 = "loop=true";
		}
		str = str + str2 + str5;
		str2 = "&";
		string str6 = "autoplay=false";
		if (autoplay)
		{
			str6 = "autoplay=true";
		}
		str = str + str2 + str6;
		str2 = "&";
		str = str + str2 + "volume=" + volume.ToString();
		web.NavigateTo(urlPrefix + str);
	}

	private void OnNetworkReady()
	{
		SetupYoutube(Networking.GetUniqueName(this.get_gameObject()));
	}
}
