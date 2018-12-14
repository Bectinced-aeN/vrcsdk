using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using System.Globalization;
using System.IO;
using System.Text;
using ThirdParty.Json.LitJson;

namespace Amazon.CognitoIdentity.Model.Internal.MarshallTransformations
{
	public class MergeDeveloperIdentitiesRequestMarshaller : IMarshaller<IRequest, MergeDeveloperIdentitiesRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((MergeDeveloperIdentitiesRequest)input);
		}

		public IRequest Marshall(MergeDeveloperIdentitiesRequest publicRequest)
		{
			IRequest request = new DefaultRequest(publicRequest, "Amazon.CognitoIdentity");
			string value = "AWSCognitoIdentityService.MergeDeveloperIdentities";
			request.Headers["X-Amz-Target"] = value;
			request.Headers["Content-Type"] = "application/x-amz-json-1.1";
			request.HttpMethod = "POST";
			string text2 = request.ResourcePath = "/";
			using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
			{
				JsonWriter jsonWriter = new JsonWriter(stringWriter);
				jsonWriter.WriteObjectStart();
				JsonMarshallerContext jsonMarshallerContext = new JsonMarshallerContext(request, jsonWriter);
				if (publicRequest.IsSetDestinationUserIdentifier())
				{
					jsonMarshallerContext.Writer.WritePropertyName("DestinationUserIdentifier");
					jsonMarshallerContext.Writer.Write(publicRequest.DestinationUserIdentifier);
				}
				if (publicRequest.IsSetDeveloperProviderName())
				{
					jsonMarshallerContext.Writer.WritePropertyName("DeveloperProviderName");
					jsonMarshallerContext.Writer.Write(publicRequest.DeveloperProviderName);
				}
				if (publicRequest.IsSetIdentityPoolId())
				{
					jsonMarshallerContext.Writer.WritePropertyName("IdentityPoolId");
					jsonMarshallerContext.Writer.Write(publicRequest.IdentityPoolId);
				}
				if (publicRequest.IsSetSourceUserIdentifier())
				{
					jsonMarshallerContext.Writer.WritePropertyName("SourceUserIdentifier");
					jsonMarshallerContext.Writer.Write(publicRequest.SourceUserIdentifier);
				}
				jsonWriter.WriteObjectEnd();
				string s = stringWriter.ToString();
				request.Content = Encoding.UTF8.GetBytes(s);
				return request;
			}
		}
	}
}
