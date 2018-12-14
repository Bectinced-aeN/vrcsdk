namespace Amazon.S3.Model
{
	public class RoutingRuleCondition
	{
		private string httpErrorCodeReturnedEquals;

		private string keyPrefixEquals;

		public string HttpErrorCodeReturnedEquals
		{
			get
			{
				return httpErrorCodeReturnedEquals;
			}
			set
			{
				httpErrorCodeReturnedEquals = value;
			}
		}

		public string KeyPrefixEquals
		{
			get
			{
				return keyPrefixEquals;
			}
			set
			{
				keyPrefixEquals = value;
			}
		}

		internal bool IsSetHttpErrorCodeReturnedEquals()
		{
			return httpErrorCodeReturnedEquals != null;
		}

		internal bool IsSetKeyPrefixEquals()
		{
			return keyPrefixEquals != null;
		}
	}
}
