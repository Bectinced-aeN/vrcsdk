using System;
using UnityEngine;
using VRC.Core.BestHTTP.Examples;
using VRC.Core.BestHTTP.SignalR;
using VRC.Core.BestHTTP.SignalR.Hubs;
using VRC.Core.BestHTTP.SignalR.JsonEncoders;
using VRC.Core.BestHTTP.SignalR.Messages;

internal class DemoHubSample : MonoBehaviour
{
	private readonly Uri URI = new Uri("https://besthttpsignalr.azurewebsites.net/signalr");

	private Connection signalRConnection;

	private DemoHub demoHub;

	private TypedDemoHub typedDemoHub;

	private Hub vbDemoHub;

	private string vbReadStateResult = string.Empty;

	private Vector2 scrollPos;

	public DemoHubSample()
		: this()
	{
	}

	private void Start()
	{
		demoHub = new DemoHub();
		typedDemoHub = new TypedDemoHub();
		vbDemoHub = new Hub("vbdemo");
		signalRConnection = new Connection(URI, demoHub, typedDemoHub, vbDemoHub);
		signalRConnection.JsonEncoder = new LitJsonEncoder();
		signalRConnection.OnConnected += delegate
		{
			var anon = new
			{
				Name = "Foo",
				Age = 20,
				Address = new
				{
					Street = "One Microsoft Way",
					Zip = "98052"
				}
			};
			demoHub.AddToGroups();
			demoHub.GetValue();
			demoHub.TaskWithException();
			demoHub.GenericTaskWithException();
			demoHub.SynchronousException();
			demoHub.DynamicTask();
			demoHub.PassingDynamicComplex(anon);
			demoHub.SimpleArray(new int[3]
			{
				5,
				5,
				6
			});
			demoHub.ComplexType(anon);
			demoHub.ComplexArray(new object[3]
			{
				anon,
				anon,
				anon
			});
			demoHub.ReportProgress("Long running job!");
			demoHub.Overload();
			demoHub.State["name"] = "Testing state!";
			demoHub.ReadStateValue();
			demoHub.PlainTask();
			demoHub.GenericTaskWithContinueWith();
			typedDemoHub.Echo("Typed echo callback");
			vbDemoHub.Call("readStateValue", delegate(Hub hub, ClientMessage msg, ResultMessage result)
			{
				vbReadStateResult = string.Format("Read some state from VB.NET! => {0}", (result.ReturnValue != null) ? result.ReturnValue.ToString() : "undefined");
			});
		};
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
			demoHub.Draw();
			typedDemoHub.Draw();
			GUILayout.Label("Read State Value", (GUILayoutOption[])new GUILayoutOption[0]);
			GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
			GUILayout.Space(20f);
			GUILayout.Label(vbReadStateResult, (GUILayoutOption[])new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
			GUILayout.Space(10f);
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
		});
	}
}
