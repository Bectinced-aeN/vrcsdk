namespace Amazon.Runtime
{
	public class AsyncOptions
	{
		public bool ExecuteCallbackOnMainThread
		{
			get;
			set;
		}

		public object State
		{
			get;
			set;
		}

		public AsyncOptions()
		{
			ExecuteCallbackOnMainThread = true;
		}
	}
}
