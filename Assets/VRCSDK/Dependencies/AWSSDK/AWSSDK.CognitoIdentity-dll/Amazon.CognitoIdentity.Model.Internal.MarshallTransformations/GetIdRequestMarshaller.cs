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
	public class GetIdRequestMarshaller : IMarshaller<IRequest, GetIdRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((GetIdRequest)input);
		}

		public IRequest Marshall(GetIdRequest publicRequest)
		{
			IRequest request = new DefaultRequest(publicRequest, "Amazon.CognitoIdentity");
			string value = "AWSCognitoIdentityService.GetId";
			request.Headers["X-Amz-Target"] = value;
			request.Headers["Content-Type"] = "application/x-amz-json-1.1";
			request.HttpMethod = "POST";
			string text2 = request.ResourcePath = "/";
			using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
			{
				JsonWriter jsonWriter = new JsonWriter(stringWriter);
				jsonWriter.WriteObjectStart();
				JsonMarshallerContext jsonMarshallerContext = new JsonMarshallerContext(request, jsonWriter);
				if (publicRequest.IsSetAccountId())
				{
					jsonMarshallerContext.Writer.WritePropertyName("AccountId");
					jsonMarshallerContext.Writer.Write(publicRequest.AccountId);
				}
				if (publicRequest.IsSetIdentityPoolId())
				{
					jsonMarshallerContext.Writer.WritePropertyName("IdentityPoolId");
					jsonMarshallerContext.Writer.Write(publicRequest.IdentityPoolId);
				}
				if (publicRequest.IsSetLogins())
				{
					jsonMarshallerContext.Writer.WritePropertyName("Logins");
					jsonMarshallerContext.Writer.WriteObjectStart();
					foreach (KeyValuePair<string, string> login in publicRequest.Logins)
					{
						jsonMarshallerContext.Writer.WritePropertyName(login.Key);
						string value2 = login.Value;
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
