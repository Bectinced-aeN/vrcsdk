namespace Amazon.S3.Model
{
	public class StorageClassAnalysisDataExport
	{
		private StorageClassAnalysisSchemaVersion storageClassAnalysisSchemaVersion;

		private AnalyticsExportDestination analyticsExportDestination;

		public StorageClassAnalysisSchemaVersion OutputSchemaVersion
		{
			get
			{
				return storageClassAnalysisSchemaVersion;
			}
			set
			{
				storageClassAnalysisSchemaVersion = value;
			}
		}

		public AnalyticsExportDestination Destination
		{
			get
			{
				return analyticsExportDestination;
			}
			set
			{
				analyticsExportDestination = value;
			}
		}

		internal bool IsSetOutputSchemaVersion()
		{
			return storageClassAnalysisSchemaVersion != null;
		}

		internal bool IsSetDestination()
		{
			return analyticsExportDestination != null;
		}
	}
}
