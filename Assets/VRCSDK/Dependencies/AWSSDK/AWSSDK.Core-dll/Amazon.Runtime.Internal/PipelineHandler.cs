using Amazon.Runtime.Internal.Util;
using System;
using System.Diagnostics;

namespace Amazon.Runtime.Internal
{
	public abstract class PipelineHandler : IPipelineHandler
	{
		public virtual ILogger Logger
		{
			get;
			set;
		}

		public IPipelineHandler InnerHandler
		{
			get;
			set;
		}

		public IPipelineHandler OuterHandler
		{
			get;
			set;
		}

		public virtual void InvokeSync(IExecutionContext executionContext)
		{
			if (InnerHandler != null)
			{
				InnerHandler.InvokeSync(executionContext);
				return;
			}
			throw new InvalidOperationException("Cannot invoke InnerHandler. InnerHandler is not set.");
		}

		[DebuggerHidden]
		public virtual IAsyncResult InvokeAsync(IAsyncExecutionContext executionContext)
		{
			if (InnerHandler != null)
			{
				return InnerHandler.InvokeAsync(executionContext);
			}
			throw new InvalidOperationException("Cannot invoke InnerHandler. InnerHandler is not set.");
		}

		[DebuggerHidden]
		public void AsyncCallback(IAsyncExecutionContext executionContext)
		{
			try
			{
				InvokeAsyncCallback(executionContext);
			}
			catch (Exception ex)
			{
				Logger.Error(ex, "An exception of type {0} was thrown from InvokeAsyncCallback().", ex.GetType().Name);
				executionContext.RequestContext.Metrics.AddProperty(Metric.Exception, ex);
				executionContext.RequestContext.Metrics.StopEvent(Metric.ClientExecuteTime);
				LogMetrics(ExecutionContext.CreateFromAsyncContext(executionContext));
				executionContext.ResponseContext.AsyncResult.HandleException(ex);
			}
		}

		[DebuggerHidden]
		protected virtual void InvokeAsyncCallback(IAsyncExecutionContext executionContext)
		{
			if (OuterHandler != null)
			{
				OuterHandler.AsyncCallback(executionContext);
			}
			else
			{
				executionContext.ResponseContext.AsyncResult.Response = executionContext.ResponseContext.Response;
				executionContext.ResponseContext.AsyncResult.InvokeCallback();
			}
		}

		protected void LogMetrics(IExecutionContext executionContext)
		{
			RequestMetrics metrics = executionContext.RequestContext.Metrics;
			if (executionContext.RequestContext.ClientConfig.LogMetrics)
			{
				string errors = metrics.GetErrors();
				if (!string.IsNullOrEmpty(errors))
				{
					Logger.InfoFormat("Request metrics errors: {0}", errors);
				}
				Logger.InfoFormat("Request metrics: {0}", metrics);
			}
		}
	}
}
