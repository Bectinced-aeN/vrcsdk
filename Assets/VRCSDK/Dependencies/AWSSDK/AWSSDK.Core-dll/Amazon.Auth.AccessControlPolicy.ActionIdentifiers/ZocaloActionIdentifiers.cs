namespace Amazon.Auth.AccessControlPolicy.ActionIdentifiers
{
	public static class ZocaloActionIdentifiers
	{
		public static readonly ActionIdentifier AllZocaloActions = new ActionIdentifier("zocalo:*");

		public static readonly ActionIdentifier ActivateUser = new ActionIdentifier("zocalo:ActivateUser");

		public static readonly ActionIdentifier AddUserToGroup = new ActionIdentifier("zocalo:AddUserToGroup");

		public static readonly ActionIdentifier CheckAlias = new ActionIdentifier("zocalo:CheckAlias");

		public static readonly ActionIdentifier CreateInstance = new ActionIdentifier("zocalo:CreateInstance");

		public static readonly ActionIdentifier DeactivateUser = new ActionIdentifier("zocalo:DeactivateUser");

		public static readonly ActionIdentifier DeleteInstance = new ActionIdentifier("zocalo:DeleteInstance");

		public static readonly ActionIdentifier DeregisterDirectory = new ActionIdentifier("zocalo:DeregisterDirectory");

		public static readonly ActionIdentifier DescribeAvailableDirectories = new ActionIdentifier("zocalo:DescribeAvailableDirectories");

		public static readonly ActionIdentifier DescribeInstances = new ActionIdentifier("zocalo:DescribeInstances");

		public static readonly ActionIdentifier RegisterDirectory = new ActionIdentifier("zocalo:RegisterDirectory");

		public static readonly ActionIdentifier RemoveUserFromGroup = new ActionIdentifier("zocalo:RemoveUserFromGroup");

		public static readonly ActionIdentifier UpdateInstanceAlias = new ActionIdentifier("zocalo:UpdateInstanceAlias");
	}
}
