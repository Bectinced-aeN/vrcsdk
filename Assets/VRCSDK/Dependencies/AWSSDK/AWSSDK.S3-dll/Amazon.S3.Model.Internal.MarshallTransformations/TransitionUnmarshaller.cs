using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class TransitionUnmarshaller : IUnmarshaller<LifecycleTransition, XmlUnmarshallerContext>, IUnmarshaller<LifecycleTransition, JsonUnmarshallerContext>
	{
		private static TransitionUnmarshaller _instance;

		public static TransitionUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new TransitionUnmarshaller();
				}
				return _instance;
			}
		}

		public LifecycleTransition Unmarshall(XmlUnmarshallerContext context)
		{
			LifecycleTransition lifecycleTransition = new LifecycleTransition();
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
					if (context.TestExpression("Date", num))
					{
						lifecycleTransition.Date = DateTimeUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Days", num))
					{
						lifecycleTransition.Days = IntUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("StorageClass", num))
					{
						lifecycleTransition.StorageClass = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
				}
				else if (context.get_IsEndElement() && context.get_CurrentDepth() < currentDepth)
				{
					return lifecycleTransition;
				}
			}
			return lifecycleTransition;
		}

		public LifecycleTransition Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
