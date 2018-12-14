namespace Amazon.Runtime
{
	public interface IAsyncExecutionContext
	{
		IAsyncResponseContext ResponseContext
		{
			get;
		}

		IAsyncRequestContext RequestContext
		{
			get;
		}

		object RuntimeState
		{
			get;
			set;
		}
	}
}
