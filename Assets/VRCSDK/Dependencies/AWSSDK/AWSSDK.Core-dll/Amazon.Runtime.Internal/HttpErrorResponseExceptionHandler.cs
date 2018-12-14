using Amazon.Runtime.Internal.Transform;
using Amazon.Runtime.Internal.Util;
using System;
using System.Net;

namespace Amazon.Runtime.Internal
{
	public class HttpErrorResponseExceptionHandler : ExceptionHandler<HttpErrorResponseException>
	{
		public HttpErrorResponseExceptionHandler(ILogger logger)
			: base(logger)
		{
		}

		public override bool HandleException(IExecutionContext executionContext, HttpErrorResponseException exception)
		{
			IRequestContext requestContext = executionContext.RequestContext;
			IWebResponseData response = exception.Response;
			if (HandleSuppressed404(executionContext, response))
			{
				return false;
			}
			requestContext.Metrics.AddProperty(Metric.StatusCode, response.StatusCode);
			AmazonServiceException ex = null;
			try
			{
				using (response.ResponseBody)
				{
					ResponseUnmarshaller unmarshaller = requestContext.Unmarshaller;
					bool readEntireResponse = true;
					UnmarshallerContext unmarshallerContext = unmarshaller.CreateContext(response, readEntireResponse, response.ResponseBody.OpenResponse(), requestContext.Metrics);
					try
					{
						ex = unmarshaller.UnmarshallException(unmarshallerContext, exception, response.StatusCode);
					}
					catch (Exception ex2)
					{
						if (ex2 is AmazonServiceException || ex2 is AmazonClientException)
						{
							throw;
						}
						string headerValue = response.GetHeaderValue("x-amzn-RequestId");
						string responseBody = unmarshallerContext.ResponseBody;
						throw new AmazonUnmarshallingException(headerValue, null, responseBody, ex2);
					}
					requestContext.Metrics.AddProperty(Metric.AWSRequestID, ex.RequestId);
					requestContext.Metrics.AddProperty(Metric.AWSErrorCode, ex.ErrorCode);
					if (requestContext.ClientConfig.LogResponse || AWSConfigs.LoggingConfig.LogResponses != 0)
					{
						base.Logger.Error(ex, "Received error response: [{0}]", unmarshallerContext.ResponseBody);
					}
				}
			}
			catch (Exception exception2)
			{
				base.Logger.Error(exception2, "Failed to unmarshall a service error response.");
				throw;
			}
			throw ex;
		}

		private bool HandleSuppressed404(IExecutionContext executionContext, IWebResponseData httpErrorResponse)
		{
			IRequestContext requestContext = executionContext.RequestContext;
			IResponseContext responseContext = executionContext.ResponseContext;
			if (httpErrorResponse != null && httpErrorResponse.StatusCode == HttpStatusCode.NotFound && requestContext.Request.Suppress404Exceptions)
			{
				using (httpErrorResponse.ResponseBody)
				{
					ResponseUnmarshaller unmarshaller = requestContext.Unmarshaller;
					bool readEntireResponse = requestContext.ClientConfig.LogResponse || AWSConfigs.LoggingConfig.LogResponses != ResponseLoggingOption.Never;
					UnmarshallerContext input = unmarshaller.CreateContext(httpErrorResponse, readEntireResponse, httpErrorResponse.ResponseBody.OpenResponse(), requestContext.Metrics);
					try
					{
						responseContext.Response = unmarshaller.Unmarshall(input);
						responseContext.Response.ContentLength = httpErrorResponse.ContentLength;
						responseContext.Response.HttpStatusCode = httpErrorResponse.StatusCode;
						return true;
					}
					catch (Exception exception)
					{
						base.Logger.Debug(exception, "Failed to unmarshall 404 response when it was supressed.");
					}
				}
			}
			return false;
		}
	}
}
