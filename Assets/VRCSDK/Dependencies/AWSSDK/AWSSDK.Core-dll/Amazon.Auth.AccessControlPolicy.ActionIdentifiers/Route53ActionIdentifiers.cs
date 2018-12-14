namespace Amazon.Auth.AccessControlPolicy.ActionIdentifiers
{
	public static class Route53ActionIdentifiers
	{
		public static readonly ActionIdentifier AllRoute53Actions = new ActionIdentifier("route53:*");

		public static readonly ActionIdentifier ChangeResourceRecordSets = new ActionIdentifier("route53:ChangeResourceRecordSets");

		public static readonly ActionIdentifier CreateHostedZone = new ActionIdentifier("route53:CreateHostedZone");

		public static readonly ActionIdentifier DeleteHostedZone = new ActionIdentifier("route53:DeleteHostedZone");

		public static readonly ActionIdentifier GetChange = new ActionIdentifier("route53:GetChange");

		public static readonly ActionIdentifier GetHostedZone = new ActionIdentifier("route53:GetHostedZone");

		public static readonly ActionIdentifier ListHostedZones = new ActionIdentifier("route53:ListHostedZones");

		public static readonly ActionIdentifier ListResourceRecordSets = new ActionIdentifier("route53:ListResourceRecordSets");
	}
}
