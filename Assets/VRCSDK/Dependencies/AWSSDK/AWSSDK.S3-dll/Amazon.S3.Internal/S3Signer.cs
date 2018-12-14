using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Auth;
using Amazon.Runtime.Internal.Util;
using Amazon.S3.Util;
using Amazon.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amazon.S3.Internal
{
	public class S3Signer : AbstractAWSSigner
	{
		private readonly bool _useSigV4;

		private static readonly HashSet<string> SignableParameters = new HashSet<string>(new string[6]
		{
			"response-content-type",
			"response-content-language",
			"response-expires",
			"response-cache-control",
			"response-content-disposition",
			"response-content-encoding"
		}, StringComparer.OrdinalIgnoreCase);

		private static readonly HashSet<string> SubResourcesSigningExclusion = new HashSet<string>(new string[1]
		{
			"id"
		}, StringComparer.OrdinalIgnoreCase);

		public override ClientProtocol Protocol => 1;

		public S3Signer()
			: this()
		{
			_useSigV4 = AWSConfigsS3.UseSignatureVersion4;
		}

		public override void Sign(IRequest request, IClientConfig clientConfig, RequestMetrics metrics, string awsAccessKeyId, string awsSecretAccessKey)
		{
			AWS4Signer val = this.SelectSigner(this, _useSigV4, request, clientConfig) as AWS4Signer;
			if (val != null)
			{
				AmazonS3Uri amazonS3Uri;
				RegionEndpoint alternateEndpoint = default(RegionEndpoint);
				if (AmazonS3Uri.TryParseAmazonS3Uri(request.get_Endpoint(), out amazonS3Uri) && amazonS3Uri.Bucket != null && BucketRegionDetector.BucketRegionCache.TryGetValue(amazonS3Uri.Bucket, ref alternateEndpoint))
				{
					request.set_AlternateEndpoint(alternateEndpoint);
				}
				AWS4SigningResult val2 = val.SignRequest(request, clientConfig, metrics, awsAccessKeyId, awsSecretAccessKey);
				request.get_Headers()["Authorization"] = val2.get_ForAuthorizationHeader();
				if (request.get_UseChunkEncoding())
				{
					request.set_AWS4SignerResult(val2);
				}
			}
			else
			{
				SignRequest(request, metrics, awsAccessKeyId, awsSecretAccessKey);
			}
		}

		internal static void SignRequest(IRequest request, RequestMetrics metrics, string awsAccessKeyId, string awsSecretAccessKey)
		{
			request.get_Headers()["X-Amz-Date"] = AWSSDKUtils.get_FormattedCurrentTimestampRFC822();
			string text = BuildStringToSign(request);
			metrics.AddProperty(17, (object)text);
			string str = CryptoUtilFactory.get_CryptoInstance().HMACSign(text, awsSecretAccessKey, 0);
			string value = "AWS " + awsAccessKeyId + ":" + str;
			request.get_Headers()["Authorization"] = value;
		}

		private static string BuildStringToSign(IRequest request)
		{
			StringBuilder stringBuilder = new StringBuilder("", 256);
			stringBuilder.Append(request.get_HttpMethod());
			stringBuilder.Append("\n");
			IDictionary<string, string> headers = request.get_Headers();
			IDictionary<string, string> parameters = request.get_Parameters();
			if (headers != null)
			{
				string text = null;
				if (headers.ContainsKey("Content-MD5") && !string.IsNullOrEmpty(text = headers["Content-MD5"]))
				{
					stringBuilder.Append(text);
				}
				stringBuilder.Append("\n");
				if (parameters.ContainsKey("ContentType"))
				{
					stringBuilder.Append(parameters["ContentType"]);
				}
				else if (headers.ContainsKey("Content-Type"))
				{
					stringBuilder.Append(headers["Content-Type"]);
				}
				stringBuilder.Append("\n");
			}
			else
			{
				stringBuilder.Append("\n\n");
			}
			if (parameters.ContainsKey("Expires"))
			{
				stringBuilder.Append(parameters["Expires"]);
				headers?.Remove("X-Amz-Date");
			}
			stringBuilder.Append("\n");
			stringBuilder.Append(BuildCanonicalizedHeaders(headers));
			string value = BuildCanonicalizedResource(request);
			if (!string.IsNullOrEmpty(value))
			{
				stringBuilder.Append(value);
			}
			return stringBuilder.ToString();
		}

		private static string BuildCanonicalizedHeaders(IDictionary<string, string> headers)
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			foreach (string item in headers.Keys.OrderBy((string x) => x, StringComparer.OrdinalIgnoreCase))
			{
				string text = item.ToLowerInvariant();
				if (text.StartsWith("x-amz-", StringComparison.Ordinal))
				{
					stringBuilder.Append(text + ":" + headers[item] + "\n");
				}
			}
			return stringBuilder.ToString();
		}

		private static string BuildCanonicalizedResource(IRequest request)
		{
			StringBuilder stringBuilder = new StringBuilder(request.get_CanonicalResourcePrefix());
			stringBuilder.Append((!string.IsNullOrEmpty(request.get_ResourcePath())) ? AWSSDKUtils.UrlEncode(request.get_ResourcePath(), true) : "/");
			Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			if (request.get_SubResources().Count > 0)
			{
				foreach (KeyValuePair<string, string> subResource in request.get_SubResources())
				{
					if (!SubResourcesSigningExclusion.Contains(subResource.Key))
					{
						dictionary.Add(subResource.Key, subResource.Value);
					}
				}
			}
			if (request.get_Parameters().Count > 0)
			{
				foreach (KeyValuePair<string, string> parameter in request.get_Parameters())
				{
					if (parameter.Value != null && SignableParameters.Contains(parameter.Key))
					{
						dictionary.Add(parameter.Key, parameter.Value);
					}
				}
			}
			string arg = "?";
			List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
			foreach (KeyValuePair<string, string> item in dictionary)
			{
				list.Add(item);
			}
			list.Sort((KeyValuePair<string, string> firstPair, KeyValuePair<string, string> nextPair) => string.CompareOrdinal(firstPair.Key, nextPair.Key));
			foreach (KeyValuePair<string, string> item2 in list)
			{
				stringBuilder.AppendFormat("{0}{1}", arg, item2.Key);
				if (item2.Value != null)
				{
					stringBuilder.AppendFormat("={0}", item2.Value);
				}
				arg = "&";
			}
			return stringBuilder.ToString();
		}
	}
}
