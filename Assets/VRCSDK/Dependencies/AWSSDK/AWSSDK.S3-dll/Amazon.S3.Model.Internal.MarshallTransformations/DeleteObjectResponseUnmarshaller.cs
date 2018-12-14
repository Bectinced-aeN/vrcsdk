using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;
using Amazon.S3.Util;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class DeleteObjectResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static DeleteObjectResponseUnmarshaller _instance;

		public static DeleteObjectResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new DeleteObjectResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			DeleteObjectResponse deleteObjectResponse = new DeleteObjectResponse();
			UnmarshallResult(context, deleteObjectResponse);
			return deleteObjectResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, DeleteObjectResponse response)
		{
			IWebResponseData responseData = context.get_ResponseData();
			if (responseData.IsHeaderPresent("x-amz-delete-marker"))
			{
				response.DeleteMarker = S3Transforms.ToString(responseData.GetHeaderValue("x-amz-delete-marker"));
			}
			if (responseData.IsHeaderPresent("x-amz-version-id"))
			{
				response.VersionId = S3Transforms.ToString(responseData.GetHeaderValue("x-amz-version-id"));
			}
			if (responseData.IsHeaderPresent(S3Constants.AmzHeaderRequestCharged))
			{
				response.RequestCharged = RequestCharged.FindValue(responseData.GetHeaderValue(S3Constants.AmzHeaderRequestCharged));
			}
		}
	}
}
