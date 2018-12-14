using UnityEngine;
using VRC.Core.BestHTTP.Examples;
using VRC.Core.BestHTTP.SignalR.Hubs;
using VRC.Core.BestHTTP.SignalR.Messages;

internal class DemoHub : Hub
{
	private float longRunningJobProgress;

	private string longRunningJobStatus = "Not Started!";

	private string fromArbitraryCodeResult = string.Empty;

	private string groupAddedResult = string.Empty;

	private string dynamicTaskResult = string.Empty;

	private string genericTaskResult = string.Empty;

	private string taskWithExceptionResult = string.Empty;

	private string genericTaskWithExceptionResult = string.Empty;

	private string synchronousExceptionResult = string.Empty;

	private string invokingHubMethodWithDynamicResult = string.Empty;

	private string simpleArrayResult = string.Empty;

	private string complexTypeResult = string.Empty;

	private string complexArrayResult = string.Empty;

	private string voidOverloadResult = string.Empty;

	private string intOverloadResult = string.Empty;

	private string readStateResult = string.Empty;

	private string plainTaskResult = string.Empty;

	private string genericTaskWithContinueWithResult = string.Empty;

	private GUIMessageList invokeResults = new GUIMessageList();

	public DemoHub()
		: base("demo")
	{
		On("invoke", Invoke);
		On("signal", Signal);
		On("groupAdded", GroupAdded);
		On("fromArbitraryCode", FromArbitraryCode);
	}

	public void ReportProgress(string arg)
	{
		Call("reportProgress", OnLongRunningJob_Done, null, OnLongRunningJob_Progress, arg);
	}

	public void OnLongRunningJob_Progress(Hub hub, ClientMessage originialMessage, ProgressMessage progress)
	{
		longRunningJobProgress = (float)progress.Progress;
		longRunningJobStatus = progress.Progress.ToString() + "%";
	}

	public void OnLongRunningJob_Done(Hub hub, ClientMessage originalMessage, ResultMessage result)
	{
		longRunningJobStatus = result.ReturnValue.ToString();
		MultipleCalls();
	}

	public void MultipleCalls()
	{
		Call("multipleCalls");
	}

	public void DynamicTask()
	{
		Call("dynamicTask", OnDynamicTask_Done, OnDynamicTask_Failed);
	}

	private void OnDynamicTask_Failed(Hub hub, ClientMessage originalMessage, FailureMessage result)
	{
		dynamicTaskResult = $"The dynamic task failed :( {result.ErrorMessage}";
	}

	private void OnDynamicTask_Done(Hub hub, ClientMessage originalMessage, ResultMessage result)
	{
		dynamicTaskResult = $"The dynamic task! {result.ReturnValue}";
	}

	public void AddToGroups()
	{
		Call("addToGroups");
	}

	public void GetValue()
	{
		Call("getValue", delegate(Hub hub, ClientMessage msg, ResultMessage result)
		{
			genericTaskResult = $"The value is {result.ReturnValue} after 5 seconds";
		});
	}

	public void TaskWithException()
	{
		Call("taskWithException", null, delegate(Hub hub, ClientMessage msg, FailureMessage error)
		{
			taskWithExceptionResult = $"Error: {error.ErrorMessage}";
		});
	}

	public void GenericTaskWithException()
	{
		Call("genericTaskWithException", null, delegate(Hub hub, ClientMessage msg, FailureMessage error)
		{
			genericTaskWithExceptionResult = $"Error: {error.ErrorMessage}";
		});
	}

	public void SynchronousException()
	{
		Call("synchronousException", null, delegate(Hub hub, ClientMessage msg, FailureMessage error)
		{
			synchronousExceptionResult = $"Error: {error.ErrorMessage}";
		});
	}

	public void PassingDynamicComplex(object person)
	{
		Call("passingDynamicComplex", delegate(Hub hub, ClientMessage msg, ResultMessage result)
		{
			invokingHubMethodWithDynamicResult = $"The person's age is {result.ReturnValue}";
		}, person);
	}

	public void SimpleArray(int[] array)
	{
		Call("simpleArray", delegate
		{
			simpleArrayResult = "Simple array works!";
		}, array);
	}

	public void ComplexType(object person)
	{
		Call("complexType", delegate
		{
			complexTypeResult = string.Format("Complex Type -> {0}", ((IHub)this).Connection.JsonEncoder.Encode(base.State["person"]));
		}, person);
	}

	public void ComplexArray(object[] complexArray)
	{
		Call("ComplexArray", delegate
		{
			complexArrayResult = "Complex Array Works!";
		}, complexArray);
	}

	public void Overload()
	{
		Call("Overload", OnVoidOverload_Done);
	}

	private void OnVoidOverload_Done(Hub hub, ClientMessage originalMessage, ResultMessage result)
	{
		voidOverloadResult = "Void Overload called";
		Overload(101);
	}

	public void Overload(int number)
	{
		Call("Overload", OnIntOverload_Done, number);
	}

	private void OnIntOverload_Done(Hub hub, ClientMessage originalMessage, ResultMessage result)
	{
		intOverloadResult = $"Overload with return value called => {result.ReturnValue.ToString()}";
	}

	public void ReadStateValue()
	{
		Call("readStateValue", delegate(Hub hub, ClientMessage msg, ResultMessage result)
		{
			readStateResult = $"Read some state! => {result.ReturnValue}";
		});
	}

	public void PlainTask()
	{
		Call("plainTask", delegate
		{
			plainTaskResult = "Plain Task Result";
		});
	}

	public void GenericTaskWithContinueWith()
	{
		Call("genericTaskWithContinueWith", delegate(Hub hub, ClientMessage msg, ResultMessage result)
		{
			genericTaskWithContinueWithResult = result.ReturnValue.ToString();
		});
	}

	private void FromArbitraryCode(Hub hub, MethodCallMessage methodCall)
	{
		fromArbitraryCodeResult = (methodCall.Arguments[0] as string);
	}

	private void GroupAdded(Hub hub, MethodCallMessage methodCall)
	{
		if (!string.IsNullOrEmpty(groupAddedResult))
		{
			groupAddedResult = "Group Already Added!";
		}
		else
		{
			groupAddedResult = "Group Added!";
		}
	}

	private void Signal(Hub hub, MethodCallMessage methodCall)
	{
		dynamicTaskResult = $"The dynamic task! {methodCall.Arguments[0]}";
	}

	private void Invoke(Hub hub, MethodCallMessage methodCall)
	{
		invokeResults.Add(string.Format("{0} client state index -> {1}", methodCall.Arguments[0], base.State["index"]));
	}

	public void Draw()
	{
		GUILayout.Label("Arbitrary Code", (GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.Space(20f);
		GUILayout.Label($"Sending {fromArbitraryCodeResult} from arbitrary code without the hub itself!", (GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		GUILayout.Label("Group Added", (GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.Space(20f);
		GUILayout.Label(groupAddedResult, (GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		GUILayout.Label("Dynamic Task", (GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.Space(20f);
		GUILayout.Label(dynamicTaskResult, (GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		GUILayout.Label("Report Progress", (GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.Space(20f);
		GUILayout.BeginVertical((GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.Label(longRunningJobStatus, (GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.HorizontalSlider(longRunningJobProgress, 0f, 100f, (GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		GUILayout.Label("Generic Task", (GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.Space(20f);
		GUILayout.Label(genericTaskResult, (GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		GUILayout.Label("Task With Exception", (GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.Space(20f);
		GUILayout.Label(taskWithExceptionResult, (GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		GUILayout.Label("Generic Task With Exception", (GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.Space(20f);
		GUILayout.Label(genericTaskWithExceptionResult, (GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		GUILayout.Label("Synchronous Exception", (GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.Space(20f);
		GUILayout.Label(synchronousExceptionResult, (GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		GUILayout.Label("Invoking hub method with dynamic", (GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.Space(20f);
		GUILayout.Label(invokingHubMethodWithDynamicResult, (GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		GUILayout.Label("Simple Array", (GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.Space(20f);
		GUILayout.Label(simpleArrayResult, (GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		GUILayout.Label("Complex Type", (GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.Space(20f);
		GUILayout.Label(complexTypeResult, (GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		GUILayout.Label("Complex Array", (GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.Space(20f);
		GUILayout.Label(complexArrayResult, (GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		GUILayout.Label("Overloads", (GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.Space(20f);
		GUILayout.BeginVertical((GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.Label(voidOverloadResult, (GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.Label(intOverloadResult, (GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		GUILayout.Label("Read State Value", (GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.Space(20f);
		GUILayout.Label(readStateResult, (GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		GUILayout.Label("Plain Task", (GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.Space(20f);
		GUILayout.Label(plainTaskResult, (GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		GUILayout.Label("Generic Task With ContinueWith", (GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.Space(20f);
		GUILayout.Label(genericTaskWithContinueWithResult, (GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		GUILayout.Label("Message Pump", (GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
		GUILayout.Space(20f);
		invokeResults.Draw((float)(Screen.get_width() - 40), 270f);
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
	}
}
