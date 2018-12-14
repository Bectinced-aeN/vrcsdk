using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class LifecycleRuleNoncurrentVersionTransitionUnmarshaller : IUnmarshaller<LifecycleRuleNoncurrentVersionTransition, XmlUnmarshallerContext>, IUnmarshaller<LifecycleRuleNoncurrentVersionTransition, JsonUnmarshallerContext>
	{
		private static LifecycleRuleNoncurrentVersionTransitionUnmarshaller _instance;

		public static LifecycleRuleNoncurrentVersionTransitionUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new LifecycleRuleNoncurrentVersionTransitionUnmarshaller();
				}
				return _instance;
			}
		}

		public LifecycleRuleNoncurrentVersionTransition Unmarshall(XmlUnmarshallerContext context)
		{
			LifecycleRuleNoncurrentVersionTransition lifecycleRuleNoncurrentVersionTransition = new LifecycleRuleNoncurrentVersionTransition();
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
					if (context.TestExpression("NoncurrentDays", num))
					{
						lifecycleRuleNoncurrentVersionTransition.NoncurrentDays = IntUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("StorageClass", num))
					{
						lifecycleRuleNoncurrentVersionTransition.StorageClass = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
				}
				else if (context.get_IsEndElement() && context.get_CurrentDepth() < currentDepth)
				{
					return lifecycleRuleNoncurrentVersionTransition;
				}
			}
			return lifecycleRuleNoncurrentVersionTransition;
		}

		public LifecycleRuleNoncurrentVersionTransition Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
