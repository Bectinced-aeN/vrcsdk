using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class AnalyticsS3BucketDestinationUnmarshaller : IUnmarshaller<AnalyticsS3BucketDestination, XmlUnmarshallerContext>, IUnmarshaller<AnalyticsS3BucketDestination, JsonUnmarshallerContext>
	{
		private static AnalyticsS3BucketDestinationUnmarshaller _instance;

		public static AnalyticsS3BucketDestinationUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new AnalyticsS3BucketDestinationUnmarshaller();
				}
				return _instance;
			}
		}

		public AnalyticsS3BucketDestination Unmarshall(XmlUnmarshallerContext context)
		{
			AnalyticsS3BucketDestination analyticsS3BucketDestination = new AnalyticsS3BucketDestination();
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
					if (context.TestExpression("Format", num))
					{
						analyticsS3BucketDestination.Format = ConstantClass.op_Implicit(AnalyticsS3ExportFileFormat.FindValue(StringUnmarshaller.get_Instance().Unmarshall(context)));
					}
					else if (context.TestExpression("BucketAccountId", num))
					{
						analyticsS3BucketDestination.BucketAccountId = StringUnmarshaller.get_Instance().Unmarshall(context);
					}
					else if (context.TestExpression("Bucket", num))
					{
						analyticsS3BucketDestination.BucketName = StringUnmarshaller.get_Instance().Unmarshall(context);
					}
					else if (context.TestExpression("Prefix", num))
					{
						analyticsS3BucketDestination.Prefix = StringUnmarshaller.get_Instance().Unmarshall(context);
					}
				}
				else if (context.get_IsEndElement() && context.get_CurrentDepth() < currentDepth)
				{
					return analyticsS3BucketDestination;
				}
			}
			return analyticsS3BucketDestination;
		}

		public AnalyticsS3BucketDestination Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
