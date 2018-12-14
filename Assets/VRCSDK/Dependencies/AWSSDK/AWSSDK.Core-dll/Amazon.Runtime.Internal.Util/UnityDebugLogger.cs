using System;
using UnityEngine;

namespace Amazon.Runtime.Internal.Util
{
	internal class UnityDebugLogger : InternalLogger
	{
		public override bool IsDebugEnabled => true;

		public override bool IsErrorEnabled => true;

		public override bool IsInfoEnabled => true;

		public UnityDebugLogger(Type declaringType)
			: base(declaringType)
		{
		}

		public override void Flush()
		{
		}

		public override void Error(Exception exception, string messageFormat, params object[] args)
		{
			if (exception != null)
			{
				Debug.LogException(exception);
			}
			if (!string.IsNullOrEmpty(messageFormat))
			{
				Debug.LogError((object)string.Format(messageFormat, args));
			}
		}

		public override void Debug(Exception exception, string messageFormat, params object[] args)
		{
			if (exception != null)
			{
				Debug.LogException(exception);
			}
			if (!string.IsNullOrEmpty(messageFormat))
			{
				Debug.Log((object)string.Format(messageFormat, args));
			}
		}

		public override void DebugFormat(string messageFormat, params object[] args)
		{
			if (!string.IsNullOrEmpty(messageFormat))
			{
				Debug.Log((object)string.Format(messageFormat, args));
			}
		}

		public override void InfoFormat(string message, params object[] arguments)
		{
			if (!string.IsNullOrEmpty(message))
			{
				Debug.Log((object)string.Format(message, arguments));
			}
		}
	}
}
