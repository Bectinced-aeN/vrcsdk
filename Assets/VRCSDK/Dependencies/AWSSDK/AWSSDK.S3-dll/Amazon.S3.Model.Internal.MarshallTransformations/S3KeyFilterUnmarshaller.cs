using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class S3KeyFilterUnmarshaller : IUnmarshaller<S3KeyFilter, XmlUnmarshallerContext>, IUnmarshaller<S3KeyFilter, JsonUnmarshallerContext>
	{
		private static S3KeyFilterUnmarshaller _instance;

		public static S3KeyFilterUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new S3KeyFilterUnmarshaller();
				}
				return _instance;
			}
		}

		public S3KeyFilter Unmarshall(XmlUnmarshallerContext context)
		{
			S3KeyFilter s3KeyFilter = new S3KeyFilter();
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
					if (context.TestExpression("FilterRule", num))
					{
						s3KeyFilter.FilterRules.Add(FilterRuleUnmarshaller.Instance.Unmarshall(context));
					}
				}
				else if (context.get_IsEndElement() && context.get_CurrentDepth() < currentDepth)
				{
					return s3KeyFilter;
				}
			}
			return s3KeyFilter;
		}

		public S3KeyFilter Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
