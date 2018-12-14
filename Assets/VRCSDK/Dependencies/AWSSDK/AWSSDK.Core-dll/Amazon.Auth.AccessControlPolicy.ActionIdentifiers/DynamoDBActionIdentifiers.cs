namespace Amazon.Auth.AccessControlPolicy.ActionIdentifiers
{
	public static class DynamoDBActionIdentifiers
	{
		public static readonly ActionIdentifier AllDynamoDBActions = new ActionIdentifier("dynamodb:*");

		public static readonly ActionIdentifier BatchGetItem = new ActionIdentifier("dynamodb:BatchGetItem");

		public static readonly ActionIdentifier BatchWriteItem = new ActionIdentifier("dynamodb:BatchWriteItem");

		public static readonly ActionIdentifier CreateTable = new ActionIdentifier("dynamodb:CreateTable");

		public static readonly ActionIdentifier DeleteItem = new ActionIdentifier("dynamodb:DeleteItem");

		public static readonly ActionIdentifier DeleteTable = new ActionIdentifier("dynamodb:DeleteTable");

		public static readonly ActionIdentifier DescribeTable = new ActionIdentifier("dynamodb:DescribeTable");

		public static readonly ActionIdentifier GetItem = new ActionIdentifier("dynamodb:GetItem");

		public static readonly ActionIdentifier ListTables = new ActionIdentifier("dynamodb:ListTables");

		public static readonly ActionIdentifier PutItem = new ActionIdentifier("dynamodb:PutItem");

		public static readonly ActionIdentifier Query = new ActionIdentifier("dynamodb:Query");

		public static readonly ActionIdentifier Scan = new ActionIdentifier("dynamodb:Scan");

		public static readonly ActionIdentifier UpdateItem = new ActionIdentifier("dynamodb:UpdateItem");

		public static readonly ActionIdentifier UpdateTable = new ActionIdentifier("dynamodb:UpdateTable");
	}
}
