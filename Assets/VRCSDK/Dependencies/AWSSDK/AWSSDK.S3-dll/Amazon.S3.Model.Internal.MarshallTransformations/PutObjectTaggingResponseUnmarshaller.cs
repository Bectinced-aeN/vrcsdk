using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class PutObjectTaggingResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static PutObjectTaggingResponseUnmarshaller _instance;

		public static PutObjectTaggingResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new PutObjectTaggingResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			PutObjectTaggingResponse putObjectTaggingResponse = new PutObjectTaggingResponse();
			UnmarshallResult(context, putObjectTaggingResponse);
			return putObjectTaggingResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, PutObjectTaggingResponse response)
		{
			IWebResponseData responseData = context.get_ResponseData();
			if (responseData.IsHeaderPresent("x-amz-version-id"))
			{
				response.VersionId = S3Transforms.ToString(responseData.GetHeaderValue("x-amz-version-id"));
			}
		}
	}
}
