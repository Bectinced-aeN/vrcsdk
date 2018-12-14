namespace Amazon.Auth.AccessControlPolicy.ActionIdentifiers
{
	public static class SESActionIdentifiers
	{
		public static readonly ActionIdentifier AllSESActions = new ActionIdentifier("ses:*");

		public static readonly ActionIdentifier DeleteIdentity = new ActionIdentifier("ses:DeleteIdentity");

		public static readonly ActionIdentifier DeleteVerifiedEmailAddress = new ActionIdentifier("ses:DeleteVerifiedEmailAddress");

		public static readonly ActionIdentifier GetIdentityDkimAttributes = new ActionIdentifier("ses:GetIdentityDkimAttributes");

		public static readonly ActionIdentifier GetIdentityNotificationAttributes = new ActionIdentifier("ses:GetIdentityNotificationAttributes");

		public static readonly ActionIdentifier GetIdentityVerificationAttributes = new ActionIdentifier("ses:GetIdentityVerificationAttributes");

		public static readonly ActionIdentifier GetSendQuota = new ActionIdentifier("ses:GetSendQuota");

		public static readonly ActionIdentifier GetSendStatistics = new ActionIdentifier("ses:GetSendStatistics");

		public static readonly ActionIdentifier ListIdentities = new ActionIdentifier("ses:ListIdentities");

		public static readonly ActionIdentifier ListVerifiedEmailAddresses = new ActionIdentifier("ses:ListVerifiedEmailAddresses");

		public static readonly ActionIdentifier SendEmail = new ActionIdentifier("ses:SendEmail");

		public static readonly ActionIdentifier SendRawEmail = new ActionIdentifier("ses:SendRawEmail");

		public static readonly ActionIdentifier SetIdentityDkimEnabled = new ActionIdentifier("ses:SetIdentityDkimEnabled");

		public static readonly ActionIdentifier SetIdentityNotificationTopic = new ActionIdentifier("ses:SetIdentityNotificationTopic");

		public static readonly ActionIdentifier SetIdentityFeedbackForwardingEnabled = new ActionIdentifier("ses:SetIdentityFeedbackForwardingEnabled");

		public static readonly ActionIdentifier VerifyDomainDkim = new ActionIdentifier("ses:VerifyDomainDkim");

		public static readonly ActionIdentifier VerifyDomainIdentity = new ActionIdentifier("ses:VerifyDomainIdentity");

		public static readonly ActionIdentifier VerifyEmailAddress = new ActionIdentifier("ses:VerifyEmailAddress");

		public static readonly ActionIdentifier VerifyEmailIdentity = new ActionIdentifier("ses:VerifyEmailIdentity");
	}
}
