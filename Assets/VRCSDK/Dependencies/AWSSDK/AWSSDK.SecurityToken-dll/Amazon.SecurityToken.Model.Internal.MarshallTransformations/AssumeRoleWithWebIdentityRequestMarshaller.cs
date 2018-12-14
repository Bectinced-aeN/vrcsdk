using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using Amazon.Runtime.Internal.Util;

namespace Amazon.SecurityToken.Model.Internal.MarshallTransformations
{
	public class AssumeRoleWithWebIdentityRequestMarshaller : IMarshaller<IRequest, AssumeRoleWithWebIdentityRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((AssumeRoleWithWebIdentityRequest)input);
		}

		public IRequest Marshall(AssumeRoleWithWebIdentityRequest publicRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			IRequest val = new DefaultRequest(publicRequest, "Amazon.SecurityToken");
			val.get_Parameters().Add("Action", "AssumeRoleWithWebIdentity");
			val.get_Parameters().Add("Version", "2011-06-15");
			if (publicRequest != null)
			{
				if (publicRequest.IsSetDurationSeconds())
				{
					val.get_Parameters().Add("DurationSeconds", StringUtils.FromInt(publicRequest.DurationSeconds));
				}
				if (publicRequest.IsSetPolicy())
				{
					val.get_Parameters().Add("Policy", StringUtils.FromString(publicRequest.Policy));
				}
				if (publicRequest.IsSetProviderId())
				{
					val.get_Parameters().Add("ProviderId", StringUtils.FromString(publicRequest.ProviderId));
				}
				if (publicRequest.IsSetRoleArn())
				{
					val.get_Parameters().Add("RoleArn", StringUtils.FromString(publicRequest.RoleArn));
				}
				if (publicRequest.IsSetRoleSessionName())
				{
					val.get_Parameters().Add("RoleSessionName", StringUtils.FromString(publicRequest.RoleSessionName));
				}
				if (publicRequest.IsSetWebIdentityToken())
				{
					val.get_Parameters().Add("WebIdentityToken", StringUtils.FromString(publicRequest.WebIdentityToken));
				}
			}
			return val;
		}
	}
}
