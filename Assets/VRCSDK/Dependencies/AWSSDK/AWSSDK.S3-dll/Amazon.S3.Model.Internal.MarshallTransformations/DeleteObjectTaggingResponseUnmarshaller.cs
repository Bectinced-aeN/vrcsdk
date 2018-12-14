using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class DeleteObjectTaggingResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static DeleteObjectTaggingResponseUnmarshaller _instance;

		public static DeleteObjectTaggingResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new DeleteObjectTaggingResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			DeleteObjectTaggingResponse deleteObjectTaggingResponse = new DeleteObjectTaggingResponse();
			UnmarshallResult(context, deleteObjectTaggingResponse);
			return deleteObjectTaggingResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, DeleteObjectTaggingResponse response)
		{
			IWebResponseData responseData = context.get_ResponseData();
			if (responseData.IsHeaderPresent("x-amz-version-id"))
			{
				response.VersionId = S3Transforms.ToString(responseData.GetHeaderValue("x-amz-version-id"));
			}
		}
	}
}
