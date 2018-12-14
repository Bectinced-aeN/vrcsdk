using System;
using System.IO;
using System.Net;

namespace Amazon.Runtime.Internal.Transform
{
	public abstract class JsonResponseUnmarshaller : ResponseUnmarshaller
	{
		public override AmazonWebServiceResponse Unmarshall(UnmarshallerContext input)
		{
			JsonUnmarshallerContext jsonUnmarshallerContext = input as JsonUnmarshallerContext;
			if (jsonUnmarshallerContext != null)
			{
				string headerValue = jsonUnmarshallerContext.ResponseData.GetHeaderValue("x-amzn-RequestId");
				try
				{
					AmazonWebServiceResponse amazonWebServiceResponse = Unmarshall(jsonUnmarshallerContext);
					amazonWebServiceResponse.ResponseMetadata = new ResponseMetadata();
					amazonWebServiceResponse.ResponseMetadata.RequestId = headerValue;
					return amazonWebServiceResponse;
				}
				catch (Exception innerException)
				{
					throw new AmazonUnmarshallingException(headerValue, jsonUnmarshallerContext.CurrentPath, innerException);
				}
			}
			throw new InvalidOperationException("Unsupported UnmarshallerContext");
		}

		public override AmazonServiceException UnmarshallException(UnmarshallerContext input, Exception innerException, HttpStatusCode statusCode)
		{
			JsonUnmarshallerContext jsonUnmarshallerContext = input as JsonUnmarshallerContext;
			if (jsonUnmarshallerContext == null)
			{
				throw new InvalidOperationException("Unsupported UnmarshallerContext");
			}
			AmazonServiceException ex = UnmarshallException(jsonUnmarshallerContext, innerException, statusCode);
			ex.RequestId = jsonUnmarshallerContext.ResponseData.GetHeaderValue("x-amzn-RequestId");
			return ex;
		}

		public abstract AmazonWebServiceResponse Unmarshall(JsonUnmarshallerContext input);

		public abstract AmazonServiceException UnmarshallException(JsonUnmarshallerContext input, Exception innerException, HttpStatusCode statusCode);

		protected override UnmarshallerContext ConstructUnmarshallerContext(Stream responseStream, bool maintainResponseBody, IWebResponseData response)
		{
			return new JsonUnmarshallerContext(responseStream, maintainResponseBody, response);
		}

		protected override bool ShouldReadEntireResponse(IWebResponseData response, bool readEntireResponse)
		{
			if (readEntireResponse)
			{
				return response.ContentType != "application/octet-stream";
			}
			return false;
		}
	}
}
