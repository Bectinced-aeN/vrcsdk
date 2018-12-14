using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;
using Amazon.S3.Util;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class UploadPartResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static UploadPartResponseUnmarshaller _instance;

		public static UploadPartResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new UploadPartResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			UploadPartResponse uploadPartResponse = new UploadPartResponse();
			UnmarshallResult(context, uploadPartResponse);
			return uploadPartResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, UploadPartResponse response)
		{
			IWebResponseData responseData = context.get_ResponseData();
			if (responseData.IsHeaderPresent("x-amz-server-side-encryption"))
			{
				response.ServerSideEncryptionMethod = S3Transforms.ToString(responseData.GetHeaderValue("x-amz-server-side-encryption"));
			}
			if (responseData.IsHeaderPresent("ETag"))
			{
				response.ETag = S3Transforms.ToString(responseData.GetHeaderValue("ETag"));
			}
			if (responseData.IsHeaderPresent(S3Constants.AmzHeaderRequestCharged))
			{
				response.RequestCharged = RequestCharged.FindValue(responseData.GetHeaderValue(S3Constants.AmzHeaderRequestCharged));
			}
		}
	}
}
