using System;

namespace Amazon.Runtime.Internal.Util
{
	internal abstract class InternalLogger
	{
		public Type DeclaringType
		{
			get;
			private set;
		}

		public bool IsEnabled
		{
			get;
			set;
		}

		public virtual bool IsErrorEnabled => true;

		public virtual bool IsDebugEnabled => true;

		public virtual bool IsInfoEnabled => true;

		public InternalLogger(Type declaringType)
		{
			DeclaringType = declaringType;
			IsEnabled = true;
		}

		public abstract void Flush();

		public abstract void Error(Exception exception, string messageFormat, params object[] args);

		public abstract void Debug(Exception exception, string messageFormat, params object[] args);

		public abstract void DebugFormat(string message, params object[] arguments);

		public abstract void InfoFormat(string message, params object[] arguments);
	}
}
