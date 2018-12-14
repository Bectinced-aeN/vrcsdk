using System;
using UnityEngine;
using VRC.Core.BestHTTP.Examples;
using VRC.Core.BestHTTP.SignalR;
using VRC.Core.BestHTTP.SignalR.Authentication;

internal class AuthenticationSample : MonoBehaviour
{
	private readonly Uri URI = new Uri("https://besthttpsignalr.azurewebsites.net/signalr");

	private Connection signalRConnection;

	private string userName = string.Empty;

	private string role = string.Empty;

	private Vector2 scrollPos;

	public AuthenticationSample()
		: this()
	{
	}

	private void Start()
	{
		signalRConnection = new Connection(URI, new BaseHub("noauthhub", "Messages"), new BaseHub("invokeauthhub", "Messages Invoked By Admin or Invoker"), new BaseHub("authhub", "Messages Requiring Authentication to Send or Receive"), new BaseHub("inheritauthhub", "Messages Requiring Authentication to Send or Receive Because of Inheritance"), new BaseHub("incomingauthhub", "Messages Requiring Authentication to Send"), new BaseHub("adminauthhub", "Messages Requiring Admin Membership to Send or Receive"), new BaseHub("userandroleauthhub", "Messages Requiring Name to be \"User\" and Role to be \"Admin\" to Send or Receive"));
		if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(role))
		{
			signalRConnection.AuthenticationProvider = new HeaderAuthenticator(userName, role);
		}
		signalRConnection.OnConnected += signalRConnection_OnConnected;
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
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			scrollPos = GUILayout.BeginScrollView(scrollPos, false, false, (GUILayoutOption[])new GUILayoutOption[0]);
			GUILayout.BeginVertical((GUILayoutOption[])new GUILayoutOption[0]);
			if (signalRConnection.AuthenticationProvider == null)
			{
				GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
				GUILayout.Label("Username (Enter 'User'):", (GUILayoutOption[])new GUILayoutOption[0]);
				userName = GUILayout.TextField(userName, (GUILayoutOption[])new GUILayoutOption[1]
				{
					GUILayout.MinWidth(100f)
				});
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
				GUILayout.Label("Roles (Enter 'Invoker' or 'Admin'):", (GUILayoutOption[])new GUILayoutOption[0]);
				role = GUILayout.TextField(role, (GUILayoutOption[])new GUILayoutOption[1]
				{
					GUILayout.MinWidth(100f)
				});
				GUILayout.EndHorizontal();
				if (GUILayout.Button("Log in", (GUILayoutOption[])new GUILayoutOption[0]))
				{
					Restart();
				}
			}
			for (int i = 0; i < signalRConnection.Hubs.Length; i++)
			{
				(signalRConnection.Hubs[i] as BaseHub).Draw();
			}
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
		});
	}

	private void signalRConnection_OnConnected(Connection manager)
	{
		for (int i = 0; i < signalRConnection.Hubs.Length; i++)
		{
			(signalRConnection.Hubs[i] as BaseHub).InvokedFromClient();
		}
	}

	private void Restart()
	{
		signalRConnection.OnConnected -= signalRConnection_OnConnected;
		signalRConnection.Close();
		signalRConnection = null;
		Start();
	}
}
