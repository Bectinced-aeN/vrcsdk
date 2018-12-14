using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class FilterUnmarshaller : IUnmarshaller<Filter, XmlUnmarshallerContext>, IUnmarshaller<Filter, JsonUnmarshallerContext>
	{
		private static FilterUnmarshaller _instance;

		public static FilterUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new FilterUnmarshaller();
				}
				return _instance;
			}
		}

		public Filter Unmarshall(XmlUnmarshallerContext context)
		{
			Filter filter = new Filter();
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
					if (context.TestExpression("S3Key", num))
					{
						filter.S3KeyFilter = S3KeyFilterUnmarshaller.Instance.Unmarshall(context);
					}
				}
				else if (context.get_IsEndElement() && context.get_CurrentDepth() < currentDepth)
				{
					return filter;
				}
			}
			return filter;
		}

		public Filter Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
