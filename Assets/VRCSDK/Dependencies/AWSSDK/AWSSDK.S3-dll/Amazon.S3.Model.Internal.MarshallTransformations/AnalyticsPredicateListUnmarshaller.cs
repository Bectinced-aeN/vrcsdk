using Amazon.Runtime.Internal.Transform;
using System.Collections.Generic;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class AnalyticsPredicateListUnmarshaller : IUnmarshaller<List<AnalyticsFilterPredicate>, XmlUnmarshallerContext>, IUnmarshaller<List<AnalyticsFilterPredicate>, JsonUnmarshallerContext>
	{
		private static AnalyticsPredicateListUnmarshaller _instance;

		public static AnalyticsPredicateListUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new AnalyticsPredicateListUnmarshaller();
				}
				return _instance;
			}
		}

		public List<AnalyticsFilterPredicate> Unmarshall(XmlUnmarshallerContext context)
		{
			List<AnalyticsFilterPredicate> list = new List<AnalyticsFilterPredicate>();
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
					if (context.TestExpression("Prefix", num))
					{
						list.Add(new AnalyticsPrefixPredicate(StringUnmarshaller.get_Instance().Unmarshall(context)));
					}
					else if (context.TestExpression("Tag", num))
					{
						list.Add(new AnalyticsTagPredicate(TagUnmarshaller.Instance.Unmarshall(context)));
					}
					else if (context.TestExpression("And", num))
					{
						list.Add(new AnalyticsAndOperator(Unmarshall(context)));
					}
				}
				else if (context.get_IsEndElement() && context.get_CurrentDepth() < currentDepth)
				{
					return list;
				}
			}
			return list;
		}

		public List<AnalyticsFilterPredicate> Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
