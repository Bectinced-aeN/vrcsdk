namespace Amazon.Auth.AccessControlPolicy.ActionIdentifiers
{
	public static class BillingActionIdentifiers
	{
		public static readonly ActionIdentifier AllBillingActions = new ActionIdentifier("aws-portal:*");

		public static readonly ActionIdentifier ModifyAccount = new ActionIdentifier("aws-portal:ModifyAccount");

		public static readonly ActionIdentifier ModifyBilling = new ActionIdentifier("aws-portal:ModifyBilling");

		public static readonly ActionIdentifier ModifyPaymentMethods = new ActionIdentifier("aws-portal:ModifyPaymentMethods");

		public static readonly ActionIdentifier ViewAccount = new ActionIdentifier("aws-portal:ViewAccount");

		public static readonly ActionIdentifier ViewBilling = new ActionIdentifier("aws-portal:ViewBilling");

		public static readonly ActionIdentifier ViewPaymentMethods = new ActionIdentifier("aws-portal:ViewPaymentMethods");

		public static readonly ActionIdentifier ViewUsage = new ActionIdentifier("aws-portal:ViewUsage");
	}
}
