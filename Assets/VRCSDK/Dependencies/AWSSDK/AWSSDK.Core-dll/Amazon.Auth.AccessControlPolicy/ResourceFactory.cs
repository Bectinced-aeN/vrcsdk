using System;
using System.Globalization;

namespace Amazon.Auth.AccessControlPolicy
{
	public static class ResourceFactory
	{
		public static Resource NewS3BucketResource(string bucketName)
		{
			if (bucketName == null)
			{
				throw new ArgumentNullException("bucketName");
			}
			return new Resource("arn:aws:s3:::" + bucketName);
		}

		public static Resource NewS3ObjectResource(string bucketName, string keyPattern)
		{
			if (bucketName == null)
			{
				throw new ArgumentNullException("bucketName");
			}
			if (keyPattern == null)
			{
				throw new ArgumentNullException("keyPattern");
			}
			return new Resource(string.Format(CultureInfo.InvariantCulture, "arn:aws:s3:::{0}/{1}", bucketName, keyPattern));
		}

		public static Resource NewSQSQueueResource(string accountId, string queueName)
		{
			return new Resource("/" + FormatAccountId(accountId) + "/" + queueName);
		}

		private static string FormatAccountId(string accountId)
		{
			if (accountId == null)
			{
				throw new ArgumentNullException("accountId");
			}
			return accountId.Trim().Replace("-", "");
		}
	}
}
