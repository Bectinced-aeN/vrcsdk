namespace Amazon.Auth.AccessControlPolicy.ActionIdentifiers
{
	public static class CognitoSyncActionIdentifiers
	{
		public static readonly ActionIdentifier AllCognitoSyncActions = new ActionIdentifier("cognito-sync:*");

		public static readonly ActionIdentifier DeleteDataset = new ActionIdentifier("cognito-sync:DeleteDataset");

		public static readonly ActionIdentifier DescribeDataset = new ActionIdentifier("cognito-sync:DescribeDataset");

		public static readonly ActionIdentifier DescribeIdentityUsage = new ActionIdentifier("cognito-sync:DescribeIdentityUsage");

		public static readonly ActionIdentifier DescribeIdentityPoolUsage = new ActionIdentifier("cognito-sync:DescribeIdentityPoolUsage");

		public static readonly ActionIdentifier ListDatasets = new ActionIdentifier("cognito-sync:ListDatasets");

		public static readonly ActionIdentifier ListIdentityPoolUsage = new ActionIdentifier("cognito-sync:ListIdentityPoolUsage");

		public static readonly ActionIdentifier ListRecords = new ActionIdentifier("cognito-sync:ListRecords");

		public static readonly ActionIdentifier UpdateRecords = new ActionIdentifier("cognito-sync:UpdateRecords");
	}
}
