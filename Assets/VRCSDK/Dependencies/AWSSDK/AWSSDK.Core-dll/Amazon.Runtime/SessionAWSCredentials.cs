using System;

namespace Amazon.Runtime
{
	public class SessionAWSCredentials : AWSCredentials
	{
		private ImmutableCredentials _lastCredentials;

		public SessionAWSCredentials(string awsAccessKeyId, string awsSecretAccessKey, string token)
		{
			if (string.IsNullOrEmpty(awsAccessKeyId))
			{
				throw new ArgumentNullException("awsAccessKeyId");
			}
			if (string.IsNullOrEmpty(awsSecretAccessKey))
			{
				throw new ArgumentNullException("awsSecretAccessKey");
			}
			if (string.IsNullOrEmpty(token))
			{
				throw new ArgumentNullException("token");
			}
			_lastCredentials = new ImmutableCredentials(awsAccessKeyId, awsSecretAccessKey, token);
		}

		public override ImmutableCredentials GetCredentials()
		{
			return _lastCredentials.Copy();
		}
	}
}
