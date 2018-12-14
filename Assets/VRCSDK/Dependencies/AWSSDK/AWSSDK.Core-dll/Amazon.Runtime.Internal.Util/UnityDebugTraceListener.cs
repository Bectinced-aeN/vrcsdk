using System.Diagnostics;
using UnityEngine;

namespace Amazon.Runtime.Internal.Util
{
	public class UnityDebugTraceListener : TraceListener
	{
		public override bool IsThreadSafe => true;

		public UnityDebugTraceListener()
		{
		}

		public UnityDebugTraceListener(string name)
			: base(name)
		{
		}

		public override void Write(string message)
		{
			Debug.Log((object)message);
		}

		public override void WriteLine(string message)
		{
			Debug.Log((object)message);
		}

		public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
		{
			LogMessage(string.Format(format, args), eventType);
		}

		public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
		{
			LogMessage(data.ToString(), eventType);
		}

		public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
		{
			LogMessage(message, eventType);
		}

		public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
		{
			foreach (object obj in data)
			{
				if (obj != null)
				{
					LogMessage(obj.ToString(), eventType);
				}
			}
		}

		public override void Fail(string message)
		{
			Debug.LogError((object)message);
		}

		public override void Fail(string message, string detailMessage)
		{
			Debug.LogError((object)(message + " " + detailMessage));
		}

		public override void Write(object o)
		{
			Debug.Log((object)o.ToString());
		}

		public override void WriteLine(object o)
		{
			Debug.Log((object)o.ToString());
		}

		public override void WriteLine(object o, string category)
		{
			Debug.Log((object)o.ToString());
		}

		private void LogMessage(string message, TraceEventType eventType)
		{
			if (eventType.Equals(TraceEventType.Critical) || eventType.Equals(TraceEventType.Error))
			{
				Debug.LogError((object)message);
			}
			else if (eventType.Equals(TraceEventType.Warning))
			{
				Debug.LogWarning((object)message);
			}
			else
			{
				Debug.Log((object)message);
			}
		}
	}
}
