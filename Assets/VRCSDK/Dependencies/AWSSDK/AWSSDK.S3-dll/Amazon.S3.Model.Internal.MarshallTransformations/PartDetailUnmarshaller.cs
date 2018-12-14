using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class PartDetailUnmarshaller : IUnmarshaller<PartDetail, XmlUnmarshallerContext>, IUnmarshaller<PartDetail, JsonUnmarshallerContext>
	{
		private static PartDetailUnmarshaller _instance;

		public static PartDetailUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new PartDetailUnmarshaller();
				}
				return _instance;
			}
		}

		public PartDetail Unmarshall(XmlUnmarshallerContext context)
		{
			PartDetail partDetail = new PartDetail();
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
						partDetail.ETag = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("LastModified", num))
					{
						partDetail.LastModified = DateTimeUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("PartNumber", num))
					{
						partDetail.PartNumber = IntUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Size", num))
					{
						partDetail.Size = IntUnmarshaller.GetInstance().Unmarshall(context);
					}
				}
				else if (context.get_IsEndElement() && context.get_CurrentDepth() < currentDepth)
				{
					return partDetail;
				}
			}
			return partDetail;
		}

		public PartDetail Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
