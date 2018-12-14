using Amazon.Runtime.Internal.Transform;
using System;
using ThirdParty.Json.LitJson;

namespace Amazon.CognitoIdentity.Model.Internal.MarshallTransformations
{
	public class CredentialsUnmarshaller : IUnmarshaller<Credentials, XmlUnmarshallerContext>, IUnmarshaller<Credentials, JsonUnmarshallerContext>
	{
		private static CredentialsUnmarshaller _instance = new CredentialsUnmarshaller();

		public static CredentialsUnmarshaller Instance => _instance;

		Credentials IUnmarshaller<Credentials, XmlUnmarshallerContext>.Unmarshall(XmlUnmarshallerContext context)
		{
			throw new NotImplementedException();
		}

		public Credentials Unmarshall(JsonUnmarshallerContext context)
		{
			context.Read();
			if (context.CurrentTokenType == JsonToken.Null)
			{
				return null;
			}
			Credentials credentials = new Credentials();
			int currentDepth = context.CurrentDepth;
			while (context.ReadAtDepth(currentDepth))
			{
				if (context.TestExpression("AccessKeyId", currentDepth))
				{
					StringUnmarshaller instance = StringUnmarshaller.Instance;
					credentials.AccessKeyId = instance.Unmarshall(context);
				}
				else if (context.TestExpression("Expiration", currentDepth))
				{
					DateTimeUnmarshaller instance2 = DateTimeUnmarshaller.Instance;
					credentials.Expiration = instance2.Unmarshall(context);
				}
				else if (context.TestExpression("SecretKey", currentDepth))
				{
					StringUnmarshaller instance3 = StringUnmarshaller.Instance;
					credentials.SecretKey = instance3.Unmarshall(context);
				}
				else if (context.TestExpression("SessionToken", currentDepth))
				{
					StringUnmarshaller instance4 = StringUnmarshaller.Instance;
					credentials.SessionToken = instance4.Unmarshall(context);
				}
			}
			return credentials;
		}
	}
}
