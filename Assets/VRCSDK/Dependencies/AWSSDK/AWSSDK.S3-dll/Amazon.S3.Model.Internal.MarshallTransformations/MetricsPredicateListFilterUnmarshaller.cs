using Amazon.Runtime.Internal.Transform;
using System.Collections.Generic;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class MetricsPredicateListFilterUnmarshaller : IUnmarshaller<List<MetricsFilterPredicate>, XmlUnmarshallerContext>, IUnmarshaller<List<MetricsFilterPredicate>, JsonUnmarshallerContext>
	{
		private static MetricsPredicateListFilterUnmarshaller _instance;

		public static MetricsPredicateListFilterUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new MetricsPredicateListFilterUnmarshaller();
				}
				return _instance;
			}
		}

		public List<MetricsFilterPredicate> Unmarshall(XmlUnmarshallerContext context)
		{
			List<MetricsFilterPredicate> list = new List<MetricsFilterPredicate>();
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
						list.Add(new MetricsPrefixPredicate(StringUnmarshaller.get_Instance().Unmarshall(context)));
					}
					else if (context.TestExpression("Tag", num))
					{
						list.Add(new MetricsTagPredicate(TagUnmarshaller.Instance.Unmarshall(context)));
					}
					else if (context.TestExpression("And", num))
					{
						list.Add(new MetricsAndOperator(Unmarshall(context)));
					}
				}
				else if (context.get_IsEndElement() && context.get_CurrentDepth() < currentDepth)
				{
					return list;
				}
			}
			return list;
		}

		public List<MetricsFilterPredicate> Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
