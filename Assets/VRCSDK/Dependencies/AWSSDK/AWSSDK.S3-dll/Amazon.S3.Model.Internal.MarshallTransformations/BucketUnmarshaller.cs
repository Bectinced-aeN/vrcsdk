using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class BucketUnmarshaller : IUnmarshaller<S3Bucket, XmlUnmarshallerContext>, IUnmarshaller<S3Bucket, JsonUnmarshallerContext>
	{
		private static BucketUnmarshaller _instance;

		public static BucketUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new BucketUnmarshaller();
				}
				return _instance;
			}
		}

		public S3Bucket Unmarshall(XmlUnmarshallerContext context)
		{
			S3Bucket s3Bucket = new S3Bucket();
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
					if (context.TestExpression("CreationDate", num))
					{
						s3Bucket.CreationDate = DateTimeUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Name", num))
					{
						s3Bucket.BucketName = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
				}
				else if (context.get_IsEndElement() && context.get_CurrentDepth() < currentDepth)
				{
					return s3Bucket;
				}
			}
			return s3Bucket;
		}

		public S3Bucket Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
