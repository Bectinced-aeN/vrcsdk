using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class ContentsItemUnmarshaller : IUnmarshaller<S3Object, XmlUnmarshallerContext>, IUnmarshaller<S3Object, JsonUnmarshallerContext>
	{
		private static ContentsItemUnmarshaller _instance;

		public static ContentsItemUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new ContentsItemUnmarshaller();
				}
				return _instance;
			}
		}

		public S3Object Unmarshall(XmlUnmarshallerContext context)
		{
			S3Object s3Object = new S3Object();
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
						s3Object.ETag = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Key", num))
					{
						s3Object.Key = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("LastModified", num))
					{
						s3Object.LastModified = DateTimeUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Owner", num))
					{
						s3Object.Owner = OwnerUnmarshaller.Instance.Unmarshall(context);
					}
					else if (context.TestExpression("Size", num))
					{
						s3Object.Size = LongUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("StorageClass", num))
					{
						s3Object.StorageClass = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
				}
				else if (context.get_IsEndElement() && context.get_CurrentDepth() < currentDepth)
				{
					return s3Object;
				}
			}
			return s3Object;
		}

		public S3Object Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
