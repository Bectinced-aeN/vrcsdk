using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;
using Amazon.Runtime.Internal.Util;
using System;
using System.IO;
using System.Net;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public abstract class S3ReponseUnmarshaller : XmlResponseUnmarshaller
	{
		public override UnmarshallerContext CreateContext(IWebResponseData response, bool readEntireResponse, Stream stream, RequestMetrics metrics)
		{
			if (response.IsHeaderPresent("x-amz-id-2"))
			{
				metrics.AddProperty(2, (object)response.GetHeaderValue("x-amz-id-2"));
			}
			if (response.IsHeaderPresent("X-Amz-Cf-Id"))
			{
				metrics.AddProperty(25, (object)response.GetHeaderValue("X-Amz-Cf-Id"));
			}
			return this.CreateContext(response, readEntireResponse, stream, metrics);
		}

		public override AmazonWebServiceResponse Unmarshall(UnmarshallerContext input)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			AmazonWebServiceResponse val = this.Unmarshall(input);
			if (val.get_ResponseMetadata() == null)
			{
				val.set_ResponseMetadata(new ResponseMetadata());
			}
			val.get_ResponseMetadata().get_Metadata().Add("x-amz-id-2", input.get_ResponseData().GetHeaderValue("x-amz-id-2"));
			if (input.get_ResponseData().IsHeaderPresent("X-Amz-Cf-Id"))
			{
				val.get_ResponseMetadata().get_Metadata().Add("X-Amz-Cf-Id", input.get_ResponseData().GetHeaderValue("X-Amz-Cf-Id"));
			}
			return val;
		}

		protected override UnmarshallerContext ConstructUnmarshallerContext(Stream responseStream, bool maintainResponseBody, IWebResponseData response)
		{
			return new S3UnmarshallerContext(responseStream, maintainResponseBody, response);
		}

		public override AmazonServiceException UnmarshallException(XmlUnmarshallerContext context, Exception innerException, HttpStatusCode statusCode)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			S3ErrorResponse s3ErrorResponse = S3ErrorResponseUnmarshaller.Instance.Unmarshall(context);
			AmazonS3Exception ex = new AmazonS3Exception(s3ErrorResponse.get_Message(), innerException, s3ErrorResponse.get_Type(), s3ErrorResponse.get_Code(), s3ErrorResponse.get_RequestId(), statusCode, s3ErrorResponse.Id2, s3ErrorResponse.AmzCfId);
			ex.Region = s3ErrorResponse.Region;
			if (s3ErrorResponse.ParsingException != null)
			{
				string responseBody = context.get_ResponseBody();
				if (!string.IsNullOrEmpty(responseBody))
				{
					ex.ResponseBody = responseBody;
				}
			}
			return ex;
		}

		protected S3ReponseUnmarshaller()
			: this()
		{
		}
	}
}
