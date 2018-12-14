using System;
using System.Collections.Generic;
using UnityEngine;
using VRC.Core.BestHTTP.Examples;
using VRC.Core.BestHTTP.SocketIO;

internal sealed class SocketIOWePlaySample : MonoBehaviour
{
	private enum States
	{
		Connecting,
		WaitForNick,
		Joined
	}

	private const float ratio = 1.5f;

	private string[] controls = new string[8]
	{
		"left",
		"right",
		"a",
		"b",
		"up",
		"down",
		"select",
		"start"
	};

	private int MaxMessages = 50;

	private States State;

	private Socket Socket;

	private string Nick = string.Empty;

	private string messageToSend = string.Empty;

	private int connections;

	private List<string> messages = new List<string>();

	private Vector2 scrollPos;

	private Texture2D FrameTexture;

	public SocketIOWePlaySample()
		: this()
	{
	}

	private void Start()
	{
		SocketOptions socketOptions = new SocketOptions();
		socketOptions.AutoConnect = false;
		SocketManager socketManager = new SocketManager(new Uri("http://io.weplay.io/socket.io/"), socketOptions);
		Socket = socketManager.Socket;
		Socket.On(SocketIOEventTypes.Connect, OnConnected);
		Socket.On("joined", OnJoined);
		Socket.On("connections", OnConnections);
		Socket.On("join", OnJoin);
		Socket.On("move", OnMove);
		Socket.On("message", OnMessage);
		Socket.On("reload", OnReload);
		Socket.On("frame", OnFrame, autoDecodePayload: false);
		Socket.On(SocketIOEventTypes.Error, OnError);
		socketManager.Open();
		State = States.Connecting;
	}

	private void OnDestroy()
	{
		Socket.Manager.Close();
	}

	private void Update()
	{
		if (Input.GetKeyDown(27))
		{
			SampleSelector.SelectedSample.DestroyUnityObject();
		}
	}

	private void OnGUI()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		switch (State)
		{
		case States.Connecting:
			GUIHelper.DrawArea(GUIHelper.ClientArea, drawHeader: true, delegate
			{
				GUILayout.BeginVertical((GUILayoutOption[])new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				GUIHelper.DrawCenteredText("Connecting to the server...");
				GUILayout.FlexibleSpace();
				GUILayout.EndVertical();
			});
			break;
		case States.WaitForNick:
			GUIHelper.DrawArea(GUIHelper.ClientArea, drawHeader: true, delegate
			{
				DrawLoginScreen();
			});
			break;
		case States.Joined:
			GUIHelper.DrawArea(GUIHelper.ClientArea, drawHeader: true, delegate
			{
				if (FrameTexture != null)
				{
					GUILayout.Box(FrameTexture, (GUILayoutOption[])new GUILayoutOption[0]);
				}
				DrawControls();
				DrawChat();
			});
			break;
		}
	}

	private void DrawLoginScreen()
	{
		GUILayout.BeginVertical((GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.FlexibleSpace();
		GUIHelper.DrawCenteredText("What's your nickname?");
		Nick = GUILayout.TextField(Nick, (GUILayoutOption[])new GUILayoutOption[0]);
		if (GUILayout.Button("Join", (GUILayoutOption[])new GUILayoutOption[0]))
		{
			Join();
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndVertical();
	}

	private void DrawControls()
	{
		GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.Label("Controls:", (GUILayoutOption[])new GUILayoutOption[0]);
		for (int i = 0; i < controls.Length; i++)
		{
			if (GUILayout.Button(controls[i], (GUILayoutOption[])new GUILayoutOption[0]))
			{
				Socket.Emit("move", controls[i]);
			}
		}
		GUILayout.Label(" Connections: " + connections, (GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.EndHorizontal();
	}

	private void DrawChat(bool withInput = true)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		GUILayout.BeginVertical((GUILayoutOption[])new GUILayoutOption[0]);
		scrollPos = GUILayout.BeginScrollView(scrollPos, false, false, (GUILayoutOption[])new GUILayoutOption[0]);
		for (int i = 0; i < messages.Count; i++)
		{
			GUILayout.Label(messages[i], (GUILayoutOption[])new GUILayoutOption[1]
			{
				GUILayout.MinWidth((float)Screen.get_width())
			});
		}
		GUILayout.EndScrollView();
		if (withInput)
		{
			GUILayout.Label("Your message: ", (GUILayoutOption[])new GUILayoutOption[0]);
			GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
			messageToSend = GUILayout.TextField(messageToSend, (GUILayoutOption[])new GUILayoutOption[0]);
			if (GUILayout.Button("Send", (GUILayoutOption[])new GUILayoutOption[1]
			{
				GUILayout.MaxWidth(100f)
			}))
			{
				SendMessage();
			}
			GUILayout.EndHorizontal();
		}
		GUILayout.EndVertical();
	}

	private void AddMessage(string msg)
	{
		messages.Insert(0, msg);
		if (messages.Count > MaxMessages)
		{
			messages.RemoveRange(MaxMessages, messages.Count - MaxMessages);
		}
	}

	private void SendMessage()
	{
		if (!string.IsNullOrEmpty(messageToSend))
		{
			Socket.Emit("message", messageToSend);
			AddMessage($"{Nick}: {messageToSend}");
			messageToSend = string.Empty;
		}
	}

	private void Join()
	{
		PlayerPrefs.SetString("Nick", Nick);
		Socket.Emit("join", Nick);
	}

	private void Reload()
	{
		FrameTexture = null;
		if (Socket != null)
		{
			Socket.Manager.Close();
			Socket = null;
			Start();
		}
	}

	private void OnConnected(Socket socket, Packet packet, params object[] args)
	{
		if (PlayerPrefs.HasKey("Nick"))
		{
			Nick = PlayerPrefs.GetString("Nick", "NickName");
			Join();
		}
		else
		{
			State = States.WaitForNick;
		}
		AddMessage("connected");
	}

	private void OnJoined(Socket socket, Packet packet, params object[] args)
	{
		State = States.Joined;
	}

	private void OnReload(Socket socket, Packet packet, params object[] args)
	{
		Reload();
	}

	private void OnMessage(Socket socket, Packet packet, params object[] args)
	{
		if (args.Length == 1)
		{
			AddMessage(args[0] as string);
		}
		else
		{
			AddMessage($"{args[1]}: {args[0]}");
		}
	}

	private void OnMove(Socket socket, Packet packet, params object[] args)
	{
		AddMessage($"{args[1]} pressed {args[0]}");
	}

	private void OnJoin(Socket socket, Packet packet, params object[] args)
	{
		string arg = (args.Length <= 1) ? string.Empty : $"({args[1]})";
		AddMessage($"{args[0]} joined {arg}");
	}

	private void OnConnections(Socket socket, Packet packet, params object[] args)
	{
		connections = Convert.ToInt32(args[0]);
	}

	private void OnFrame(Socket socket, Packet packet, params object[] args)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		if (State == States.Joined)
		{
			if (FrameTexture == null)
			{
				FrameTexture = new Texture2D(0, 0, 4, false);
				FrameTexture.set_filterMode(0);
			}
			byte[] array = packet.Attachments[0];
			FrameTexture.LoadImage(array);
		}
	}

	private void OnError(Socket socket, Packet packet, params object[] args)
	{
		AddMessage($"--ERROR - {args[0].ToString()}");
	}
}
