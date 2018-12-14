namespace Amazon.Auth.AccessControlPolicy.ActionIdentifiers
{
	public static class DirectConnectActionIdentifiers
	{
		public static readonly ActionIdentifier AllDirectConnectActions = new ActionIdentifier("directconnect:*");

		public static readonly ActionIdentifier CreateConnection = new ActionIdentifier("directconnect:CreateConnection");

		public static readonly ActionIdentifier CreatePrivateVirtualInterface = new ActionIdentifier("directconnect:CreatePrivateVirtualInterface");

		public static readonly ActionIdentifier CreatePublicVirtualInterface = new ActionIdentifier("directconnect:CreatePublicVirtualInterface");

		public static readonly ActionIdentifier DeleteConnection = new ActionIdentifier("directconnect:DeleteConnection");

		public static readonly ActionIdentifier DeleteVirtualInterface = new ActionIdentifier("directconnect:DeleteVirtualInterface");

		public static readonly ActionIdentifier DescribeConnectionDetail = new ActionIdentifier("directconnect:DescribeConnectionDetail");

		public static readonly ActionIdentifier DescribeConnections = new ActionIdentifier("directconnect:DescribeConnections");

		public static readonly ActionIdentifier DescribeOfferingDetail = new ActionIdentifier("directconnect:DescribeOfferingDetail");

		public static readonly ActionIdentifier DescribeOfferings = new ActionIdentifier("directconnect:DescribeOfferings");

		public static readonly ActionIdentifier DescribeVirtualGateways = new ActionIdentifier("directconnect:DescribeVirtualGateways");

		public static readonly ActionIdentifier DescribeVirtualInterfaces = new ActionIdentifier("directconnect:DescribeVirtualInterfaces");
	}
}
