using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.S3.Model;
using Amazon.S3.Util;
using System;

namespace Amazon.S3.Internal
{
	public class AmazonS3KmsHandler : PipelineHandler
	{
		public override void InvokeSync(IExecutionContext executionContext)
		{
			PreInvoke(executionContext);
			this.InvokeSync(executionContext);
		}

		public override IAsyncResult InvokeAsync(IAsyncExecutionContext executionContext)
		{
			PreInvoke(ExecutionContext.CreateFromAsyncContext(executionContext));
			return this.InvokeAsync(executionContext);
		}

		protected virtual void PreInvoke(IExecutionContext executionContext)
		{
			EvaluateIfSigV4Required(executionContext.get_RequestContext().get_Request());
		}

		internal static void EvaluateIfSigV4Required(IRequest request)
		{
			if (request.get_OriginalRequest() is GetObjectRequest && AmazonS3Uri.IsAmazonS3Endpoint(request.get_Endpoint()) && new AmazonS3Uri(request.get_Endpoint().OriginalString).Region != RegionEndpoint.USEast1)
			{
				request.set_UseSigV4(true);
			}
		}

		public AmazonS3KmsHandler()
			: this()
		{
		}
	}
}
