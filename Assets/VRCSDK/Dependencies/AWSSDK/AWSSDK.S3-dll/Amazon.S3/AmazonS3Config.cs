using Amazon.Runtime;
using Amazon.Util.Internal;
using System;

namespace Amazon.S3
{
	public class AmazonS3Config : ClientConfig
	{
		private const string _accelerateEndpoint = "s3-accelerate.amazonaws.com";

		private const string _accelerateDualstackEndpoint = "s3-accelerate.dualstack.amazonaws.com";

		private bool forcePathStyle;

		private bool useAccelerateEndpoint;

		private static readonly string UserAgentString = InternalSDKUtils.BuildUserAgentString("3.3.4.0");

		private string _userAgent = UserAgentString;

		public bool ForcePathStyle
		{
			get
			{
				return forcePathStyle;
			}
			set
			{
				forcePathStyle = value;
			}
		}

		public bool UseAccelerateEndpoint
		{
			get
			{
				return useAccelerateEndpoint;
			}
			set
			{
				useAccelerateEndpoint = value;
			}
		}

		internal string AccelerateEndpoint
		{
			get
			{
				if (!this.get_UseDualstackEndpoint())
				{
					return "s3-accelerate.amazonaws.com";
				}
				return "s3-accelerate.dualstack.amazonaws.com";
			}
		}

		public override string RegionEndpointServiceName => "s3";

		public override string ServiceVersion => "2006-03-01";

		public override string UserAgent => _userAgent;

		public override void Validate()
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			this.Validate();
			if (ForcePathStyle && UseAccelerateEndpoint)
			{
				throw new AmazonClientException("S3 accelerate is not compatible with Path style requests. Disable Path style requests using AmazonS3Config.ForcePathStyle property to use S3 accelerate.");
			}
			if (!string.IsNullOrEmpty(this.get_ServiceURL()) && (this.get_ServiceURL().IndexOf("s3-accelerate.amazonaws.com", StringComparison.OrdinalIgnoreCase) >= 0 || this.get_ServiceURL().IndexOf("s3-accelerate.dualstack.amazonaws.com", StringComparison.OrdinalIgnoreCase) >= 0))
			{
				if (this.get_RegionEndpoint() == null && string.IsNullOrEmpty(this.get_AuthenticationRegion()))
				{
					throw new AmazonClientException("Specify a region using AmazonS3Config.RegionEndpoint or AmazonS3Config.AuthenticationRegion to use S3 accelerate.");
				}
				if (this.get_RegionEndpoint() == null && !string.IsNullOrEmpty(this.get_AuthenticationRegion()))
				{
					this.set_RegionEndpoint(RegionEndpoint.GetBySystemName(this.get_AuthenticationRegion()));
				}
				UseAccelerateEndpoint = true;
			}
		}

		protected override void Initialize()
		{
			this.set_AllowAutoRedirect(false);
		}

		public AmazonS3Config()
			: this()
		{
			this.set_AuthenticationServiceName("s3");
		}
	}
}
