using Amazon.Runtime.Internal.Transform;

namespace Amazon.CognitoIdentity.Model.Internal.MarshallTransformations
{
	public class CognitoIdentityProviderInfoMarshaller : IRequestMarshaller<CognitoIdentityProviderInfo, JsonMarshallerContext>
	{
		public static readonly CognitoIdentityProviderInfoMarshaller Instance = new CognitoIdentityProviderInfoMarshaller();

		public void Marshall(CognitoIdentityProviderInfo requestObject, JsonMarshallerContext context)
		{
			if (requestObject.IsSetClientId())
			{
				context.Writer.WritePropertyName("ClientId");
				context.Writer.Write(requestObject.ClientId);
			}
			if (requestObject.IsSetProviderName())
			{
				context.Writer.WritePropertyName("ProviderName");
				context.Writer.Write(requestObject.ProviderName);
			}
		}
	}
}
