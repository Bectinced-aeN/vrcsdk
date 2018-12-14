namespace Amazon.Auth.AccessControlPolicy.ActionIdentifiers
{
	public static class CloudFrontActionIdentifiers
	{
		public static readonly ActionIdentifier AllCloudFrontActions = new ActionIdentifier("cloudfront:*");

		public static readonly ActionIdentifier CreateCloudFrontOriginAccessIdentity = new ActionIdentifier("cloudfront:CreateCloudFrontOriginAccessIdentity");

		public static readonly ActionIdentifier CreateDistribution = new ActionIdentifier("cloudfront:CreateDistribution");

		public static readonly ActionIdentifier CreateInvalidation = new ActionIdentifier("cloudfront:CreateInvalidation");

		public static readonly ActionIdentifier CreateStreamingDistribution = new ActionIdentifier("cloudfront:CreateStreamingDistribution");

		public static readonly ActionIdentifier DeleteCloudFrontOriginAccessIdentity = new ActionIdentifier("cloudfront:DeleteCloudFrontOriginAccessIdentity");

		public static readonly ActionIdentifier DeleteDistribution = new ActionIdentifier("cloudfront:DeleteDistribution");

		public static readonly ActionIdentifier DeleteStreamingDistribution = new ActionIdentifier("cloudfront:DeleteStreamingDistribution");

		public static readonly ActionIdentifier GetCloudFrontOriginAccessIdentity = new ActionIdentifier("cloudfront:GetCloudFrontOriginAccessIdentity");

		public static readonly ActionIdentifier GetCloudFrontOriginAccessIdentityConfig = new ActionIdentifier("cloudfront:GetCloudFrontOriginAccessIdentityConfig");

		public static readonly ActionIdentifier GetDistribution = new ActionIdentifier("cloudfront:GetDistribution");

		public static readonly ActionIdentifier GetDistributionConfig = new ActionIdentifier("cloudfront:GetDistributionConfig");

		public static readonly ActionIdentifier GetInvalidation = new ActionIdentifier("cloudfront:GetInvalidation");

		public static readonly ActionIdentifier GetStreamingDistribution = new ActionIdentifier("cloudfront:GetStreamingDistribution");

		public static readonly ActionIdentifier GetStreamingDistributionConfig = new ActionIdentifier("cloudfront:GetStreamingDistributionConfig");

		public static readonly ActionIdentifier ListCloudFrontOriginAccessIdentities = new ActionIdentifier("cloudfront:ListCloudFrontOriginAccessIdentities");

		public static readonly ActionIdentifier ListDistributions = new ActionIdentifier("cloudfront:ListDistributions");

		public static readonly ActionIdentifier ListInvalidations = new ActionIdentifier("cloudfront:ListInvalidations");

		public static readonly ActionIdentifier ListStreamingDistributions = new ActionIdentifier("cloudfront:ListStreamingDistributions");

		public static readonly ActionIdentifier UpdateCloudFrontOriginAccessIdentity = new ActionIdentifier("cloudfront:UpdateCloudFrontOriginAccessIdentity");

		public static readonly ActionIdentifier UpdateDistribution = new ActionIdentifier("cloudfront:UpdateDistribution");

		public static readonly ActionIdentifier UpdateStreamingDistribution = new ActionIdentifier("cloudfront:UpdateStreamingDistribution");
	}
}
