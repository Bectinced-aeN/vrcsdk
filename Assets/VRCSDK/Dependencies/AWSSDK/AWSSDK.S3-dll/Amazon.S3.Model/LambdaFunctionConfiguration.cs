namespace Amazon.S3.Model
{
	public class LambdaFunctionConfiguration : NotificationConfiguration
	{
		public string Id
		{
			get;
			set;
		}

		public string FunctionArn
		{
			get;
			set;
		}

		internal bool IsSetId()
		{
			return Id != null;
		}

		internal bool IsSetFunctionArn()
		{
			return FunctionArn != null;
		}
	}
}
