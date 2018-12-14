using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class ReplicationDestinationUnmarshaller : IUnmarshaller<ReplicationDestination, XmlUnmarshallerContext>, IUnmarshaller<ReplicationDestination, JsonUnmarshallerContext>
	{
		private static ReplicationDestinationUnmarshaller _instance;

		public static ReplicationDestinationUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new ReplicationDestinationUnmarshaller();
				}
				return _instance;
			}
		}

		public ReplicationDestination Unmarshall(XmlUnmarshallerContext context)
		{
			ReplicationDestination replicationDestination = new ReplicationDestination();
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
					if (context.TestExpression("Bucket", num))
					{
						replicationDestination.BucketArn = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("StorageClass", num))
					{
						replicationDestination.StorageClass = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
				}
				else if (context.get_IsEndElement() && context.get_CurrentDepth() < currentDepth)
				{
					return replicationDestination;
				}
			}
			return replicationDestination;
		}

		public ReplicationDestination Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
