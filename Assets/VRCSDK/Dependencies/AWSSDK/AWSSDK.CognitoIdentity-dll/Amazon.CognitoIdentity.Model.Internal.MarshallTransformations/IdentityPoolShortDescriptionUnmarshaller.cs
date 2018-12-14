using Amazon.Runtime.Internal.Transform;
using System;
using ThirdParty.Json.LitJson;

namespace Amazon.CognitoIdentity.Model.Internal.MarshallTransformations
{
	public class IdentityPoolShortDescriptionUnmarshaller : IUnmarshaller<IdentityPoolShortDescription, XmlUnmarshallerContext>, IUnmarshaller<IdentityPoolShortDescription, JsonUnmarshallerContext>
	{
		private static IdentityPoolShortDescriptionUnmarshaller _instance = new IdentityPoolShortDescriptionUnmarshaller();

		public static IdentityPoolShortDescriptionUnmarshaller Instance => _instance;

		IdentityPoolShortDescription IUnmarshaller<IdentityPoolShortDescription, XmlUnmarshallerContext>.Unmarshall(XmlUnmarshallerContext context)
		{
			throw new NotImplementedException();
		}

		public IdentityPoolShortDescription Unmarshall(JsonUnmarshallerContext context)
		{
			context.Read();
			if (context.CurrentTokenType == JsonToken.Null)
			{
				return null;
			}
			IdentityPoolShortDescription identityPoolShortDescription = new IdentityPoolShortDescription();
			int currentDepth = context.CurrentDepth;
			while (context.ReadAtDepth(currentDepth))
			{
				if (context.TestExpression("IdentityPoolId", currentDepth))
				{
					StringUnmarshaller instance = StringUnmarshaller.Instance;
					identityPoolShortDescription.IdentityPoolId = instance.Unmarshall(context);
				}
				else if (context.TestExpression("IdentityPoolName", currentDepth))
				{
					StringUnmarshaller instance2 = StringUnmarshaller.Instance;
					identityPoolShortDescription.IdentityPoolName = instance2.Unmarshall(context);
				}
			}
			return identityPoolShortDescription;
		}
	}
}
