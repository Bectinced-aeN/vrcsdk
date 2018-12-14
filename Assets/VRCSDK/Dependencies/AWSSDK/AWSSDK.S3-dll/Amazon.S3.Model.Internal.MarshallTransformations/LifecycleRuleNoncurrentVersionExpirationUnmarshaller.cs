using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class LifecycleRuleNoncurrentVersionExpirationUnmarshaller : IUnmarshaller<LifecycleRuleNoncurrentVersionExpiration, XmlUnmarshallerContext>, IUnmarshaller<LifecycleRuleNoncurrentVersionExpiration, JsonUnmarshallerContext>
	{
		private static LifecycleRuleNoncurrentVersionExpirationUnmarshaller _instance;

		public static LifecycleRuleNoncurrentVersionExpirationUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new LifecycleRuleNoncurrentVersionExpirationUnmarshaller();
				}
				return _instance;
			}
		}

		public LifecycleRuleNoncurrentVersionExpiration Unmarshall(XmlUnmarshallerContext context)
		{
			LifecycleRuleNoncurrentVersionExpiration lifecycleRuleNoncurrentVersionExpiration = new LifecycleRuleNoncurrentVersionExpiration();
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
						lifecycleRuleNoncurrentVersionExpiration.NoncurrentDays = IntUnmarshaller.GetInstance().Unmarshall(context);
					}
				}
				else if (context.get_IsEndElement() && context.get_CurrentDepth() < currentDepth)
				{
					return lifecycleRuleNoncurrentVersionExpiration;
				}
			}
			return lifecycleRuleNoncurrentVersionExpiration;
		}

		public LifecycleRuleNoncurrentVersionExpiration Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
