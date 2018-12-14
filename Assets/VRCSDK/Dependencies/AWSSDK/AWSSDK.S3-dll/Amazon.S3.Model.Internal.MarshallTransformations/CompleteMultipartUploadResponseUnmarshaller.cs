using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;
using Amazon.S3.Util;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class CompleteMultipartUploadResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static CompleteMultipartUploadResponseUnmarshaller _instance;

		public static CompleteMultipartUploadResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new CompleteMultipartUploadResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			CompleteMultipartUploadResponse completeMultipartUploadResponse = new CompleteMultipartUploadResponse();
			while (context.Read())
			{
				if (context.get_IsStartElement())
				{
					UnmarshallResult(context, completeMultipartUploadResponse);
				}
			}
			return completeMultipartUploadResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, CompleteMultipartUploadResponse response)
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
					if (context.TestExpression("Location", num))
					{
						response.Location = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Bucket", num))
					{
						response.BucketName = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Key", num))
					{
						response.Key = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("ETag", num))
					{
						response.ETag = StringUnmarshaller.GetInstance().Unmarshall(context);
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
			if (responseData.IsHeaderPresent("x-amz-server-side-encryption"))
			{
				response.ServerSideEncryptionMethod = S3Transforms.ToString(responseData.GetHeaderValue("x-amz-server-side-encryption"));
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
