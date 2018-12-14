namespace Amazon.Auth.AccessControlPolicy.ActionIdentifiers
{
	public static class SNSActionIdentifiers
	{
		public static readonly ActionIdentifier AllSNSActions = new ActionIdentifier("sns:*");

		public static readonly ActionIdentifier AddPermission = new ActionIdentifier("sns:AddPermission");

		public static readonly ActionIdentifier ConfirmSubscription = new ActionIdentifier("sns:ConfirmSubscription");

		public static readonly ActionIdentifier CreatePlatformApplication = new ActionIdentifier("sns:CreatePlatformApplication");

		public static readonly ActionIdentifier CreatePlatformEndpoint = new ActionIdentifier("sns:CreatePlatformEndpoint");

		public static readonly ActionIdentifier CreateTopic = new ActionIdentifier("sns:CreateTopic");

		public static readonly ActionIdentifier DeleteEndpoint = new ActionIdentifier("sns:DeleteEndpoint");

		public static readonly ActionIdentifier DeletePlatformApplication = new ActionIdentifier("sns:DeletePlatformApplication");

		public static readonly ActionIdentifier DeleteTopic = new ActionIdentifier("sns:DeleteTopic");

		public static readonly ActionIdentifier GetEndpointAttributes = new ActionIdentifier("sns:GetEndpointAttributes");

		public static readonly ActionIdentifier GetPlatformApplicationAttributes = new ActionIdentifier("sns:GetPlatformApplicationAttributes");

		public static readonly ActionIdentifier GetSubscriptionAttributes = new ActionIdentifier("sns:GetSubscriptionAttributes");

		public static readonly ActionIdentifier GetTopicAttributes = new ActionIdentifier("sns:GetTopicAttributes");

		public static readonly ActionIdentifier ListEndpointsByPlatformApplication = new ActionIdentifier("sns:ListEndpointsByPlatformApplication");

		public static readonly ActionIdentifier ListPlatformApplications = new ActionIdentifier("sns:ListPlatformApplications");

		public static readonly ActionIdentifier ListSubscriptions = new ActionIdentifier("sns:ListSubscriptions");

		public static readonly ActionIdentifier ListSubscriptionsByTopic = new ActionIdentifier("sns:ListSubscriptionsByTopic");

		public static readonly ActionIdentifier ListTopics = new ActionIdentifier("sns:ListTopics");

		public static readonly ActionIdentifier Publish = new ActionIdentifier("sns:Publish");

		public static readonly ActionIdentifier RemovePermission = new ActionIdentifier("sns:RemovePermission");

		public static readonly ActionIdentifier SetEndpointAttributes = new ActionIdentifier("sns:SetEndpointAttributes");

		public static readonly ActionIdentifier SetPlatformApplicationAttributes = new ActionIdentifier("sns:SetPlatformApplicationAttributes");

		public static readonly ActionIdentifier SetSubscriptionAttributes = new ActionIdentifier("sns:SetSubscriptionAttributes");

		public static readonly ActionIdentifier SetTopicAttributes = new ActionIdentifier("sns:SetTopicAttributes");

		public static readonly ActionIdentifier Subscribe = new ActionIdentifier("sns:Subscribe");

		public static readonly ActionIdentifier Unsubscribe = new ActionIdentifier("sns:Unsubscribe");
	}
}
