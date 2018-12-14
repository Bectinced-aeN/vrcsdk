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
		VRC_EventHandler component = this.GetComponent<VRC_EventHandler>();
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
		if (playlist != null && playlist.Length != 0)
		{
			str = str + str2 + "playlist=" + string.Join(",", playlist);
			str2 = "&";
		}
		string str3 = "hide_ui=false";
		if (hideUI)
		{
			str3 = "hide_ui=true";
		}
		str = str + str2 + str3;
		str2 = "&";
		string str4 = "loop=false";
		if (loop)
		{
			str4 = "loop=true";
		}
		str = str + str2 + str4;
		str2 = "&";
		string str5 = "autoplay=false";
		if (autoplay)
		{
			str5 = "autoplay=true";
		}
		str = str + str2 + str5;
		str2 = "&";
		str = str + str2 + "volume=" + volume.ToString();
		VRC_EventHandler.VrcEvent vrcEvent = new VRC_EventHandler.VrcEvent();
		vrcEvent.EventType = VRC_EventHandler.VrcEventType.SetWebPanelURI;
		vrcEvent.Name = "SetWebPanelURI";
		vrcEvent.ParameterString = urlPrefix + str;
		vrcEvent.ParameterObjects = (GameObject[])new GameObject[1]
		{
			this.get_gameObject()
		};
		component.TriggerEvent(vrcEvent, VRC_EventHandler.VrcBroadcastType.Local, 0, 0f);
		Debug.Log((object)("Setting youtube URL: " + urlPrefix + str));
	}

	private void OnNetworkReady()
	{
		if (Networking.IsMaster)
		{
			string randomDigits = GetRandomDigits(8);
			Networking.RPC(VRC_EventHandler.VrcTargetType.AllBuffered, this.get_gameObject(), "SetupYoutube", randomDigits);
		}
	}

	private string GetRandomDigits(int digits)
	{
		string text = string.Empty;
		for (int i = 0; i < digits; i++)
		{
			text += Random.Range(0, 9).ToString();
		}
		return text;
	}
}
