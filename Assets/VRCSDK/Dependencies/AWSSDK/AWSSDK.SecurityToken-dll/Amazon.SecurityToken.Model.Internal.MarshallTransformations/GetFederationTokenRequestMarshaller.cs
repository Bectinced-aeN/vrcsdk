using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using Amazon.Runtime.Internal.Util;

namespace Amazon.SecurityToken.Model.Internal.MarshallTransformations
{
	public class GetFederationTokenRequestMarshaller : IMarshaller<IRequest, GetFederationTokenRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((GetFederationTokenRequest)input);
		}

		public IRequest Marshall(GetFederationTokenRequest publicRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			IRequest val = new DefaultRequest(publicRequest, "Amazon.SecurityToken");
			val.get_Parameters().Add("Action", "GetFederationToken");
			val.get_Parameters().Add("Version", "2011-06-15");
			if (publicRequest != null)
			{
				if (publicRequest.IsSetDurationSeconds())
				{
					val.get_Parameters().Add("DurationSeconds", StringUtils.FromInt(publicRequest.DurationSeconds));
				}
				if (publicRequest.IsSetName())
				{
					val.get_Parameters().Add("Name", StringUtils.FromString(publicRequest.Name));
				}
				if (publicRequest.IsSetPolicy())
				{
					val.get_Parameters().Add("Policy", StringUtils.FromString(publicRequest.Policy));
				}
			}
			return val;
		}
	}
}
