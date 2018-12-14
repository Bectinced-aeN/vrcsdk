namespace Amazon.Auth.AccessControlPolicy.ActionIdentifiers
{
	public static class WhispersyncActionIdentifiers
	{
		public static readonly ActionIdentifier AllWhispersyncActions = new ActionIdentifier("whispersync:*");

		public static readonly ActionIdentifier GetDatamapUpdates = new ActionIdentifier("whispersync:GetDatamapUpdates");

		public static readonly ActionIdentifier PatchDatamapUpdates = new ActionIdentifier("whispersync:PatchDatamapUpdates");
	}
}
