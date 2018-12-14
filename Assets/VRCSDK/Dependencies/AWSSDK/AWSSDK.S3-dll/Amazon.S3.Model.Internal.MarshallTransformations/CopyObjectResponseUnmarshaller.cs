using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;
using Amazon.S3.Util;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class CopyObjectResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static CopyObjectResponseUnmarshaller _instance;

		public static CopyObjectResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new CopyObjectResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			CopyObjectResponse copyObjectResponse = new CopyObjectResponse();
			while (context.Read())
			{
				if (context.get_IsStartElement())
				{
					UnmarshallResult(context, copyObjectResponse);
				}
			}
			return copyObjectResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, CopyObjectResponse response)
		{
			int currentDepth = context.get_CurrentDepth();
			int num = currentDepth + 1;
			if (context.get_IsStartOfDocument())
			{
				num += 2;
			}
			while (context.Read())
			{
				if (context.get_IsStartElement() || context.get_IsAttribute())
				{
					if (context.TestExpression("ETag", num))
					{
						response.ETag = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("LastModified", num))
					{
						response.LastModified = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
				}
				else if (context.get_IsEndElement() && context.get_CurrentDepth() < currentDepth)
				{
					return;
				}
			}
			IWebResponseData responseData = context.get_ResponseData();
			if (responseData.IsHeaderPresent("x-amz-expiration"))
			{
				response.Expiration = new Expiration(responseData.GetHeaderValue("x-amz-expiration"));
			}
			if (responseData.IsHeaderPresent("x-amz-copy-source-version-id"))
			{
				response.SourceVersionId = S3Transforms.ToString(responseData.GetHeaderValue("x-amz-copy-source-version-id"));
			}
			if (responseData.IsHeaderPresent("x-amz-server-side-encryption"))
			{
				response.ServerSideEncryptionMethod = S3Transforms.ToString(responseData.GetHeaderValue("x-amz-server-side-encryption"));
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
