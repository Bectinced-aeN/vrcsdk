using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class VersionsItemUnmarshaller : IUnmarshaller<S3ObjectVersion, XmlUnmarshallerContext>, IUnmarshaller<S3ObjectVersion, JsonUnmarshallerContext>
	{
		private static VersionsItemUnmarshaller _instance;

		public static VersionsItemUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new VersionsItemUnmarshaller();
				}
				return _instance;
			}
		}

		public S3ObjectVersion Unmarshall(XmlUnmarshallerContext context)
		{
			S3ObjectVersion s3ObjectVersion = new S3ObjectVersion();
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
					if (context.TestExpression("ETag", num))
					{
						s3ObjectVersion.ETag = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("IsLatest", num))
					{
						s3ObjectVersion.IsLatest = BoolUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Key", num))
					{
						s3ObjectVersion.Key = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("LastModified", num))
					{
						s3ObjectVersion.LastModified = DateTimeUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Owner", num))
					{
						s3ObjectVersion.Owner = OwnerUnmarshaller.Instance.Unmarshall(context);
					}
					else if (context.TestExpression("Size", num))
					{
						s3ObjectVersion.Size = LongUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("StorageClass", num))
					{
						s3ObjectVersion.StorageClass = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("VersionId", num))
					{
						s3ObjectVersion.VersionId = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
				}
				else if (context.get_IsEndElement() && context.get_CurrentDepth() < currentDepth)
				{
					return s3ObjectVersion;
				}
			}
			return s3ObjectVersion;
		}

		public S3ObjectVersion Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
