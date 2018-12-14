using System;

namespace Amazon.Runtime.Internal
{
	public class CredentialsRetriever : PipelineHandler
	{
		protected AWSCredentials Credentials
		{
			get;
			private set;
		}

		public CredentialsRetriever(AWSCredentials credentials)
		{
			Credentials = credentials;
		}

		protected virtual void PreInvoke(IExecutionContext executionContext)
		{
			ImmutableCredentials immutableCredentials = null;
			if (Credentials != null && !(Credentials is AnonymousAWSCredentials))
			{
				using (executionContext.RequestContext.Metrics.StartEvent(Metric.CredentialsRequestTime))
				{
					immutableCredentials = Credentials.GetCredentials();
				}
			}
			executionContext.RequestContext.ImmutableCredentials = immutableCredentials;
		}

		public override void InvokeSync(IExecutionContext executionContext)
		{
			PreInvoke(executionContext);
			base.InvokeSync(executionContext);
		}

		public override IAsyncResult InvokeAsync(IAsyncExecutionContext executionContext)
		{
			PreInvoke(ExecutionContext.CreateFromAsyncContext(executionContext));
			return base.InvokeAsync(executionContext);
		}
	}
}
