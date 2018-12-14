using System;
using System.IO;
using System.Net;

namespace Amazon.Runtime.Internal.Transform
{
	public abstract class XmlResponseUnmarshaller : ResponseUnmarshaller
	{
		public override AmazonWebServiceResponse Unmarshall(UnmarshallerContext input)
		{
			XmlUnmarshallerContext xmlUnmarshallerContext = input as XmlUnmarshallerContext;
			if (xmlUnmarshallerContext == null)
			{
				throw new InvalidOperationException("Unsupported UnmarshallerContext");
			}
			AmazonWebServiceResponse amazonWebServiceResponse = Unmarshall(xmlUnmarshallerContext);
			if (xmlUnmarshallerContext.ResponseData.IsHeaderPresent("x-amzn-RequestId") && !string.IsNullOrEmpty(xmlUnmarshallerContext.ResponseData.GetHeaderValue("x-amzn-RequestId")))
			{
				if (amazonWebServiceResponse.ResponseMetadata == null)
				{
					amazonWebServiceResponse.ResponseMetadata = new ResponseMetadata();
				}
				amazonWebServiceResponse.ResponseMetadata.RequestId = xmlUnmarshallerContext.ResponseData.GetHeaderValue("x-amzn-RequestId");
			}
			else if (xmlUnmarshallerContext.ResponseData.IsHeaderPresent("x-amz-request-id") && !string.IsNullOrEmpty(xmlUnmarshallerContext.ResponseData.GetHeaderValue("x-amz-request-id")))
			{
				if (amazonWebServiceResponse.ResponseMetadata == null)
				{
					amazonWebServiceResponse.ResponseMetadata = new ResponseMetadata();
				}
				amazonWebServiceResponse.ResponseMetadata.RequestId = xmlUnmarshallerContext.ResponseData.GetHeaderValue("x-amz-request-id");
			}
			return amazonWebServiceResponse;
		}

		public override AmazonServiceException UnmarshallException(UnmarshallerContext input, Exception innerException, HttpStatusCode statusCode)
		{
			XmlUnmarshallerContext xmlUnmarshallerContext = input as XmlUnmarshallerContext;
			if (xmlUnmarshallerContext == null)
			{
				throw new InvalidOperationException("Unsupported UnmarshallerContext");
			}
			return UnmarshallException(xmlUnmarshallerContext, innerException, statusCode);
		}

		public abstract AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext input);

		public abstract AmazonServiceException UnmarshallException(XmlUnmarshallerContext input, Exception innerException, HttpStatusCode statusCode);

		protected override UnmarshallerContext ConstructUnmarshallerContext(Stream responseStream, bool maintainResponseBody, IWebResponseData response)
		{
			return new XmlUnmarshallerContext(responseStream, maintainResponseBody, response);
		}
	}
}
