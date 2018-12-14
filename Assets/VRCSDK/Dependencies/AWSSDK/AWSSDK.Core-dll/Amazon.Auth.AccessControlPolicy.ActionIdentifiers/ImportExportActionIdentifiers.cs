namespace Amazon.Auth.AccessControlPolicy.ActionIdentifiers
{
	public static class ImportExportActionIdentifiers
	{
		public static readonly ActionIdentifier AllImportExportActions = new ActionIdentifier("importexport:*");

		public static readonly ActionIdentifier CreateJob = new ActionIdentifier("importexport:CreateJob");

		public static readonly ActionIdentifier UpdateJob = new ActionIdentifier("importexport:UpdateJob");

		public static readonly ActionIdentifier CancelJob = new ActionIdentifier("importexport:CancelJob");

		public static readonly ActionIdentifier ListJobs = new ActionIdentifier("importexport:ListJobs");

		public static readonly ActionIdentifier GetStatus = new ActionIdentifier("importexport:GetStatus");
	}
}
