using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using ThirdParty.Json.LitJson;

namespace Amazon.CognitoIdentity.Model.Internal.MarshallTransformations
{
	public class CreateIdentityPoolRequestMarshaller : IMarshaller<IRequest, CreateIdentityPoolRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((CreateIdentityPoolRequest)input);
		}

		public IRequest Marshall(CreateIdentityPoolRequest publicRequest)
		{
			IRequest request = new DefaultRequest(publicRequest, "Amazon.CognitoIdentity");
			string value = "AWSCognitoIdentityService.CreateIdentityPool";
			request.Headers["X-Amz-Target"] = value;
			request.Headers["Content-Type"] = "application/x-amz-json-1.1";
			request.HttpMethod = "POST";
			string text2 = request.ResourcePath = "/";
			using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
			{
				JsonWriter jsonWriter = new JsonWriter(stringWriter);
				jsonWriter.WriteObjectStart();
				JsonMarshallerContext jsonMarshallerContext = new JsonMarshallerContext(request, jsonWriter);
				if (publicRequest.IsSetAllowUnauthenticatedIdentities())
				{
					jsonMarshallerContext.Writer.WritePropertyName("AllowUnauthenticatedIdentities");
					jsonMarshallerContext.Writer.Write(publicRequest.AllowUnauthenticatedIdentities);
				}
				if (publicRequest.IsSetCognitoIdentityProviders())
				{
					jsonMarshallerContext.Writer.WritePropertyName("CognitoIdentityProviders");
					jsonMarshallerContext.Writer.WriteArrayStart();
					foreach (CognitoIdentityProviderInfo cognitoIdentityProvider in publicRequest.CognitoIdentityProviders)
					{
						jsonMarshallerContext.Writer.WriteObjectStart();
						CognitoIdentityProviderInfoMarshaller.Instance.Marshall(cognitoIdentityProvider, jsonMarshallerContext);
						jsonMarshallerContext.Writer.WriteObjectEnd();
					}
					jsonMarshallerContext.Writer.WriteArrayEnd();
				}
				if (publicRequest.IsSetDeveloperProviderName())
				{
					jsonMarshallerContext.Writer.WritePropertyName("DeveloperProviderName");
					jsonMarshallerContext.Writer.Write(publicRequest.DeveloperProviderName);
				}
				if (publicRequest.IsSetIdentityPoolName())
				{
					jsonMarshallerContext.Writer.WritePropertyName("IdentityPoolName");
					jsonMarshallerContext.Writer.Write(publicRequest.IdentityPoolName);
				}
				if (publicRequest.IsSetOpenIdConnectProviderARNs())
				{
					jsonMarshallerContext.Writer.WritePropertyName("OpenIdConnectProviderARNs");
					jsonMarshallerContext.Writer.WriteArrayStart();
					foreach (string openIdConnectProviderARN in publicRequest.OpenIdConnectProviderARNs)
					{
						jsonMarshallerContext.Writer.Write(openIdConnectProviderARN);
					}
					jsonMarshallerContext.Writer.WriteArrayEnd();
				}
				if (publicRequest.IsSetSamlProviderARNs())
				{
					jsonMarshallerContext.Writer.WritePropertyName("SamlProviderARNs");
					jsonMarshallerContext.Writer.WriteArrayStart();
					foreach (string samlProviderARN in publicRequest.SamlProviderARNs)
					{
						jsonMarshallerContext.Writer.Write(samlProviderARN);
					}
					jsonMarshallerContext.Writer.WriteArrayEnd();
				}
				if (publicRequest.IsSetSupportedLoginProviders())
				{
					jsonMarshallerContext.Writer.WritePropertyName("SupportedLoginProviders");
					jsonMarshallerContext.Writer.WriteObjectStart();
					foreach (KeyValuePair<string, string> supportedLoginProvider in publicRequest.SupportedLoginProviders)
					{
						jsonMarshallerContext.Writer.WritePropertyName(supportedLoginProvider.Key);
						string value2 = supportedLoginProvider.Value;
						jsonMarshallerContext.Writer.Write(value2);
					}
					jsonMarshallerContext.Writer.WriteObjectEnd();
				}
				jsonWriter.WriteObjectEnd();
				string s = stringWriter.ToString();
				request.Content = Encoding.UTF8.GetBytes(s);
				return request;
			}
		}
	}
}
