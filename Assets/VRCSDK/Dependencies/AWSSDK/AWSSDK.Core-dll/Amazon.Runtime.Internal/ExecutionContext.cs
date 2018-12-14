namespace Amazon.Runtime.Internal
{
	public class ExecutionContext : IExecutionContext
	{
		public IRequestContext RequestContext
		{
			get;
			private set;
		}

		public IResponseContext ResponseContext
		{
			get;
			private set;
		}

		public ExecutionContext(bool enableMetrics)
		{
			RequestContext = new RequestContext(enableMetrics);
			ResponseContext = new ResponseContext();
		}

		public ExecutionContext(IRequestContext requestContext, IResponseContext responseContext)
		{
			RequestContext = requestContext;
			ResponseContext = responseContext;
		}

		public static IExecutionContext CreateFromAsyncContext(IAsyncExecutionContext asyncContext)
		{
			return new ExecutionContext(asyncContext.RequestContext, asyncContext.ResponseContext);
		}
	}
}
