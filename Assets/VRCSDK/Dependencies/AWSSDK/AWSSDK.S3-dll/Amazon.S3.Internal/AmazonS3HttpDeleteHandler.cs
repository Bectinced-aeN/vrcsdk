using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Util.Internal;
using System;
using System.Collections.Generic;

namespace Amazon.S3.Internal
{
	public class AmazonS3HttpDeleteHandler : PipelineHandler
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
			executionContext.get_RequestContext().get_OriginalRequest();
			ModifyDeleteRequest(executionContext);
		}

		internal static void ModifyDeleteRequest(IExecutionContext executionContext)
		{
			IRequest request = executionContext.get_RequestContext().get_Request();
			IDictionary<string, string> headers = request.get_Headers();
			IDictionary<string, string> parameters = request.get_Parameters();
			if (InternalSDKUtils.get_IsAndroid() && request.get_HttpMethod() == "DELETE" && !parameters.ContainsKey("ContentType") && !headers.ContainsKey("Content-Type"))
			{
				headers.Add("Content-Type", "application/x-www-form-urlencoded");
			}
		}

		public AmazonS3HttpDeleteHandler()
			: this()
		{
		}
	}
}
