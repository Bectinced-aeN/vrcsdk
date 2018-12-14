namespace Amazon.Auth.AccessControlPolicy.ActionIdentifiers
{
	public static class SecurityTokenServiceActionIdentifiers
	{
		public static readonly ActionIdentifier AllSecurityTokenServiceActions = new ActionIdentifier("sts:*");

		public static readonly ActionIdentifier GetFederationToken = new ActionIdentifier("sts:GetFederationToken");

		public static readonly ActionIdentifier AssumeRole = new ActionIdentifier("sts:AssumeRole");
	}
}
