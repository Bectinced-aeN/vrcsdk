namespace Amazon.Auth.AccessControlPolicy.ActionIdentifiers
{
	public static class MarketplaceActionIdentifiers
	{
		public static readonly ActionIdentifier AllMarketplaceActions = new ActionIdentifier("aws-marketplace:*");

		public static readonly ActionIdentifier Subscribe = new ActionIdentifier("aws-marketplace:Subscribe");

		public static readonly ActionIdentifier Unsubscribe = new ActionIdentifier("aws-marketplace:Unsubscribe");

		public static readonly ActionIdentifier ViewSubscriptions = new ActionIdentifier("aws-marketplace:ViewSubscriptions");
	}
}
