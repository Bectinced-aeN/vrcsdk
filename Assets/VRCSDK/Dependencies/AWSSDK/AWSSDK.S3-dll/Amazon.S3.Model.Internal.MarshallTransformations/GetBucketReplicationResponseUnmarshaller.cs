using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class GetBucketReplicationResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static GetBucketReplicationResponseUnmarshaller _instance;

		public static GetBucketReplicationResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetBucketReplicationResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			GetBucketReplicationResponse getBucketReplicationResponse = new GetBucketReplicationResponse();
			while (context.Read())
			{
				if (context.get_IsStartElement())
				{
					UnmarshallResult(context, getBucketReplicationResponse);
				}
			}
			return getBucketReplicationResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, GetBucketReplicationResponse response)
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
					if (context.TestExpression("Role", num))
					{
						response.Configuration.Role = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Rule", num))
					{
						response.Configuration.Rules.Add(ReplicationRuleUnmarshaller.Instance.Unmarshall(context));
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
