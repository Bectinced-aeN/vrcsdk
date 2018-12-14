namespace Amazon.Auth.AccessControlPolicy.ActionIdentifiers
{
	public static class SQSActionIdentifiers
	{
		public static readonly ActionIdentifier AllSQSActions = new ActionIdentifier("sqs:*");

		public static readonly ActionIdentifier AddPermission = new ActionIdentifier("sqs:AddPermission");

		public static readonly ActionIdentifier ChangeMessageVisibility = new ActionIdentifier("sqs:ChangeMessageVisibility");

		public static readonly ActionIdentifier CreateQueue = new ActionIdentifier("sqs:CreateQueue");

		public static readonly ActionIdentifier DeleteMessage = new ActionIdentifier("sqs:DeleteMessage");

		public static readonly ActionIdentifier DeleteQueue = new ActionIdentifier("sqs:DeleteQueue");

		public static readonly ActionIdentifier GetQueueAttributes = new ActionIdentifier("sqs:GetQueueAttributes");

		public static readonly ActionIdentifier GetQueueUrl = new ActionIdentifier("sqs:GetQueueUrl");

		public static readonly ActionIdentifier ListQueues = new ActionIdentifier("sqs:ListQueues");

		public static readonly ActionIdentifier ReceiveMessage = new ActionIdentifier("sqs:ReceiveMessage");

		public static readonly ActionIdentifier RemovePermission = new ActionIdentifier("sqs:RemovePermission");

		public static readonly ActionIdentifier SendMessage = new ActionIdentifier("sqs:SendMessage");

		public static readonly ActionIdentifier SetQueueAttributes = new ActionIdentifier("sqs:SetQueueAttributes");
	}
}
