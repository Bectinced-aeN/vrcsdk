namespace Amazon.Auth.AccessControlPolicy.ActionIdentifiers
{
	public static class RedshiftActionIdentifiers
	{
		public static readonly ActionIdentifier AllRedshiftActions = new ActionIdentifier("redshift:*");

		public static readonly ActionIdentifier AuthorizeClusterSecurityGroupIngress = new ActionIdentifier("redshift:AuthorizeClusterSecurityGroupIngress");

		public static readonly ActionIdentifier AuthorizeSnapshotAccess = new ActionIdentifier("redshift:AuthorizeSnapshotAccess");

		public static readonly ActionIdentifier CopyClusterSnapshot = new ActionIdentifier("redshift:CopyClusterSnapshot");

		public static readonly ActionIdentifier CreateCluster = new ActionIdentifier("redshift:CreateCluster");

		public static readonly ActionIdentifier CreateClusterParameterGroup = new ActionIdentifier("redshift:CreateClusterParameterGroup");

		public static readonly ActionIdentifier CreateClusterSecurityGroup = new ActionIdentifier("redshift:CreateClusterSecurityGroup");

		public static readonly ActionIdentifier CreateClusterSnapshot = new ActionIdentifier("redshift:CreateClusterSnapshot");

		public static readonly ActionIdentifier CreateClusterSubnetGroup = new ActionIdentifier("redshift:CreateClusterSubnetGroup");

		public static readonly ActionIdentifier CreateEventSubscription = new ActionIdentifier("redshift:CreateEventSubscription");

		public static readonly ActionIdentifier CreateHsmClientCertificate = new ActionIdentifier("redshift:CreateHsmClientCertificate");

		public static readonly ActionIdentifier CreateHsmConfiguration = new ActionIdentifier("redshift:CreateHsmConfiguration");

		public static readonly ActionIdentifier DeleteCluster = new ActionIdentifier("redshift:DeleteCluster");

		public static readonly ActionIdentifier DeleteClusterParameterGroup = new ActionIdentifier("redshift:DeleteClusterParameterGroup");

		public static readonly ActionIdentifier DeleteClusterSecurityGroup = new ActionIdentifier("redshift:DeleteClusterSecurityGroup");

		public static readonly ActionIdentifier DeleteClusterSnapshot = new ActionIdentifier("redshift:DeleteClusterSnapshot");

		public static readonly ActionIdentifier DeleteClusterSubnetGroup = new ActionIdentifier("redshift:DeleteClusterSubnetGroup");

		public static readonly ActionIdentifier DeleteEventSubscription = new ActionIdentifier("redshift:DeleteEventSubscription");

		public static readonly ActionIdentifier DeleteHsmClientCertificate = new ActionIdentifier("redshift:DeleteHsmClientCertificate");

		public static readonly ActionIdentifier DeleteHsmConfiguration = new ActionIdentifier("redshift:DeleteHsmConfiguration");

		public static readonly ActionIdentifier DescribeClusterParameterGroups = new ActionIdentifier("redshift:DescribeClusterParameterGroups");

		public static readonly ActionIdentifier DescribeClusterParameters = new ActionIdentifier("redshift:DescribeClusterParameters");

		public static readonly ActionIdentifier DescribeClusterSecurityGroups = new ActionIdentifier("redshift:DescribeClusterSecurityGroups");

		public static readonly ActionIdentifier DescribeClusterSnapshots = new ActionIdentifier("redshift:DescribeClusterSnapshots");

		public static readonly ActionIdentifier DescribeClusterSubnetGroups = new ActionIdentifier("redshift:DescribeClusterSubnetGroups");

		public static readonly ActionIdentifier DescribeClusterVersions = new ActionIdentifier("redshift:DescribeClusterVersions");

		public static readonly ActionIdentifier DescribeClusters = new ActionIdentifier("redshift:DescribeClusters");

		public static readonly ActionIdentifier DescribeDefaultClusterParameters = new ActionIdentifier("redshift:DescribeDefaultClusterParameters");

		public static readonly ActionIdentifier DescribeEventCategories = new ActionIdentifier("redshift:DescribeEventCategories");

		public static readonly ActionIdentifier DescribeEventSubscriptions = new ActionIdentifier("redshift:DescribeEventSubscriptions");

		public static readonly ActionIdentifier DescribeEvents = new ActionIdentifier("redshift:DescribeEvents");

		public static readonly ActionIdentifier DescribeHsmClientCertificates = new ActionIdentifier("redshift:DescribeHsmClientCertificates");

		public static readonly ActionIdentifier DescribeHsmConfigurations = new ActionIdentifier("redshift:DescribeHsmConfigurations");

		public static readonly ActionIdentifier DescribeLoggingStatus = new ActionIdentifier("redshift:DescribeLoggingStatus");

		public static readonly ActionIdentifier DescribeOrderableClusterOptions = new ActionIdentifier("redshift:DescribeOrderableClusterOptions");

		public static readonly ActionIdentifier DescribeReservedNodeOfferings = new ActionIdentifier("redshift:DescribeReservedNodeOfferings");

		public static readonly ActionIdentifier DescribeReservedNodes = new ActionIdentifier("redshift:DescribeReservedNodes");

		public static readonly ActionIdentifier DescribeResize = new ActionIdentifier("redshift:DescribeResize");

		public static readonly ActionIdentifier DisableLogging = new ActionIdentifier("redshift:DisableLogging");

		public static readonly ActionIdentifier DisableSnapshotCopy = new ActionIdentifier("redshift:DisableSnapshotCopy");

		public static readonly ActionIdentifier EnableLogging = new ActionIdentifier("redshift:EnableLogging");

		public static readonly ActionIdentifier EnableSnapshotCopy = new ActionIdentifier("redshift:EnableSnapshotCopy");

		public static readonly ActionIdentifier ModifyCluster = new ActionIdentifier("redshift:ModifyCluster");

		public static readonly ActionIdentifier ModifyClusterParameterGroup = new ActionIdentifier("redshift:ModifyClusterParameterGroup");

		public static readonly ActionIdentifier ModifyClusterSubnetGroup = new ActionIdentifier("redshift:ModifyClusterSubnetGroup");

		public static readonly ActionIdentifier ModifyEventSubscription = new ActionIdentifier("redshift:ModifyEventSubscription");

		public static readonly ActionIdentifier ModifySnapshotCopyRetentionPeriod = new ActionIdentifier("redshift:ModifySnapshotCopyRetentionPeriod");

		public static readonly ActionIdentifier PurchaseReservedNodeOffering = new ActionIdentifier("redshift:PurchaseReservedNodeOffering");

		public static readonly ActionIdentifier RebootCluster = new ActionIdentifier("redshift:RebootCluster");

		public static readonly ActionIdentifier ResetClusterParameterGroup = new ActionIdentifier("redshift:ResetClusterParameterGroup");

		public static readonly ActionIdentifier RestoreFromClusterSnapshot = new ActionIdentifier("redshift:RestoreFromClusterSnapshot");

		public static readonly ActionIdentifier RevokeClusterSecurityGroupIngress = new ActionIdentifier("redshift:RevokeClusterSecurityGroupIngress");

		public static readonly ActionIdentifier RevokeSnapshotAccess = new ActionIdentifier("redshift:RevokeSnapshotAccess");

		public static readonly ActionIdentifier RotateEncryptionKey = new ActionIdentifier("redshift:RotateEncryptionKey");

		public static readonly ActionIdentifier ViewQueriesInConsole = new ActionIdentifier("redshift:ViewQueriesInConsole");
	}
}
