using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using Amazon.Runtime.Internal.Util;

namespace Amazon.SecurityToken.Model.Internal.MarshallTransformations
{
	public class DecodeAuthorizationMessageRequestMarshaller : IMarshaller<IRequest, DecodeAuthorizationMessageRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((DecodeAuthorizationMessageRequest)input);
		}

		public IRequest Marshall(DecodeAuthorizationMessageRequest publicRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			IRequest val = new DefaultRequest(publicRequest, "Amazon.SecurityToken");
			val.get_Parameters().Add("Action", "DecodeAuthorizationMessage");
			val.get_Parameters().Add("Version", "2011-06-15");
			if (publicRequest != null && publicRequest.IsSetEncodedMessage())
			{
				val.get_Parameters().Add("EncodedMessage", StringUtils.FromString(publicRequest.EncodedMessage));
			}
			return val;
		}
	}
}
