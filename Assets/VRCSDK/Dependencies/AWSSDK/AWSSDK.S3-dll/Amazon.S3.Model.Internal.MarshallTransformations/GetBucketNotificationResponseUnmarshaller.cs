using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;
using System.Collections.Generic;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class GetBucketNotificationResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static GetBucketNotificationResponseUnmarshaller _instance;

		public static GetBucketNotificationResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetBucketNotificationResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			GetBucketNotificationResponse getBucketNotificationResponse = new GetBucketNotificationResponse();
			getBucketNotificationResponse.TopicConfigurations = new List<TopicConfiguration>();
			while (context.Read())
			{
				if (context.get_IsStartElement())
				{
					UnmarshallResult(context, getBucketNotificationResponse);
				}
			}
			return getBucketNotificationResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, GetBucketNotificationResponse response)
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
					if (context.TestExpression("TopicConfiguration", num))
					{
						response.TopicConfigurations.Add(TopicConfigurationUnmarshaller.Instance.Unmarshall(context));
					}
					else if (context.TestExpression("QueueConfiguration", num))
					{
						response.QueueConfigurations.Add(QueueConfigurationUnmarshaller.Instance.Unmarshall(context));
					}
					else if (context.TestExpression("CloudFunctionConfiguration", num))
					{
						response.LambdaFunctionConfigurations.Add(LambdaFunctionConfigurationUnmarshaller.Instance.Unmarshall(context));
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
