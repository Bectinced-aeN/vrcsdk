using System;
using UnityEngine;
using VRC.Core.BestHTTP.Examples;
using VRC.Core.BestHTTP.SignalR;

internal sealed class SimpleStreamingSample : MonoBehaviour
{
	private readonly Uri URI = new Uri("https://besthttpsignalr.azurewebsites.net/streaming-connection");

	private Connection signalRConnection;

	private GUIMessageList messages = new GUIMessageList();

	public SimpleStreamingSample()
		: this()
	{
	}

	private void Start()
	{
		signalRConnection = new Connection(URI);
		signalRConnection.OnNonHubMessage += signalRConnection_OnNonHubMessage;
		signalRConnection.OnStateChanged += signalRConnection_OnStateChanged;
		signalRConnection.OnError += signalRConnection_OnError;
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
			GUILayout.Label("Messages", (GUILayoutOption[])new GUILayoutOption[0]);
			GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
			GUILayout.Space(20f);
			messages.Draw((float)(Screen.get_width() - 20), 0f);
			GUILayout.EndHorizontal();
		});
	}

	private void signalRConnection_OnNonHubMessage(Connection connection, object data)
	{
		messages.Add("[Server Message] " + data.ToString());
	}

	private void signalRConnection_OnStateChanged(Connection connection, ConnectionStates oldState, ConnectionStates newState)
	{
		messages.Add($"[State Change] {oldState} => {newState}");
	}

	private void signalRConnection_OnError(Connection connection, string error)
	{
		messages.Add("[Error] " + error);
	}
}
