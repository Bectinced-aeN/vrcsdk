using Amazon.Runtime;
using Amazon.Util.Internal;

namespace Amazon.CognitoIdentity
{
	public class AmazonCognitoIdentityConfig : ClientConfig
	{
		private static readonly string UserAgentString = InternalSDKUtils.BuildUserAgentString("3.3.0.3");

		private string _userAgent = UserAgentString;

		public override string RegionEndpointServiceName => "cognito-identity";

		public override string ServiceVersion => "2014-06-30";

		public override string UserAgent => _userAgent;

		public AmazonCognitoIdentityConfig()
		{
			base.AuthenticationServiceName = "cognito-identity";
		}
	}
}
