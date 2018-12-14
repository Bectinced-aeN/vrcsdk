using System;
using System.Collections.Generic;
using UnityEngine;

namespace VRCSDK2
{
	[RequireComponent(typeof(AudioSource))]
	[ExecuteInEditMode]
	public class VRC_WebPanel : MonoBehaviour, INetworkID, IVRCEventProvider
	{
		[Serializable]
		public class WebFile
		{
			public byte[] data;

			public string path;
		}

		public delegate void InitializeDelegate(VRC_WebPanel obj);

		public string webRoot = "WebRoot";

		public string virtualHost;

		public string defaultUrl = "http://api.vrchat.cloud/public/blank.html";

		public int resolutionWidth = 1280;

		public int resolutionHeight = 720;

		public Rect displayRegion = new Rect(0f, 0f, 1f, 1f);

		public bool interactive;

		public Material[] extraVideoScreens;

		public static InitializeDelegate Initialize;

		public bool syncURI;

		public bool syncInput;

		public bool syncDisplayAndAudio;

		public GameObject cursor;

		public bool transparent;

		public VRC_Station station;

		public bool cookiesEnabled;

		[HideInInspector]
		public int networkId;

		[HideInInspector]
		public List<WebFile> webData;

		public Action _WebPanelForward;

		public Action _WebPanelBackward;

		public Action _WebPanelReload;

		public Action<string> _ExecuteScript;

		[HideInInspector]
		public int NetworkID
		{
			get
			{
				return networkId;
			}
			set
			{
				networkId = value;
			}
		}

		public VRC_WebPanel()
			: this()
		{
		}//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)


		public void WebPanelForward()
		{
			if (_WebPanelForward != null)
			{
				_WebPanelForward();
			}
		}

		public void WebPanelBackward()
		{
			if (_WebPanelBackward != null)
			{
				_WebPanelBackward();
			}
		}

		public void WebPanelReload()
		{
			if (_WebPanelReload != null)
			{
				_WebPanelReload();
			}
		}

		public void ExecuteScript(string script)
		{
			if (_ExecuteScript != null)
			{
				_ExecuteScript(script);
			}
		}

		private void Start()
		{
			if (Application.get_isPlaying() && Initialize != null)
			{
				Initialize(this);
			}
		}

		public IEnumerable<VRC_EventHandler.VrcEvent> ProvideEvents()
		{
			List<VRC_EventHandler.VrcEvent> list = new List<VRC_EventHandler.VrcEvent>();
			VRC_EventHandler.VrcEvent vrcEvent = new VRC_EventHandler.VrcEvent();
			vrcEvent.Name = "SetWebPanelURI";
			vrcEvent.EventType = VRC_EventHandler.VrcEventType.SetWebPanelURI;
			vrcEvent.ParameterString = "http://example.com";
			vrcEvent.ParameterObjects = (GameObject[])new GameObject[1]
			{
				this.get_gameObject()
			};
			list.Add(vrcEvent);
			vrcEvent = new VRC_EventHandler.VrcEvent();
			vrcEvent.Name = "SetWebPanelVolume";
			vrcEvent.EventType = VRC_EventHandler.VrcEventType.SetWebPanelVolume;
			vrcEvent.ParameterFloat = 0.5f;
			vrcEvent.ParameterObjects = (GameObject[])new GameObject[1]
			{
				this.get_gameObject()
			};
			list.Add(vrcEvent);
			vrcEvent = new VRC_EventHandler.VrcEvent();
			vrcEvent.Name = "WebPanelForward";
			vrcEvent.EventType = VRC_EventHandler.VrcEventType.SendRPC;
			vrcEvent.ParameterString = "WebPanelForward";
			vrcEvent.ParameterObjects = (GameObject[])new GameObject[1]
			{
				this.get_gameObject()
			};
			list.Add(vrcEvent);
			vrcEvent = new VRC_EventHandler.VrcEvent();
			vrcEvent.Name = "WebPanelBackward";
			vrcEvent.EventType = VRC_EventHandler.VrcEventType.SendRPC;
			vrcEvent.ParameterString = "WebPanelBackward";
			vrcEvent.ParameterObjects = (GameObject[])new GameObject[1]
			{
				this.get_gameObject()
			};
			list.Add(vrcEvent);
			vrcEvent = new VRC_EventHandler.VrcEvent();
			vrcEvent.Name = "WebPanelReload";
			vrcEvent.EventType = VRC_EventHandler.VrcEventType.SendRPC;
			vrcEvent.ParameterString = "WebPanelReload";
			vrcEvent.ParameterObjects = (GameObject[])new GameObject[1]
			{
				this.get_gameObject()
			};
			list.Add(vrcEvent);
			return list;
		}
	}
}
