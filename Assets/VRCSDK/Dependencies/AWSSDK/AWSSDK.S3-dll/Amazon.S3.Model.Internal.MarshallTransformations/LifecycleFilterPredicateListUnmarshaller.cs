using Amazon.Runtime.Internal.Transform;
using System.Collections.Generic;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class LifecycleFilterPredicateListUnmarshaller : IUnmarshaller<List<LifecycleFilterPredicate>, XmlUnmarshallerContext>, IUnmarshaller<List<LifecycleFilterPredicate>, JsonUnmarshallerContext>
	{
		private static LifecycleFilterPredicateListUnmarshaller _instance;

		public static LifecycleFilterPredicateListUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new LifecycleFilterPredicateListUnmarshaller();
				}
				return _instance;
			}
		}

		public List<LifecycleFilterPredicate> Unmarshall(XmlUnmarshallerContext context)
		{
			List<LifecycleFilterPredicate> list = new List<LifecycleFilterPredicate>();
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
						string prefix = StringUnmarshaller.get_Instance().Unmarshall(context);
						list.Add(new LifecyclePrefixPredicate
						{
							Prefix = prefix
						});
					}
					if (context.TestExpression("Tag", num))
					{
						Tag tag = TagUnmarshaller.Instance.Unmarshall(context);
						list.Add(new LifecycleTagPredicate
						{
							Tag = tag
						});
					}
					if (context.TestExpression("And", num))
					{
						list.Add(new LifecycleAndOperator
						{
							Operands = Unmarshall(context)
						});
					}
				}
				else if (context.get_IsEndElement() && context.get_CurrentDepth() < currentDepth)
				{
					return list;
				}
			}
			return list;
		}

		public List<LifecycleFilterPredicate> Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
