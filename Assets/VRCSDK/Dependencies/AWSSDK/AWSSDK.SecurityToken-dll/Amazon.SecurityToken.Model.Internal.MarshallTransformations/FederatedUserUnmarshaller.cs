using Amazon.Runtime.Internal.Transform;

namespace Amazon.SecurityToken.Model.Internal.MarshallTransformations
{
	public class FederatedUserUnmarshaller : IUnmarshaller<FederatedUser, XmlUnmarshallerContext>, IUnmarshaller<FederatedUser, JsonUnmarshallerContext>
	{
		private static FederatedUserUnmarshaller _instance = new FederatedUserUnmarshaller();

		public static FederatedUserUnmarshaller Instance => _instance;

		public FederatedUser Unmarshall(XmlUnmarshallerContext context)
		{
			FederatedUser federatedUser = new FederatedUser();
			int currentDepth = context.get_CurrentDepth();
			int num = currentDepth + 1;
			if (context.get_IsStartOfDocument())
			{
				num += 2;
			}
			while (context.ReadAtDepth(currentDepth))
			{
				if (context.get_IsStartElement() || context.get_IsAttribute())
				{
					if (context.TestExpression("Arn", num))
					{
						StringUnmarshaller instance = StringUnmarshaller.get_Instance();
						federatedUser.Arn = instance.Unmarshall(context);
					}
					else if (context.TestExpression("FederatedUserId", num))
					{
						StringUnmarshaller instance2 = StringUnmarshaller.get_Instance();
						federatedUser.FederatedUserId = instance2.Unmarshall(context);
					}
				}
				else if (context.get_IsEndElement() && context.get_CurrentDepth() < currentDepth)
				{
					return federatedUser;
				}
			}
			return federatedUser;
		}

		public FederatedUser Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
