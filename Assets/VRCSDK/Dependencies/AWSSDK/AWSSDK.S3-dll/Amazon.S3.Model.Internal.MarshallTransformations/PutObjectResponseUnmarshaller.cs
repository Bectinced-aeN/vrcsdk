using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;
using Amazon.S3.Util;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class PutObjectResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static PutObjectResponseUnmarshaller _instance;

		public static PutObjectResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new PutObjectResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			PutObjectResponse putObjectResponse = new PutObjectResponse();
			UnmarshallResult(context, putObjectResponse);
			return putObjectResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, PutObjectResponse response)
		{
			IWebResponseData responseData = context.get_ResponseData();
			if (responseData.IsHeaderPresent("x-amz-expiration"))
			{
				response.Expiration = new Expiration(responseData.GetHeaderValue("x-amz-expiration"));
			}
			if (responseData.IsHeaderPresent("x-amz-server-side-encryption"))
			{
				response.ServerSideEncryptionMethod = S3Transforms.ToString(responseData.GetHeaderValue("x-amz-server-side-encryption"));
			}
			if (responseData.IsHeaderPresent("ETag"))
			{
				response.ETag = S3Transforms.ToString(responseData.GetHeaderValue("ETag"));
			}
			if (responseData.IsHeaderPresent("x-amz-version-id"))
			{
				response.VersionId = S3Transforms.ToString(responseData.GetHeaderValue("x-amz-version-id"));
			}
			if (responseData.IsHeaderPresent("x-amz-server-side-encryption-aws-kms-key-id"))
			{
				response.ServerSideEncryptionKeyManagementServiceKeyId = S3Transforms.ToString(responseData.GetHeaderValue("x-amz-server-side-encryption-aws-kms-key-id"));
			}
			if (responseData.IsHeaderPresent(S3Constants.AmzHeaderRequestCharged))
			{
				response.RequestCharged = RequestCharged.FindValue(responseData.GetHeaderValue(S3Constants.AmzHeaderRequestCharged));
			}
		}
	}
}
