using Amazon.Runtime.Internal.Transform;
using System;

namespace Amazon.Runtime.Internal
{
	public class Unmarshaller : PipelineHandler
	{
		private bool _supportsResponseLogging;

		public Unmarshaller(bool supportsResponseLogging)
		{
			_supportsResponseLogging = supportsResponseLogging;
		}

		public override void InvokeSync(IExecutionContext executionContext)
		{
			base.InvokeSync(executionContext);
			if (executionContext.ResponseContext.HttpResponse.IsSuccessStatusCode)
			{
				Unmarshall(executionContext);
			}
		}

		protected override void InvokeAsyncCallback(IAsyncExecutionContext executionContext)
		{
			if (executionContext.ResponseContext.AsyncResult.Exception == null)
			{
				Unmarshall(ExecutionContext.CreateFromAsyncContext(executionContext));
			}
			base.InvokeAsyncCallback(executionContext);
		}

		private void Unmarshall(IExecutionContext executionContext)
		{
			IRequestContext requestContext = executionContext.RequestContext;
			IResponseContext responseContext = executionContext.ResponseContext;
			using (requestContext.Metrics.StartEvent(Metric.ResponseProcessingTime))
			{
				ResponseUnmarshaller unmarshaller = requestContext.Unmarshaller;
				try
				{
					bool supportsResponseLogging = _supportsResponseLogging;
					UnmarshallerContext unmarshallerContext = unmarshaller.CreateContext(responseContext.HttpResponse, supportsResponseLogging, responseContext.HttpResponse.ResponseBody.OpenResponse(), requestContext.Metrics);
					try
					{
						AmazonWebServiceResponse amazonWebServiceResponse2 = responseContext.Response = UnmarshallResponse(unmarshallerContext, requestContext);
					}
					catch (Exception ex)
					{
						if (ex is AmazonServiceException || ex is AmazonClientException)
						{
							throw;
						}
						string headerValue = responseContext.HttpResponse.GetHeaderValue("x-amzn-RequestId");
						string responseBody = unmarshallerContext.ResponseBody;
						throw new AmazonUnmarshallingException(headerValue, null, responseBody, ex);
					}
				}
				finally
				{
					if (!unmarshaller.HasStreamingProperty)
					{
						responseContext.HttpResponse.ResponseBody.Dispose();
					}
				}
			}
		}

		private AmazonWebServiceResponse UnmarshallResponse(UnmarshallerContext context, IRequestContext requestContext)
		{
			try
			{
				ResponseUnmarshaller unmarshaller = requestContext.Unmarshaller;
				AmazonWebServiceResponse amazonWebServiceResponse = null;
				using (requestContext.Metrics.StartEvent(Metric.ResponseUnmarshallTime))
				{
					amazonWebServiceResponse = unmarshaller.UnmarshallResponse(context);
				}
				requestContext.Metrics.AddProperty(Metric.StatusCode, amazonWebServiceResponse.HttpStatusCode);
				requestContext.Metrics.AddProperty(Metric.BytesProcessed, amazonWebServiceResponse.ContentLength);
				if (amazonWebServiceResponse.ResponseMetadata != null)
				{
					requestContext.Metrics.AddProperty(Metric.AWSRequestID, amazonWebServiceResponse.ResponseMetadata.RequestId);
				}
				context.ValidateCRC32IfAvailable();
				return amazonWebServiceResponse;
			}
			finally
			{
				if (ShouldLogResponseBody(_supportsResponseLogging, requestContext))
				{
					Logger.DebugFormat("Received response (truncated to {0} bytes): [{1}]", AWSConfigs.LoggingConfig.LogResponsesSizeLimit, context.ResponseBody);
				}
			}
		}

		private static bool ShouldLogResponseBody(bool supportsResponseLogging, IRequestContext requestContext)
		{
			if (supportsResponseLogging)
			{
				if (!requestContext.ClientConfig.LogResponse)
				{
					return AWSConfigs.LoggingConfig.LogResponses == ResponseLoggingOption.Always;
				}
				return true;
			}
			return false;
		}
	}
}
