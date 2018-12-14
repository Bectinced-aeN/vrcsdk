using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class GrantUnmarshaller : IUnmarshaller<S3Grant, XmlUnmarshallerContext>, IUnmarshaller<S3Grant, JsonUnmarshallerContext>
	{
		private static GrantUnmarshaller _instance;

		public static GrantUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GrantUnmarshaller();
				}
				return _instance;
			}
		}

		public S3Grant Unmarshall(XmlUnmarshallerContext context)
		{
			S3Grant s3Grant = new S3Grant();
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
					if (context.TestExpression("Grantee", num))
					{
						s3Grant.Grantee = GranteeUnmarshaller.Instance.Unmarshall(context);
					}
					else if (context.TestExpression("Permission", num))
					{
						s3Grant.Permission = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
				}
				else if (context.get_IsEndElement() && context.get_CurrentDepth() < currentDepth)
				{
					return s3Grant;
				}
			}
			return s3Grant;
		}

		public S3Grant Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
