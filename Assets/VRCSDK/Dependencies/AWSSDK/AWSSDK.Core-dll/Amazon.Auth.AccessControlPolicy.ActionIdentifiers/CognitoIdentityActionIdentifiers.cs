namespace Amazon.Auth.AccessControlPolicy.ActionIdentifiers
{
	public static class CognitoIdentityActionIdentifiers
	{
		public static readonly ActionIdentifier AllCognitoIdentityActions = new ActionIdentifier("cognito-identity:*");

		public static readonly ActionIdentifier CreateIdentityPool = new ActionIdentifier("cognito-identity:CreateIdentityPool");

		public static readonly ActionIdentifier DeleteIdentityPool = new ActionIdentifier("cognito-identity:DeleteIdentityPool");

		public static readonly ActionIdentifier DescribeIdentityPool = new ActionIdentifier("cognito-identity:DescribeIdentityPool");

		public static readonly ActionIdentifier ListIdentities = new ActionIdentifier("cognito-identity:ListIdentities");

		public static readonly ActionIdentifier ListIdentityPools = new ActionIdentifier("cognito-identity:ListIdentityPools");

		public static readonly ActionIdentifier UpdateIdentityPool = new ActionIdentifier("cognito-identity:UpdateIdentityPool");
	}
}
