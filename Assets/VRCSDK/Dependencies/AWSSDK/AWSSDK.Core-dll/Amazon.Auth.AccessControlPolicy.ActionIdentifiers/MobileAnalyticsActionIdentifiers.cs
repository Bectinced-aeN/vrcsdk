namespace Amazon.Auth.AccessControlPolicy.ActionIdentifiers
{
	public static class MobileAnalyticsActionIdentifiers
	{
		public static readonly ActionIdentifier AllMobileAnalyticsActions = new ActionIdentifier("mobileanalytics:*");

		public static readonly ActionIdentifier PutEvents = new ActionIdentifier("mobileanalytics:PutEvents");

		public static readonly ActionIdentifier GetReports = new ActionIdentifier("mobileanalytics:GetReports");

		public static readonly ActionIdentifier GetFinancialReports = new ActionIdentifier("mobileanalytics:GetFinancialReports");
	}
}
