using Amazon.Runtime.Internal.Util;
using Amazon.Util;
using System;

namespace Amazon.Runtime.Internal.Auth
{
	public class CloudFrontSigner : AbstractAWSSigner
	{
		public override ClientProtocol Protocol => ClientProtocol.RestProtocol;

		public override void Sign(IRequest request, IClientConfig clientConfig, RequestMetrics metrics, string awsAccessKeyId, string awsSecretAccessKey)
		{
			if (string.IsNullOrEmpty(awsAccessKeyId))
			{
				throw new ArgumentOutOfRangeException("awsAccessKeyId", "The AWS Access Key ID cannot be NULL or a Zero length string");
			}
			string formattedTimestampRFC = AWSSDKUtils.GetFormattedTimestampRFC822(0);
			request.Headers.Add("X-Amz-Date", formattedTimestampRFC);
			string str = AbstractAWSSigner.ComputeHash(formattedTimestampRFC, awsSecretAccessKey, SigningAlgorithm.HmacSHA1);
			request.Headers.Add("Authorization", "AWS " + awsAccessKeyId + ":" + str);
		}
	}
}
