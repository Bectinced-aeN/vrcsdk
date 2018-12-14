using Amazon.Runtime.Internal.Transform;

namespace Amazon.SecurityToken.Model.Internal.MarshallTransformations
{
	public class AssumedRoleUserUnmarshaller : IUnmarshaller<AssumedRoleUser, XmlUnmarshallerContext>, IUnmarshaller<AssumedRoleUser, JsonUnmarshallerContext>
	{
		private static AssumedRoleUserUnmarshaller _instance = new AssumedRoleUserUnmarshaller();

		public static AssumedRoleUserUnmarshaller Instance => _instance;

		public AssumedRoleUser Unmarshall(XmlUnmarshallerContext context)
		{
			AssumedRoleUser assumedRoleUser = new AssumedRoleUser();
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
						assumedRoleUser.Arn = instance.Unmarshall(context);
					}
					else if (context.TestExpression("AssumedRoleId", num))
					{
						StringUnmarshaller instance2 = StringUnmarshaller.get_Instance();
						assumedRoleUser.AssumedRoleId = instance2.Unmarshall(context);
					}
				}
				else if (context.get_IsEndElement() && context.get_CurrentDepth() < currentDepth)
				{
					return assumedRoleUser;
				}
			}
			return assumedRoleUser;
		}

		public AssumedRoleUser Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
