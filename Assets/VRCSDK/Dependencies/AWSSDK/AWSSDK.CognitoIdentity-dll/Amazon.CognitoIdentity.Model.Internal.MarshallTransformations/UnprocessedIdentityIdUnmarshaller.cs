using Amazon.Runtime.Internal.Transform;
using System;
using ThirdParty.Json.LitJson;

namespace Amazon.CognitoIdentity.Model.Internal.MarshallTransformations
{
	public class UnprocessedIdentityIdUnmarshaller : IUnmarshaller<UnprocessedIdentityId, XmlUnmarshallerContext>, IUnmarshaller<UnprocessedIdentityId, JsonUnmarshallerContext>
	{
		private static UnprocessedIdentityIdUnmarshaller _instance = new UnprocessedIdentityIdUnmarshaller();

		public static UnprocessedIdentityIdUnmarshaller Instance => _instance;

		UnprocessedIdentityId IUnmarshaller<UnprocessedIdentityId, XmlUnmarshallerContext>.Unmarshall(XmlUnmarshallerContext context)
		{
			throw new NotImplementedException();
		}

		public UnprocessedIdentityId Unmarshall(JsonUnmarshallerContext context)
		{
			context.Read();
			if (context.CurrentTokenType == JsonToken.Null)
			{
				return null;
			}
			UnprocessedIdentityId unprocessedIdentityId = new UnprocessedIdentityId();
			int currentDepth = context.CurrentDepth;
			while (context.ReadAtDepth(currentDepth))
			{
				if (context.TestExpression("ErrorCode", currentDepth))
				{
					StringUnmarshaller instance = StringUnmarshaller.Instance;
					unprocessedIdentityId.ErrorCode = instance.Unmarshall(context);
				}
				else if (context.TestExpression("IdentityId", currentDepth))
				{
					StringUnmarshaller instance2 = StringUnmarshaller.Instance;
					unprocessedIdentityId.IdentityId = instance2.Unmarshall(context);
				}
			}
			return unprocessedIdentityId;
		}
	}
}
