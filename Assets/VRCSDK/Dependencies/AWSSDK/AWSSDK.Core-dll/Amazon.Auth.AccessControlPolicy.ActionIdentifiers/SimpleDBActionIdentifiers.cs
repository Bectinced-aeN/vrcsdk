namespace Amazon.Auth.AccessControlPolicy.ActionIdentifiers
{
	public static class SimpleDBActionIdentifiers
	{
		public static readonly ActionIdentifier AllSimpleDBActions = new ActionIdentifier("sdb:*");

		public static readonly ActionIdentifier BatchDeleteAttributes = new ActionIdentifier("sdb:BatchDeleteAttributes");

		public static readonly ActionIdentifier BatchPutAttributes = new ActionIdentifier("sdb:BatchPutAttributes");

		public static readonly ActionIdentifier CreateDomain = new ActionIdentifier("sdb:CreateDomain");

		public static readonly ActionIdentifier DeleteAttributes = new ActionIdentifier("sdb:DeleteAttributes");

		public static readonly ActionIdentifier DeleteDomain = new ActionIdentifier("sdb:DeleteDomain");

		public static readonly ActionIdentifier DomainMetadata = new ActionIdentifier("sdb:DomainMetadata");

		public static readonly ActionIdentifier GetAttributes = new ActionIdentifier("sdb:GetAttributes");

		public static readonly ActionIdentifier ListDomains = new ActionIdentifier("sdb:ListDomains");

		public static readonly ActionIdentifier PutAttributes = new ActionIdentifier("sdb:PutAttributes");

		public static readonly ActionIdentifier Select = new ActionIdentifier("sdb:Select");
	}
}
