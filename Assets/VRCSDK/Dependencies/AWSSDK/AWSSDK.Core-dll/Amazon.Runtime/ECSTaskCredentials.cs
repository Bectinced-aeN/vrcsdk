using Amazon.Util;
using System;
using System.Globalization;

namespace Amazon.Runtime
{
	public class ECSTaskCredentials : URIBasedRefreshingCredentialHelper
	{
		public const string ContainerCredentialsURIEnvVariable = "AWS_CONTAINER_CREDENTIALS_RELATIVE_URI";

		public const string EndpointAddress = "http://169.254.170.2";

		private string Uri;

		private string Server;

		private static int MaxRetries = 5;

		public ECSTaskCredentials()
		{
			Uri = Environment.GetEnvironmentVariable("AWS_CONTAINER_CREDENTIALS_RELATIVE_URI");
			Server = "http://169.254.170.2";
		}

		protected override CredentialsRefreshState GenerateNewCredentials()
		{
			SecurityCredentials securityCredentials = null;
			Uri uri = new Uri(Server + Uri);
			JitteredDelay jitteredDelay = new JitteredDelay(new TimeSpan(0, 0, 0, 0, 200), new TimeSpan(0, 0, 0, 0, 50));
			int num = 1;
			while (true)
			{
				try
				{
					securityCredentials = URIBasedRefreshingCredentialHelper.GetObjectFromResponse<SecurityCredentials>(uri);
					if (securityCredentials != null)
					{
						break;
					}
				}
				catch (Exception ex)
				{
					if (num == MaxRetries)
					{
						throw new AmazonServiceException(string.Format(CultureInfo.InvariantCulture, "Unable to retrieve credentials. Message = \"{0}\".", ex.Message));
					}
				}
				AWSSDKUtils.Sleep(jitteredDelay.Next());
				num++;
			}
			return new CredentialsRefreshState
			{
				Credentials = new ImmutableCredentials(securityCredentials.AccessKeyId, securityCredentials.SecretAccessKey, securityCredentials.Token),
				Expiration = securityCredentials.Expiration
			};
		}
	}
}
