using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class GetBucketVersioningResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static GetBucketVersioningResponseUnmarshaller _instance;

		public static GetBucketVersioningResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetBucketVersioningResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			GetBucketVersioningResponse getBucketVersioningResponse = new GetBucketVersioningResponse();
			while (context.Read())
			{
				if (context.get_IsStartElement())
				{
					UnmarshallResult(context, getBucketVersioningResponse);
				}
			}
			return getBucketVersioningResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, GetBucketVersioningResponse response)
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
					if (context.TestExpression("Status", num))
					{
						response.VersioningConfig.Status = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("MfaDelete", num))
					{
						response.VersioningConfig.EnableMfaDelete = string.Equals(StringUnmarshaller.GetInstance().Unmarshall(context), "Enabled");
					}
				}
				else if (context.get_IsEndElement() && context.get_CurrentDepth() < currentDepth)
				{
					break;
				}
			}
		}
	}
}
