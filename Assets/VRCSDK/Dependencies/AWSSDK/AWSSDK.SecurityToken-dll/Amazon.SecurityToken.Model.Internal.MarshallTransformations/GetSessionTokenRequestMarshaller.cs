using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using Amazon.Runtime.Internal.Util;

namespace Amazon.SecurityToken.Model.Internal.MarshallTransformations
{
	public class GetSessionTokenRequestMarshaller : IMarshaller<IRequest, GetSessionTokenRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((GetSessionTokenRequest)input);
		}

		public IRequest Marshall(GetSessionTokenRequest publicRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			IRequest val = new DefaultRequest(publicRequest, "Amazon.SecurityToken");
			val.get_Parameters().Add("Action", "GetSessionToken");
			val.get_Parameters().Add("Version", "2011-06-15");
			if (publicRequest != null)
			{
				if (publicRequest.IsSetDurationSeconds())
				{
					val.get_Parameters().Add("DurationSeconds", StringUtils.FromInt(publicRequest.DurationSeconds));
				}
				if (publicRequest.IsSetSerialNumber())
				{
					val.get_Parameters().Add("SerialNumber", StringUtils.FromString(publicRequest.SerialNumber));
				}
				if (publicRequest.IsSetTokenCode())
				{
					val.get_Parameters().Add("TokenCode", StringUtils.FromString(publicRequest.TokenCode));
				}
			}
			return val;
		}
	}
}
