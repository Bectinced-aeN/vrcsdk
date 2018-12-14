using Amazon.Runtime;
using Amazon.Util.Internal;

namespace Amazon.SecurityToken
{
	public class AmazonSecurityTokenServiceConfig : ClientConfig
	{
		private static readonly string UserAgentString = InternalSDKUtils.BuildUserAgentString("3.3.0.3");

		private string _userAgent = UserAgentString;

		public override string RegionEndpointServiceName => "sts";

		public override string ServiceVersion => "2011-06-15";

		public override string UserAgent => _userAgent;

		public AmazonSecurityTokenServiceConfig()
			: this()
		{
			this.set_AuthenticationServiceName("sts");
			RegionEndpoint regionEndpoint = FallbackRegionFactory.GetRegionEndpoint(false);
			this.set_RegionEndpoint(regionEndpoint ?? RegionEndpoint.USEast1);
		}
	}
}
