using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.SecurityToken.Model.Internal.MarshallTransformations
{
	public class GetCallerIdentityRequestMarshaller : IMarshaller<IRequest, GetCallerIdentityRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((GetCallerIdentityRequest)input);
		}

		public IRequest Marshall(GetCallerIdentityRequest publicRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Expected O, but got Unknown
			DefaultRequest val = new DefaultRequest(publicRequest, "Amazon.SecurityToken");
			val.get_Parameters().Add("Action", "GetCallerIdentity");
			val.get_Parameters().Add("Version", "2011-06-15");
			return val;
		}
	}
}
