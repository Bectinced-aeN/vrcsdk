namespace Amazon.S3.Model
{
	public class StorageClassAnalysis
	{
		private StorageClassAnalysisDataExport storageClassAnalysisDataExport;

		public StorageClassAnalysisDataExport DataExport
		{
			get
			{
				return storageClassAnalysisDataExport;
			}
			set
			{
				storageClassAnalysisDataExport = value;
			}
		}

		internal bool IsSetDataExport()
		{
			return storageClassAnalysisDataExport != null;
		}
	}
}
