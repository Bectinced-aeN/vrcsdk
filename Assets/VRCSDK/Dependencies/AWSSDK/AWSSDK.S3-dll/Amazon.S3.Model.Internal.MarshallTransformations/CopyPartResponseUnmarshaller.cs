using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class CopyPartResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static CopyPartResponseUnmarshaller _instance;

		public static CopyPartResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new CopyPartResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			CopyPartResponse copyPartResponse = new CopyPartResponse();
			while (context.Read())
			{
				if (context.get_IsStartElement())
				{
					UnmarshallResult(context, copyPartResponse);
				}
			}
			return copyPartResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, CopyPartResponse response)
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
						response.LastModified = DateTimeUnmarshaller.GetInstance().Unmarshall(context);
					}
				}
				else if (context.get_IsEndElement() && context.get_CurrentDepth() < currentDepth)
				{
					return;
				}
			}
			IWebResponseData responseData = context.get_ResponseData();
			if (responseData.IsHeaderPresent("x-amz-copy-source-version-id"))
			{
				response.CopySourceVersionId = S3Transforms.ToString(responseData.GetHeaderValue("x-amz-copy-source-version-id"));
			}
			if (responseData.IsHeaderPresent("x-amz-server-side-encryption"))
			{
				response.ServerSideEncryptionMethod = S3Transforms.ToString(responseData.GetHeaderValue("x-amz-server-side-encryption"));
			}
			if (responseData.IsHeaderPresent("x-amz-server-side-encryption-aws-kms-key-id"))
			{
				response.ServerSideEncryptionKeyManagementServiceKeyId = S3Transforms.ToString(responseData.GetHeaderValue("x-amz-server-side-encryption-aws-kms-key-id"));
			}
		}
	}
}
