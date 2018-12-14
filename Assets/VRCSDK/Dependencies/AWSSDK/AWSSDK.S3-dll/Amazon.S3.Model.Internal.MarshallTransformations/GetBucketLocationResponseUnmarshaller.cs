using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class GetBucketLocationResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static GetBucketLocationResponseUnmarshaller _instance;

		public static GetBucketLocationResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetBucketLocationResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			GetBucketLocationResponse getBucketLocationResponse = new GetBucketLocationResponse();
			UnmarshallResult(context, getBucketLocationResponse);
			return getBucketLocationResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, GetBucketLocationResponse response)
		{
			int currentDepth = context.get_CurrentDepth();
			int num = 1;
			while (context.Read())
			{
				if (context.get_IsStartElement() || context.get_IsAttribute())
				{
					if (context.TestExpression("LocationConstraint", num))
					{
						response.Location = StringUnmarshaller.GetInstance().Unmarshall(context);
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
