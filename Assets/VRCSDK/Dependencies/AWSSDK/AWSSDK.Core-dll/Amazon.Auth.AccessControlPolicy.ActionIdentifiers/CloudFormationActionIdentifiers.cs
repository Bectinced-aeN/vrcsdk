namespace Amazon.Auth.AccessControlPolicy.ActionIdentifiers
{
	public static class CloudFormationActionIdentifiers
	{
		public static readonly ActionIdentifier AllCloudFormationActions = new ActionIdentifier("cloudformation:*");

		public static readonly ActionIdentifier CreateStack = new ActionIdentifier("cloudformation:CreateStack");

		public static readonly ActionIdentifier DeleteStack = new ActionIdentifier("cloudformation:DeleteStack");

		public static readonly ActionIdentifier DescribeStackEvents = new ActionIdentifier("cloudformation:DescribeStackEvents");

		public static readonly ActionIdentifier DescribeStackResource = new ActionIdentifier("cloudformation:DescribeStackResource");

		public static readonly ActionIdentifier DescribeStackResources = new ActionIdentifier("cloudformation:DescribeStackResources");

		public static readonly ActionIdentifier DescribeStacks = new ActionIdentifier("cloudformation:DescribeStacks");

		public static readonly ActionIdentifier EstimateTemplateCost = new ActionIdentifier("cloudformation:EstimateTemplateCost");

		public static readonly ActionIdentifier GetTemplate = new ActionIdentifier("cloudformation:GetTemplate");

		public static readonly ActionIdentifier ListStacks = new ActionIdentifier("cloudformation:ListStacks");

		public static readonly ActionIdentifier ListStackResources = new ActionIdentifier("cloudformation:ListStackResources");

		public static readonly ActionIdentifier UpdateStack = new ActionIdentifier("cloudformation:UpdateStack");

		public static readonly ActionIdentifier ValidateTemplate = new ActionIdentifier("cloudformation:ValidateTemplate");
	}
}
