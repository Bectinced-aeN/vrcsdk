namespace Amazon.Auth.AccessControlPolicy.ActionIdentifiers
{
	public static class ElasticMapReduceActionIdentifiers
	{
		public static readonly ActionIdentifier AllElasticMapReduceActions = new ActionIdentifier("elasticmapreduce:*");

		public static readonly ActionIdentifier AddInstanceGroups = new ActionIdentifier("elasticmapreduce:AddInstanceGroups");

		public static readonly ActionIdentifier AddTags = new ActionIdentifier("elasticmapreduce:AddTags");

		public static readonly ActionIdentifier AddJobFlowSteps = new ActionIdentifier("elasticmapreduce:AddJobFlowSteps");

		public static readonly ActionIdentifier DescribeCluster = new ActionIdentifier("elasticmapreduce:DescribeCluster");

		public static readonly ActionIdentifier DescribeJobFlows = new ActionIdentifier("elasticmapreduce:DescribeJobFlows");

		public static readonly ActionIdentifier DescribeStep = new ActionIdentifier("elasticmapreduce:DescribeStep");

		public static readonly ActionIdentifier ListBootstrapActions = new ActionIdentifier("elasticmapreduce:ListBootstrapActions");

		public static readonly ActionIdentifier ListClusters = new ActionIdentifier("elasticmapreduce:ListClusters");

		public static readonly ActionIdentifier ListInstanceGroups = new ActionIdentifier("elasticmapreduce:ListInstanceGroups");

		public static readonly ActionIdentifier ListInstances = new ActionIdentifier("elasticmapreduce:ListInstances");

		public static readonly ActionIdentifier ListSteps = new ActionIdentifier("elasticmapreduce:ListSteps");

		public static readonly ActionIdentifier ModifyInstanceGroups = new ActionIdentifier("elasticmapreduce:ModifyInstanceGroups");

		public static readonly ActionIdentifier RemoveTags = new ActionIdentifier("elasticmapreduce:RemoveTags");

		public static readonly ActionIdentifier RunJobFlow = new ActionIdentifier("elasticmapreduce:RunJobFlow");

		public static readonly ActionIdentifier SetTerminationProtection = new ActionIdentifier("elasticmapreduce:SetTerminationProtection");

		public static readonly ActionIdentifier TerminateJobFlows = new ActionIdentifier("elasticmapreduce:TerminateJobFlows");
	}
}
