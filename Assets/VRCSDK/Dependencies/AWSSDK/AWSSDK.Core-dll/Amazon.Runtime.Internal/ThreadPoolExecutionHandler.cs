using Amazon.Runtime.Internal.Util;
using System;

namespace Amazon.Runtime.Internal
{
	public class ThreadPoolExecutionHandler : PipelineHandler
	{
		private static ThreadPoolThrottler<IAsyncExecutionContext> _throttler;

		private static object _lock = new object();

		public ThreadPoolExecutionHandler(int concurrentRequests)
		{
			lock (_lock)
			{
				if (_throttler == null)
				{
					_throttler = new ThreadPoolThrottler<IAsyncExecutionContext>(concurrentRequests);
				}
			}
		}

		public override IAsyncResult InvokeAsync(IAsyncExecutionContext executionContext)
		{
			if (UnityInitializer.IsMainThread())
			{
				_throttler.Enqueue(executionContext, InvokeAsyncHelper, ErrorCallback);
				return null;
			}
			return base.InvokeAsync(executionContext);
		}

		private void InvokeAsyncHelper(IAsyncExecutionContext executionContext)
		{
			base.InvokeAsync(executionContext);
		}

		private void ErrorCallback(Exception exception, IAsyncExecutionContext executionContext)
		{
			Logger.Error(exception, "An exception of type {0} was thrown from InvokeAsyncCallback().", exception.GetType().Name);
			executionContext.RequestContext.Metrics.AddProperty(Metric.Exception, exception);
			executionContext.RequestContext.Metrics.StopEvent(Metric.ClientExecuteTime);
			LogMetrics(ExecutionContext.CreateFromAsyncContext(executionContext));
			executionContext.ResponseContext.AsyncResult = new RuntimeAsyncResult(executionContext.RequestContext.Callback, executionContext.RequestContext.State);
			executionContext.ResponseContext.AsyncResult.Exception = exception;
			executionContext.ResponseContext.AsyncResult.AsyncOptions = executionContext.RequestContext.AsyncOptions;
			executionContext.ResponseContext.AsyncResult.Action = executionContext.RequestContext.Action;
			executionContext.ResponseContext.AsyncResult.Request = executionContext.RequestContext.OriginalRequest;
			executionContext.ResponseContext.AsyncResult.InvokeCallback();
		}
	}
}
