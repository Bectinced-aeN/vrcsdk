using System;
using UnityEngine;
using VRC.Core.BestHTTP.Examples;
using VRC.Core.BestHTTP.SignalR;
using VRC.Core.BestHTTP.SignalR.Hubs;

internal sealed class ConnectionStatusSample : MonoBehaviour
{
	private readonly Uri URI = new Uri("https://besthttpsignalr.azurewebsites.net/signalr");

	private Connection signalRConnection;

	private GUIMessageList messages = new GUIMessageList();

	public ConnectionStatusSample()
		: this()
	{
	}

	private void Start()
	{
		signalRConnection = new Connection(URI, "StatusHub");
		signalRConnection.OnNonHubMessage += signalRConnection_OnNonHubMessage;
		signalRConnection.OnError += signalRConnection_OnError;
		signalRConnection.OnStateChanged += signalRConnection_OnStateChanged;
		signalRConnection["StatusHub"].OnMethodCall += statusHub_OnMethodCall;
		signalRConnection.Open();
	}

	private void OnDestroy()
	{
		signalRConnection.Close();
	}

	private void OnGUI()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		GUIHelper.DrawArea(GUIHelper.ClientArea, drawHeader: true, delegate
		{
			GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
			if (GUILayout.Button("START", (GUILayoutOption[])new GUILayoutOption[0]) && signalRConnection.State != ConnectionStates.Connected)
			{
				signalRConnection.Open();
			}
			if (GUILayout.Button("STOP", (GUILayoutOption[])new GUILayoutOption[0]) && signalRConnection.State == ConnectionStates.Connected)
			{
				signalRConnection.Close();
				messages.Clear();
			}
			if (GUILayout.Button("PING", (GUILayoutOption[])new GUILayoutOption[0]) && signalRConnection.State == ConnectionStates.Connected)
			{
				signalRConnection["StatusHub"].Call("Ping");
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(20f);
			GUILayout.Label("Connection Status Messages", (GUILayoutOption[])new GUILayoutOption[0]);
			GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
			GUILayout.Space(20f);
			messages.Draw((float)(Screen.get_width() - 20), 0f);
			GUILayout.EndHorizontal();
		});
	}

	private void signalRConnection_OnNonHubMessage(Connection manager, object data)
	{
		messages.Add("[Server Message] " + data.ToString());
	}

	private void signalRConnection_OnStateChanged(Connection manager, ConnectionStates oldState, ConnectionStates newState)
	{
		messages.Add($"[State Change] {oldState} => {newState}");
	}

	private void signalRConnection_OnError(Connection manager, string error)
	{
		messages.Add("[Error] " + error);
	}

	private void statusHub_OnMethodCall(Hub hub, string method, params object[] args)
	{
		string arg = (args.Length <= 0) ? string.Empty : (args[0] as string);
		string arg2 = (args.Length <= 1) ? string.Empty : args[1].ToString();
		switch (method)
		{
		case "joined":
			messages.Add($"[{hub.Name}] {arg} joined at {arg2}");
			break;
		case "rejoined":
			messages.Add($"[{hub.Name}] {arg} reconnected at {arg2}");
			break;
		case "leave":
			messages.Add($"[{hub.Name}] {arg} leaved at {arg2}");
			break;
		default:
			messages.Add($"[{hub.Name}] {method}");
			break;
		}
	}
}
