using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;
using System;
using System.IO;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class GetBucketPolicyResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static GetBucketPolicyResponseUnmarshaller _instance;

		public static GetBucketPolicyResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetBucketPolicyResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			GetBucketPolicyResponse getBucketPolicyResponse = new GetBucketPolicyResponse();
			UnmarshallResult(context, getBucketPolicyResponse);
			return getBucketPolicyResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, GetBucketPolicyResponse response)
		{
			using (StreamReader streamReader = new StreamReader(context.get_Stream()))
			{
				response.Policy = streamReader.ReadToEnd();
				if (response.Policy.StartsWith("<?xml", StringComparison.OrdinalIgnoreCase))
				{
					response.Policy = null;
				}
			}
		}
	}
}
