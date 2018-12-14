using Amazon.Runtime.Internal.Util;
using Amazon.Util;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Amazon.Runtime.Internal.Auth
{
	public class AWS4PreSignedUrlSigner : AWS4Signer
	{
		public const long MaxAWS4PreSignedUrlExpiry = 604800L;

		internal const string XAmzSignature = "X-Amz-Signature";

		internal const string XAmzAlgorithm = "X-Amz-Algorithm";

		internal const string XAmzCredential = "X-Amz-Credential";

		internal const string XAmzExpires = "X-Amz-Expires";

		public const string UnsignedPayload = "UNSIGNED-PAYLOAD";

		public override void Sign(IRequest request, IClientConfig clientConfig, RequestMetrics metrics, string awsAccessKeyId, string awsSecretAccessKey)
		{
			throw new InvalidOperationException("PreSignedUrl signature computation is not supported by this method; use SignRequest instead.");
		}

		public new AWS4SigningResult SignRequest(IRequest request, IClientConfig clientConfig, RequestMetrics metrics, string awsAccessKeyId, string awsSecretAccessKey)
		{
			return SignRequest(request, clientConfig, metrics, awsAccessKeyId, awsSecretAccessKey, "s3", null);
		}

		public static AWS4SigningResult SignRequest(IRequest request, IClientConfig clientConfig, RequestMetrics metrics, string awsAccessKeyId, string awsSecretAccessKey, string service, string overrideSigningRegion)
		{
			request.Headers.Remove("Authorization");
			if (!request.Headers.ContainsKey("host"))
			{
				string text = request.Endpoint.Host;
				if (!request.Endpoint.IsDefaultPort)
				{
					text = text + ":" + request.Endpoint.Port;
				}
				request.Headers.Add("host", text);
			}
			DateTime correctedUtcNow = AWSSDKUtils.CorrectedUtcNow;
			string text2 = overrideSigningRegion ?? AWS4Signer.DetermineSigningRegion(clientConfig, service, request.AlternateEndpoint, request);
			if (request.Headers.ContainsKey("X-Amz-Content-SHA256"))
			{
				request.Headers.Remove("X-Amz-Content-SHA256");
			}
			IDictionary<string, string> sortedHeaders = AWS4Signer.SortHeaders(request.Headers);
			string text3 = AWS4Signer.CanonicalizeHeaderNames(sortedHeaders);
			IDictionary<string, string> parametersToCanonicalize = AWS4Signer.GetParametersToCanonicalize(request);
			parametersToCanonicalize.Add("X-Amz-Algorithm", "AWS4-HMAC-SHA256");
			parametersToCanonicalize.Add("X-Amz-Credential", string.Format(CultureInfo.InvariantCulture, "{0}/{1}/{2}/{3}/{4}", awsAccessKeyId, AWS4Signer.FormatDateTime(correctedUtcNow, "yyyyMMdd"), text2, service, "aws4_request"));
			parametersToCanonicalize.Add("X-Amz-Date", AWS4Signer.FormatDateTime(correctedUtcNow, "yyyyMMddTHHmmssZ"));
			parametersToCanonicalize.Add("X-Amz-SignedHeaders", text3);
			string canonicalQueryString = AWS4Signer.CanonicalizeQueryParameters(parametersToCanonicalize);
			string text4 = AWS4Signer.CanonicalizeRequest(request.Endpoint, request.ResourcePath, request.HttpMethod, sortedHeaders, canonicalQueryString, (service == "s3") ? "UNSIGNED-PAYLOAD" : "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855");
			metrics?.AddProperty(Metric.CanonicalRequest, text4);
			return AWS4Signer.ComputeSignature(awsAccessKeyId, awsSecretAccessKey, text2, correctedUtcNow, service, text3, text4, metrics);
		}
	}
}
