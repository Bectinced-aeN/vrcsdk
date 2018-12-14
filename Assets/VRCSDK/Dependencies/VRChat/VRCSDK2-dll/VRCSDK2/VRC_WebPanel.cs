using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace VRCSDK2
{
	[RequireComponent(typeof(AudioSource))]
	[ExecuteInEditMode]
	public class VRC_WebPanel : VRC_Interactable, IVRCEventProvider
	{
		[Serializable]
		public class WebFile
		{
			public byte[] data;

			public string path;
		}

		public delegate void InitializeDelegate(VRC_WebPanel obj);

		public string webRoot = "WebRoot";

		public string defaultUrl = "http://api.vrchat.cloud/public/blank.html";

		public int resolutionWidth = 1280;

		public int resolutionHeight = 720;

		public Rect displayRegion = new Rect(0f, 0f, 1f, 1f);

		public bool interactive;

		public Material[] extraVideoScreens;

		public new static InitializeDelegate Initialize;

		public bool localOnly;

		public bool syncURI;

		public bool syncInput;

		public bool syncDisplayAndAudio;

		public GameObject cursor;

		public bool transparent;

		public VRC_Station station;

		public bool cookiesEnabled;

		public bool autoFormSubmit = true;

		[HideInInspector]
		public List<WebFile> webData;

		public Action<string> _NavigateTo;

		public Action _WebPanelForward;

		public Action _WebPanelBackward;

		public Action _WebPanelReload;

		public Action<string> _ExecuteScript;

		public Func<string, Delegate, bool> _BindCall;

		private string WebRootPath => (webRoot != null && !(webRoot.Trim() == string.Empty)) ? (Application.get_dataPath() + webRoot.Replace('/', Path.DirectorySeparatorChar)) : null;

		[RPC(new VRC_EventHandler.VrcTargetType[]
		{

		})]
		public void WebPanelForward()
		{
			if (_WebPanelForward != null)
			{
				_WebPanelForward();
			}
		}

		[RPC(new VRC_EventHandler.VrcTargetType[]
		{

		})]
		public void WebPanelBackward()
		{
			if (_WebPanelBackward != null)
			{
				_WebPanelBackward();
			}
		}

		[RPC(new VRC_EventHandler.VrcTargetType[]
		{

		})]
		public void WebPanelReload()
		{
			if (_WebPanelReload != null)
			{
				_WebPanelReload();
			}
		}

		public bool BindCall(string function, Delegate handler)
		{
			if (_BindCall == null)
			{
				return false;
			}
			return _BindCall(function, handler);
		}

		public void NavigateTo(string uri)
		{
			if (_NavigateTo != null)
			{
				_NavigateTo(uri);
			}
		}

		public override void Awake()
		{
			base.Awake();
			if (Application.get_isPlaying() && Initialize != null)
			{
				Initialize(this);
			}
		}

		public override void Interact()
		{
		}

		public IEnumerable<VRC_EventHandler.VrcEvent> ProvideEvents()
		{
			List<VRC_EventHandler.VrcEvent> list = new List<VRC_EventHandler.VrcEvent>();
			VRC_EventHandler.VrcEvent vrcEvent = new VRC_EventHandler.VrcEvent();
			vrcEvent.Name = "SetWebPanelURI";
			vrcEvent.ParameterInt = 7;
			vrcEvent.EventType = VRC_EventHandler.VrcEventType.SetWebPanelURI;
			vrcEvent.ParameterString = "http://example.com";
			vrcEvent.ParameterObjects = (GameObject[])new GameObject[1]
			{
				this.get_gameObject()
			};
			list.Add(vrcEvent);
			vrcEvent = new VRC_EventHandler.VrcEvent();
			vrcEvent.Name = "SetWebPanelVolume";
			vrcEvent.ParameterInt = 7;
			vrcEvent.EventType = VRC_EventHandler.VrcEventType.SetWebPanelVolume;
			vrcEvent.ParameterFloat = 0.5f;
			vrcEvent.ParameterObjects = (GameObject[])new GameObject[1]
			{
				this.get_gameObject()
			};
			list.Add(vrcEvent);
			vrcEvent = new VRC_EventHandler.VrcEvent();
			vrcEvent.Name = "WebPanelForward";
			vrcEvent.ParameterInt = 6;
			vrcEvent.EventType = VRC_EventHandler.VrcEventType.SendRPC;
			vrcEvent.ParameterString = "WebPanelForward";
			vrcEvent.ParameterObjects = (GameObject[])new GameObject[1]
			{
				this.get_gameObject()
			};
			list.Add(vrcEvent);
			vrcEvent = new VRC_EventHandler.VrcEvent();
			vrcEvent.Name = "WebPanelBackward";
			vrcEvent.ParameterInt = 6;
			vrcEvent.EventType = VRC_EventHandler.VrcEventType.SendRPC;
			vrcEvent.ParameterString = "WebPanelBackward";
			vrcEvent.ParameterObjects = (GameObject[])new GameObject[1]
			{
				this.get_gameObject()
			};
			list.Add(vrcEvent);
			vrcEvent = new VRC_EventHandler.VrcEvent();
			vrcEvent.Name = "WebPanelReload";
			vrcEvent.ParameterInt = 6;
			vrcEvent.EventType = VRC_EventHandler.VrcEventType.SendRPC;
			vrcEvent.ParameterString = "WebPanelReload";
			vrcEvent.ParameterObjects = (GameObject[])new GameObject[1]
			{
				this.get_gameObject()
			};
			list.Add(vrcEvent);
			return list;
		}

		private void ReadData(string root, int junk)
		{
			string[] files = Directory.GetFiles(root);
			string[] array = files;
			foreach (string text in array)
			{
				if (!text.EndsWith(".meta") && !text.EndsWith(".unity"))
				{
					webData.Add(new WebFile
					{
						path = text.Substring(Application.get_dataPath().Length + 1).Replace(Path.DirectorySeparatorChar, '/'),
						data = File.ReadAllBytes(text)
					});
				}
			}
			string[] directories = Directory.GetDirectories(root);
			string[] array2 = directories;
			foreach (string root2 in array2)
			{
				ReadData(root2, 0);
			}
		}

		public void ImportWebData()
		{
			webData = new List<WebFile>();
			string webRootPath = WebRootPath;
			if (!string.IsNullOrEmpty(webRootPath) && Directory.Exists(webRootPath) && (!webRootPath.StartsWith(Application.get_dataPath() + Path.DirectorySeparatorChar + "VRCSDK") || webRootPath == Application.get_dataPath() + Path.DirectorySeparatorChar + "VRCSDK" + Path.DirectorySeparatorChar + "Examples" + Path.DirectorySeparatorChar + "Sample Assets" + Path.DirectorySeparatorChar + "WebRoot"))
			{
				ReadData(webRootPath, 0);
				string str = string.Join(", ", webData.ConvertAll((WebFile item) => item.path + " [" + item.data.Length + "]").ToArray());
				Debug.Log((object)("Web Panel has files: \n" + str));
			}
		}
	}
}
