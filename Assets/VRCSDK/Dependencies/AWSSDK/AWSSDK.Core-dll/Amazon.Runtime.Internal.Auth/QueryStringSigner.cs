using Amazon.Runtime.Internal.Util;
using Amazon.Util;
using System;

namespace Amazon.Runtime.Internal.Auth
{
	public class QueryStringSigner : AbstractAWSSigner
	{
		public override ClientProtocol Protocol => ClientProtocol.QueryStringProtocol;

		public override void Sign(IRequest request, IClientConfig clientConfig, RequestMetrics metrics, string awsAccessKeyId, string awsSecretAccessKey)
		{
			if (string.IsNullOrEmpty(awsAccessKeyId))
			{
				throw new ArgumentOutOfRangeException("awsAccessKeyId", "The AWS Access Key ID cannot be NULL or a Zero length string");
			}
			request.Parameters["AWSAccessKeyId"] = awsAccessKeyId;
			request.Parameters["SignatureVersion"] = clientConfig.SignatureVersion;
			request.Parameters["SignatureMethod"] = clientConfig.SignatureMethod.ToString();
			request.Parameters["Timestamp"] = AWSSDKUtils.FormattedCurrentTimestampISO8601;
			request.Parameters.Remove("Signature");
			string text = AWSSDKUtils.CalculateStringToSignV2(request.Parameters, request.Endpoint.AbsoluteUri);
			metrics.AddProperty(Metric.StringToSign, text);
			string value = AbstractAWSSigner.ComputeHash(text, awsSecretAccessKey, clientConfig.SignatureMethod);
			request.Parameters["Signature"] = value;
		}
	}
}
