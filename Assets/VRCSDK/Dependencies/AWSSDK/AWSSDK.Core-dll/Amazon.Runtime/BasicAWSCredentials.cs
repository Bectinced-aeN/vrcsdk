namespace Amazon.Runtime
{
	public class BasicAWSCredentials : AWSCredentials
	{
		private ImmutableCredentials _credentials;

		public BasicAWSCredentials(string accessKey, string secretKey)
		{
			if (!string.IsNullOrEmpty(accessKey))
			{
				_credentials = new ImmutableCredentials(accessKey, secretKey, null);
			}
		}

		public override ImmutableCredentials GetCredentials()
		{
			if (_credentials == null)
			{
				return null;
			}
			return _credentials.Copy();
		}
	}
}
