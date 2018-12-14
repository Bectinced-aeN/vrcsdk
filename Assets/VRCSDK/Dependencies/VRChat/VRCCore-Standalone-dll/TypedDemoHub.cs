using UnityEngine;
using VRC.Core.BestHTTP.SignalR.Hubs;
using VRC.Core.BestHTTP.SignalR.Messages;

internal class TypedDemoHub : Hub
{
	private string typedEchoResult = string.Empty;

	private string typedEchoClientResult = string.Empty;

	public TypedDemoHub()
		: base("typeddemohub")
	{
		On("Echo", Echo);
	}

	private void Echo(Hub hub, MethodCallMessage methodCall)
	{
		typedEchoClientResult = $"{methodCall.Arguments[0]} #{methodCall.Arguments[1]} triggered!";
	}

	public void Echo(string msg)
	{
		Call("echo", OnEcho_Done, msg);
	}

	private void OnEcho_Done(Hub hub, ClientMessage originalMessage, ResultMessage result)
	{
		typedEchoResult = "TypedDemoHub.Echo(string message) invoked!";
	}

	public void Draw()
	{
		GUILayout.Label("Typed callback", (GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.Space(20f);
		GUILayout.BeginVertical((GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.Label(typedEchoResult, (GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.Label(typedEchoClientResult, (GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
	}
}
