using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class LoggingEnabledUnmarshaller : IUnmarshaller<S3BucketLoggingConfig, XmlUnmarshallerContext>, IUnmarshaller<S3BucketLoggingConfig, JsonUnmarshallerContext>
	{
		private static LoggingEnabledUnmarshaller _instance;

		public static LoggingEnabledUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new LoggingEnabledUnmarshaller();
				}
				return _instance;
			}
		}

		public S3BucketLoggingConfig Unmarshall(XmlUnmarshallerContext context)
		{
			S3BucketLoggingConfig s3BucketLoggingConfig = new S3BucketLoggingConfig();
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
					if (context.TestExpression("TargetBucket", num))
					{
						s3BucketLoggingConfig.TargetBucketName = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Grant", num + 1))
					{
						s3BucketLoggingConfig.Grants.Add(GrantUnmarshaller.Instance.Unmarshall(context));
					}
					else if (context.TestExpression("TargetPrefix", num))
					{
						s3BucketLoggingConfig.TargetPrefix = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
				}
				else if (context.get_IsEndElement() && context.get_CurrentDepth() < currentDepth)
				{
					return s3BucketLoggingConfig;
				}
			}
			return s3BucketLoggingConfig;
		}

		public S3BucketLoggingConfig Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
