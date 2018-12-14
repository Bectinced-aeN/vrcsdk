using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class GetBucketAccelerateConfigurationResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static GetBucketAccelerateConfigurationResponseUnmarshaller _instance;

		public static GetBucketAccelerateConfigurationResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetBucketAccelerateConfigurationResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			GetBucketAccelerateConfigurationResponse getBucketAccelerateConfigurationResponse = new GetBucketAccelerateConfigurationResponse();
			while (context.Read())
			{
				if (context.get_IsStartElement())
				{
					UnmarshallResult(context, getBucketAccelerateConfigurationResponse);
				}
			}
			return getBucketAccelerateConfigurationResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, GetBucketAccelerateConfigurationResponse response)
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
						response.Status = StringUnmarshaller.GetInstance().Unmarshall(context);
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
