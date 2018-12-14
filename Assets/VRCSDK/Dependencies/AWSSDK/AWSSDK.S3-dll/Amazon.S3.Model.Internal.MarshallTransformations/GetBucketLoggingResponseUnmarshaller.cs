using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class GetBucketLoggingResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static GetBucketLoggingResponseUnmarshaller _instance;

		public static GetBucketLoggingResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetBucketLoggingResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			GetBucketLoggingResponse getBucketLoggingResponse = new GetBucketLoggingResponse();
			while (context.Read())
			{
				if (context.get_IsStartElement())
				{
					UnmarshallResult(context, getBucketLoggingResponse);
				}
			}
			return getBucketLoggingResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, GetBucketLoggingResponse response)
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
					if (context.TestExpression("LoggingEnabled", num))
					{
						response.BucketLoggingConfig = LoggingEnabledUnmarshaller.Instance.Unmarshall(context);
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
