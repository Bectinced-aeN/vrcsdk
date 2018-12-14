namespace Amazon.Auth.AccessControlPolicy.ActionIdentifiers
{
	public static class MarketplaceManagementPortalActionIdentifiers
	{
		public static readonly ActionIdentifier AllMarketplaceManagementPortalActions = new ActionIdentifier("aws-marketplace-management:*");

		public static readonly ActionIdentifier uploadFiles = new ActionIdentifier("aws-marketplace-management:uploadFiles");

		public static readonly ActionIdentifier viewMarketing = new ActionIdentifier("aws-marketplace-management:viewMarketing");

		public static readonly ActionIdentifier viewReports = new ActionIdentifier("aws-marketplace-management:viewReports");

		public static readonly ActionIdentifier viewSupport = new ActionIdentifier("aws-marketplace-management:viewSupport");
	}
}
