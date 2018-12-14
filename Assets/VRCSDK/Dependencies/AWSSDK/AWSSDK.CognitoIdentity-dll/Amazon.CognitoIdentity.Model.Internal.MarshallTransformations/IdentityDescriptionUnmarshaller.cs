using Amazon.Runtime.Internal.Transform;
using System;
using ThirdParty.Json.LitJson;

namespace Amazon.CognitoIdentity.Model.Internal.MarshallTransformations
{
	public class IdentityDescriptionUnmarshaller : IUnmarshaller<IdentityDescription, XmlUnmarshallerContext>, IUnmarshaller<IdentityDescription, JsonUnmarshallerContext>
	{
		private static IdentityDescriptionUnmarshaller _instance = new IdentityDescriptionUnmarshaller();

		public static IdentityDescriptionUnmarshaller Instance => _instance;

		IdentityDescription IUnmarshaller<IdentityDescription, XmlUnmarshallerContext>.Unmarshall(XmlUnmarshallerContext context)
		{
			throw new NotImplementedException();
		}

		public IdentityDescription Unmarshall(JsonUnmarshallerContext context)
		{
			context.Read();
			if (context.CurrentTokenType == JsonToken.Null)
			{
				return null;
			}
			IdentityDescription identityDescription = new IdentityDescription();
			int currentDepth = context.CurrentDepth;
			while (context.ReadAtDepth(currentDepth))
			{
				if (context.TestExpression("CreationDate", currentDepth))
				{
					DateTimeUnmarshaller instance = DateTimeUnmarshaller.Instance;
					identityDescription.CreationDate = instance.Unmarshall(context);
				}
				else if (context.TestExpression("IdentityId", currentDepth))
				{
					StringUnmarshaller instance2 = StringUnmarshaller.Instance;
					identityDescription.IdentityId = instance2.Unmarshall(context);
				}
				else if (context.TestExpression("LastModifiedDate", currentDepth))
				{
					DateTimeUnmarshaller instance3 = DateTimeUnmarshaller.Instance;
					identityDescription.LastModifiedDate = instance3.Unmarshall(context);
				}
				else if (context.TestExpression("Logins", currentDepth))
				{
					ListUnmarshaller<string, StringUnmarshaller> listUnmarshaller = new ListUnmarshaller<string, StringUnmarshaller>(StringUnmarshaller.Instance);
					identityDescription.Logins = listUnmarshaller.Unmarshall(context);
				}
			}
			return identityDescription;
		}
	}
}
