using System;

namespace Amazon.Runtime.Internal
{
	public class ErrorCallbackHandler : PipelineHandler
	{
		public Action<IExecutionContext, Exception> OnError
		{
			get;
			set;
		}

		public override void InvokeSync(IExecutionContext executionContext)
		{
			try
			{
				base.InvokeSync(executionContext);
			}
			catch (Exception exception)
			{
				HandleException(executionContext, exception);
				throw;
			}
		}

		protected override void InvokeAsyncCallback(IAsyncExecutionContext executionContext)
		{
			Exception exception = executionContext.ResponseContext.AsyncResult.Exception;
			if (executionContext.ResponseContext.AsyncResult.Exception != null)
			{
				HandleException(ExecutionContext.CreateFromAsyncContext(executionContext), exception);
			}
			base.InvokeAsyncCallback(executionContext);
		}

		protected void HandleException(IExecutionContext executionContext, Exception exception)
		{
			OnError(executionContext, exception);
		}
	}
}
