using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using System.Globalization;
using System.IO;
using System.Text;
using ThirdParty.Json.LitJson;

namespace Amazon.CognitoIdentity.Model.Internal.MarshallTransformations
{
	public class ListIdentitiesRequestMarshaller : IMarshaller<IRequest, ListIdentitiesRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((ListIdentitiesRequest)input);
		}

		public IRequest Marshall(ListIdentitiesRequest publicRequest)
		{
			IRequest request = new DefaultRequest(publicRequest, "Amazon.CognitoIdentity");
			string value = "AWSCognitoIdentityService.ListIdentities";
			request.Headers["X-Amz-Target"] = value;
			request.Headers["Content-Type"] = "application/x-amz-json-1.1";
			request.HttpMethod = "POST";
			string text2 = request.ResourcePath = "/";
			using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
			{
				JsonWriter jsonWriter = new JsonWriter(stringWriter);
				jsonWriter.WriteObjectStart();
				JsonMarshallerContext jsonMarshallerContext = new JsonMarshallerContext(request, jsonWriter);
				if (publicRequest.IsSetHideDisabled())
				{
					jsonMarshallerContext.Writer.WritePropertyName("HideDisabled");
					jsonMarshallerContext.Writer.Write(publicRequest.HideDisabled);
				}
				if (publicRequest.IsSetIdentityPoolId())
				{
					jsonMarshallerContext.Writer.WritePropertyName("IdentityPoolId");
					jsonMarshallerContext.Writer.Write(publicRequest.IdentityPoolId);
				}
				if (publicRequest.IsSetMaxResults())
				{
					jsonMarshallerContext.Writer.WritePropertyName("MaxResults");
					jsonMarshallerContext.Writer.Write(publicRequest.MaxResults);
				}
				if (publicRequest.IsSetNextToken())
				{
					jsonMarshallerContext.Writer.WritePropertyName("NextToken");
					jsonMarshallerContext.Writer.Write(publicRequest.NextToken);
				}
				jsonWriter.WriteObjectEnd();
				string s = stringWriter.ToString();
				request.Content = Encoding.UTF8.GetBytes(s);
				return request;
			}
		}
	}
}
