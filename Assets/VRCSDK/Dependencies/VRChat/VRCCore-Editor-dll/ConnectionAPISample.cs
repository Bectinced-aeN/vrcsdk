using System;
using UnityEngine;
using VRC.Core.BestHTTP.Cookies;
using VRC.Core.BestHTTP.Examples;
using VRC.Core.BestHTTP.JSON;
using VRC.Core.BestHTTP.SignalR;
using VRC.Core.BestHTTP.SignalR.JsonEncoders;

internal sealed class ConnectionAPISample : MonoBehaviour
{
	private enum MessageTypes
	{
		Send,
		Broadcast,
		Join,
		PrivateMessage,
		AddToGroup,
		RemoveFromGroup,
		SendToGroup,
		BroadcastExceptMe
	}

	private readonly Uri URI = new Uri("https://besthttpsignalr.azurewebsites.net/raw-connection/");

	private Connection signalRConnection;

	private string ToEveryBodyText = string.Empty;

	private string ToMeText = string.Empty;

	private string PrivateMessageText = string.Empty;

	private string PrivateMessageUserOrGroupName = string.Empty;

	private GUIMessageList messages = new GUIMessageList();

	public ConnectionAPISample()
		: this()
	{
	}

	private void Start()
	{
		if (PlayerPrefs.HasKey("userName"))
		{
			CookieJar.Set(URI, new Cookie("user", PlayerPrefs.GetString("userName")));
		}
		signalRConnection = new Connection(URI);
		signalRConnection.JsonEncoder = new LitJsonEncoder();
		signalRConnection.OnStateChanged += signalRConnection_OnStateChanged;
		signalRConnection.OnNonHubMessage += signalRConnection_OnGeneralMessage;
		signalRConnection.Open();
	}

	private void OnGUI()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		GUIHelper.DrawArea(GUIHelper.ClientArea, drawHeader: true, delegate
		{
			GUILayout.BeginVertical((GUILayoutOption[])new GUILayoutOption[0]);
			GUILayout.Label("To Everybody", (GUILayoutOption[])new GUILayoutOption[0]);
			GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
			ToEveryBodyText = GUILayout.TextField(ToEveryBodyText, (GUILayoutOption[])new GUILayoutOption[1]
			{
				GUILayout.MinWidth(100f)
			});
			if (GUILayout.Button("Broadcast", (GUILayoutOption[])new GUILayoutOption[0]))
			{
				Broadcast(ToEveryBodyText);
			}
			if (GUILayout.Button("Broadcast (All Except Me)", (GUILayoutOption[])new GUILayoutOption[0]))
			{
				BroadcastExceptMe(ToEveryBodyText);
			}
			if (GUILayout.Button("Enter Name", (GUILayoutOption[])new GUILayoutOption[0]))
			{
				EnterName(ToEveryBodyText);
			}
			if (GUILayout.Button("Join Group", (GUILayoutOption[])new GUILayoutOption[0]))
			{
				JoinGroup(ToEveryBodyText);
			}
			if (GUILayout.Button("Leave Group", (GUILayoutOption[])new GUILayoutOption[0]))
			{
				LeaveGroup(ToEveryBodyText);
			}
			GUILayout.EndHorizontal();
			GUILayout.Label("To Me", (GUILayoutOption[])new GUILayoutOption[0]);
			GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
			ToMeText = GUILayout.TextField(ToMeText, (GUILayoutOption[])new GUILayoutOption[1]
			{
				GUILayout.MinWidth(100f)
			});
			if (GUILayout.Button("Send to me", (GUILayoutOption[])new GUILayoutOption[0]))
			{
				SendToMe(ToMeText);
			}
			GUILayout.EndHorizontal();
			GUILayout.Label("Private Message", (GUILayoutOption[])new GUILayoutOption[0]);
			GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
			GUILayout.Label("Message:", (GUILayoutOption[])new GUILayoutOption[0]);
			PrivateMessageText = GUILayout.TextField(PrivateMessageText, (GUILayoutOption[])new GUILayoutOption[1]
			{
				GUILayout.MinWidth(100f)
			});
			GUILayout.Label("User or Group name:", (GUILayoutOption[])new GUILayoutOption[0]);
			PrivateMessageUserOrGroupName = GUILayout.TextField(PrivateMessageUserOrGroupName, (GUILayoutOption[])new GUILayoutOption[1]
			{
				GUILayout.MinWidth(100f)
			});
			if (GUILayout.Button("Send to user", (GUILayoutOption[])new GUILayoutOption[0]))
			{
				SendToUser(PrivateMessageUserOrGroupName, PrivateMessageText);
			}
			if (GUILayout.Button("Send to group", (GUILayoutOption[])new GUILayoutOption[0]))
			{
				SendToGroup(PrivateMessageUserOrGroupName, PrivateMessageText);
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(20f);
			if (signalRConnection.State == ConnectionStates.Closed)
			{
				if (GUILayout.Button("Start Connection", (GUILayoutOption[])new GUILayoutOption[0]))
				{
					signalRConnection.Open();
				}
			}
			else if (GUILayout.Button("Stop Connection", (GUILayoutOption[])new GUILayoutOption[0]))
			{
				signalRConnection.Close();
			}
			GUILayout.Space(20f);
			GUILayout.Label("Messages", (GUILayoutOption[])new GUILayoutOption[0]);
			GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
			GUILayout.Space(20f);
			messages.Draw((float)(Screen.get_width() - 20), 0f);
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
		});
	}

	private void OnDestroy()
	{
		signalRConnection.Close();
	}

	private void signalRConnection_OnGeneralMessage(Connection manager, object data)
	{
		string str = Json.Encode(data);
		messages.Add("[Server Message] " + str);
	}

	private void signalRConnection_OnStateChanged(Connection manager, ConnectionStates oldState, ConnectionStates newState)
	{
		messages.Add($"[State Change] {oldState.ToString()} => {newState.ToString()}");
	}

	private void Broadcast(string text)
	{
		signalRConnection.Send(new
		{
			Type = MessageTypes.Broadcast,
			Value = text
		});
	}

	private void BroadcastExceptMe(string text)
	{
		signalRConnection.Send(new
		{
			Type = MessageTypes.BroadcastExceptMe,
			Value = text
		});
	}

	private void EnterName(string name)
	{
		signalRConnection.Send(new
		{
			Type = MessageTypes.Join,
			Value = name
		});
	}

	private void JoinGroup(string groupName)
	{
		signalRConnection.Send(new
		{
			Type = MessageTypes.AddToGroup,
			Value = groupName
		});
	}

	private void LeaveGroup(string groupName)
	{
		signalRConnection.Send(new
		{
			Type = MessageTypes.RemoveFromGroup,
			Value = groupName
		});
	}

	private void SendToMe(string text)
	{
		signalRConnection.Send(new
		{
			Type = MessageTypes.Send,
			Value = text
		});
	}

	private void SendToUser(string userOrGroupName, string text)
	{
		signalRConnection.Send(new
		{
			Type = MessageTypes.PrivateMessage,
			Value = $"{userOrGroupName}|{text}"
		});
	}

	private void SendToGroup(string userOrGroupName, string text)
	{
		signalRConnection.Send(new
		{
			Type = MessageTypes.SendToGroup,
			Value = $"{userOrGroupName}|{text}"
		});
	}
}
