using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Util;
using Amazon.S3.Model;
using System;

namespace Amazon.S3.Internal
{
	public class AmazonS3ExceptionHandler : PipelineHandler
	{
		public override void InvokeSync(IExecutionContext executionContext)
		{
			try
			{
				this.InvokeSync(executionContext);
			}
			catch (Exception exception)
			{
				HandleException(executionContext, exception);
				throw;
			}
		}

		protected override void InvokeAsyncCallback(IAsyncExecutionContext executionContext)
		{
			Exception exception = executionContext.get_ResponseContext().get_AsyncResult().get_Exception();
			if (executionContext.get_ResponseContext().get_AsyncResult().get_Exception() != null)
			{
				HandleException(ExecutionContext.CreateFromAsyncContext(executionContext), exception);
			}
			this.InvokeAsyncCallback(executionContext);
		}

		protected virtual void HandleException(IExecutionContext executionContext, Exception exception)
		{
			PutObjectRequest putObjectRequest = executionContext.get_RequestContext().get_OriginalRequest() as PutObjectRequest;
			if (putObjectRequest != null)
			{
				HashStream val = putObjectRequest.InputStream as HashStream;
				if (val != null)
				{
					putObjectRequest.InputStream = val.GetNonWrapperBaseStream();
				}
			}
			UploadPartRequest uploadPartRequest = executionContext.get_RequestContext().get_OriginalRequest() as UploadPartRequest;
			if (uploadPartRequest != null)
			{
				HashStream val2 = uploadPartRequest.InputStream as HashStream;
				if (val2 != null)
				{
					uploadPartRequest.InputStream = val2.GetNonWrapperBaseStream();
				}
			}
			if (executionContext.get_RequestContext().get_Request() != null)
			{
				AmazonS3Client.CleanupRequest(executionContext.get_RequestContext().get_Request());
			}
		}

		public AmazonS3ExceptionHandler()
			: this()
		{
		}
	}
}
