namespace Amazon.Runtime.Internal
{
	public class AsyncExecutionContext : IAsyncExecutionContext
	{
		public IAsyncResponseContext ResponseContext
		{
			get;
			private set;
		}

		public IAsyncRequestContext RequestContext
		{
			get;
			private set;
		}

		public object RuntimeState
		{
			get;
			set;
		}

		public AsyncExecutionContext(bool enableMetrics)
		{
			RequestContext = new AsyncRequestContext(enableMetrics);
			ResponseContext = new AsyncResponseContext();
		}

		public AsyncExecutionContext(IAsyncRequestContext requestContext, IAsyncResponseContext responseContext)
		{
			RequestContext = requestContext;
			ResponseContext = responseContext;
		}
	}
}
