using Amazon.Runtime.Internal.Transform;

namespace Amazon.SecurityToken.Model.Internal.MarshallTransformations
{
	public class CredentialsUnmarshaller : IUnmarshaller<Credentials, XmlUnmarshallerContext>, IUnmarshaller<Credentials, JsonUnmarshallerContext>
	{
		private static CredentialsUnmarshaller _instance = new CredentialsUnmarshaller();

		public static CredentialsUnmarshaller Instance => _instance;

		public Credentials Unmarshall(XmlUnmarshallerContext context)
		{
			Credentials credentials = new Credentials();
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
					if (context.TestExpression("AccessKeyId", num))
					{
						StringUnmarshaller instance = StringUnmarshaller.get_Instance();
						credentials.AccessKeyId = instance.Unmarshall(context);
					}
					else if (context.TestExpression("Expiration", num))
					{
						DateTimeUnmarshaller instance2 = DateTimeUnmarshaller.get_Instance();
						credentials.Expiration = instance2.Unmarshall(context);
					}
					else if (context.TestExpression("SecretAccessKey", num))
					{
						StringUnmarshaller instance3 = StringUnmarshaller.get_Instance();
						credentials.SecretAccessKey = instance3.Unmarshall(context);
					}
					else if (context.TestExpression("SessionToken", num))
					{
						StringUnmarshaller instance4 = StringUnmarshaller.get_Instance();
						credentials.SessionToken = instance4.Unmarshall(context);
					}
				}
				else if (context.get_IsEndElement() && context.get_CurrentDepth() < currentDepth)
				{
					return credentials;
				}
			}
			return credentials;
		}

		public Credentials Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
