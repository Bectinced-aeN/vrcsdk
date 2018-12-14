namespace Amazon.Auth.AccessControlPolicy.ActionIdentifiers
{
	public static class CloudTrailActionIdentifiers
	{
		public static readonly ActionIdentifier AllCloudTrailActions = new ActionIdentifier("cloudtrail:*");

		public static readonly ActionIdentifier CreateTrail = new ActionIdentifier("cloudtrail:CreateTrail");

		public static readonly ActionIdentifier DeleteTrail = new ActionIdentifier("cloudtrail:DeleteTrail");

		public static readonly ActionIdentifier DescribeTrails = new ActionIdentifier("cloudtrail:DescribeTrails");

		public static readonly ActionIdentifier GetTrailStatus = new ActionIdentifier("cloudtrail:GetTrailStatus");

		public static readonly ActionIdentifier StartLogging = new ActionIdentifier("cloudtrail:StartLogging");

		public static readonly ActionIdentifier StopLogging = new ActionIdentifier("cloudtrail:StopLogging");

		public static readonly ActionIdentifier UpdateTrail = new ActionIdentifier("cloudtrail:UpdateTrail");
	}
}
