using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using System.Globalization;
using System.IO;
using System.Text;
using ThirdParty.Json.LitJson;

namespace Amazon.CognitoIdentity.Model.Internal.MarshallTransformations
{
	public class LookupDeveloperIdentityRequestMarshaller : IMarshaller<IRequest, LookupDeveloperIdentityRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((LookupDeveloperIdentityRequest)input);
		}

		public IRequest Marshall(LookupDeveloperIdentityRequest publicRequest)
		{
			IRequest request = new DefaultRequest(publicRequest, "Amazon.CognitoIdentity");
			string value = "AWSCognitoIdentityService.LookupDeveloperIdentity";
			request.Headers["X-Amz-Target"] = value;
			request.Headers["Content-Type"] = "application/x-amz-json-1.1";
			request.HttpMethod = "POST";
			string text2 = request.ResourcePath = "/";
			using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
			{
				JsonWriter jsonWriter = new JsonWriter(stringWriter);
				jsonWriter.WriteObjectStart();
				JsonMarshallerContext jsonMarshallerContext = new JsonMarshallerContext(request, jsonWriter);
				if (publicRequest.IsSetDeveloperUserIdentifier())
				{
					jsonMarshallerContext.Writer.WritePropertyName("DeveloperUserIdentifier");
					jsonMarshallerContext.Writer.Write(publicRequest.DeveloperUserIdentifier);
				}
				if (publicRequest.IsSetIdentityId())
				{
					jsonMarshallerContext.Writer.WritePropertyName("IdentityId");
					jsonMarshallerContext.Writer.Write(publicRequest.IdentityId);
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
