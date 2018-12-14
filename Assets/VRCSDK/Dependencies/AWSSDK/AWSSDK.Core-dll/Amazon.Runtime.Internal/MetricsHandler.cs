using System;

namespace Amazon.Runtime.Internal
{
	public class MetricsHandler : PipelineHandler
	{
		public override void InvokeSync(IExecutionContext executionContext)
		{
			executionContext.RequestContext.Metrics.AddProperty(Metric.AsyncCall, false);
			try
			{
				executionContext.RequestContext.Metrics.StartEvent(Metric.ClientExecuteTime);
				base.InvokeSync(executionContext);
			}
			finally
			{
				executionContext.RequestContext.Metrics.StopEvent(Metric.ClientExecuteTime);
				LogMetrics(executionContext);
			}
		}

		public override IAsyncResult InvokeAsync(IAsyncExecutionContext executionContext)
		{
			executionContext.RequestContext.Metrics.AddProperty(Metric.AsyncCall, true);
			executionContext.RequestContext.Metrics.StartEvent(Metric.ClientExecuteTime);
			return base.InvokeAsync(executionContext);
		}

		protected override void InvokeAsyncCallback(IAsyncExecutionContext executionContext)
		{
			executionContext.RequestContext.Metrics.StopEvent(Metric.ClientExecuteTime);
			LogMetrics(ExecutionContext.CreateFromAsyncContext(executionContext));
			base.InvokeAsyncCallback(executionContext);
		}
	}
}
