using Amazon.Runtime.Internal.Util;
using Amazon.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Amazon.Runtime.Internal.Auth
{
	public class AWS4Signer : AbstractAWSSigner
	{
		public const string Scheme = "AWS4";

		public const string Algorithm = "HMAC-SHA256";

		public const string AWS4AlgorithmTag = "AWS4-HMAC-SHA256";

		public const string Terminator = "aws4_request";

		public static readonly byte[] TerminatorBytes = Encoding.UTF8.GetBytes("aws4_request");

		public const string Credential = "Credential";

		public const string SignedHeaders = "SignedHeaders";

		public const string Signature = "Signature";

		public const string EmptyBodySha256 = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855";

		public const string StreamingBodySha256 = "STREAMING-AWS4-HMAC-SHA256-PAYLOAD";

		public const string AWSChunkedEncoding = "aws-chunked";

		private static readonly Regex CompressWhitespaceRegex = new Regex("\\s+");

		private const SigningAlgorithm SignerAlgorithm = SigningAlgorithm.HmacSHA256;

		public override ClientProtocol Protocol => ClientProtocol.RestProtocol;

		public override void Sign(IRequest request, IClientConfig clientConfig, RequestMetrics metrics, string awsAccessKeyId, string awsSecretAccessKey)
		{
			AWS4SigningResult aWS4SigningResult = SignRequest(request, clientConfig, metrics, awsAccessKeyId, awsSecretAccessKey);
			request.Headers["Authorization"] = aWS4SigningResult.ForAuthorizationHeader;
		}

		public AWS4SigningResult SignRequest(IRequest request, IClientConfig clientConfig, RequestMetrics metrics, string awsAccessKeyId, string awsSecretAccessKey)
		{
			DateTime signedAt = InitializeHeaders(request.Headers, request.Endpoint);
			string text = DetermineService(clientConfig);
			string region = DetermineSigningRegion(clientConfig, text, request.AlternateEndpoint, request);
			string canonicalQueryString = CanonicalizeQueryParameters(GetParametersToCanonicalize(request));
			string precomputedBodyHash = SetRequestBodyHash(request);
			IDictionary<string, string> sortedHeaders = SortHeaders(request.Headers);
			string text2 = CanonicalizeRequest(request.Endpoint, request.ResourcePath, request.HttpMethod, sortedHeaders, canonicalQueryString, precomputedBodyHash);
			metrics?.AddProperty(Metric.CanonicalRequest, text2);
			return ComputeSignature(awsAccessKeyId, awsSecretAccessKey, region, signedAt, text, CanonicalizeHeaderNames(sortedHeaders), text2, metrics);
		}

		public static DateTime InitializeHeaders(IDictionary<string, string> headers, Uri requestEndpoint)
		{
			return InitializeHeaders(headers, requestEndpoint, AWSSDKUtils.CorrectedUtcNow);
		}

		public static DateTime InitializeHeaders(IDictionary<string, string> headers, Uri requestEndpoint, DateTime requestDateTime)
		{
			CleanHeaders(headers);
			if (!headers.ContainsKey("host"))
			{
				string text = requestEndpoint.Host;
				if (!requestEndpoint.IsDefaultPort)
				{
					text = text + ":" + requestEndpoint.Port;
				}
				headers.Add("host", text);
			}
			DateTime result = requestDateTime;
			headers["X-Amz-Date"] = result.ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture);
			return result;
		}

		private static void CleanHeaders(IDictionary<string, string> headers)
		{
			headers.Remove("Authorization");
			headers.Remove("X-Amz-Content-SHA256");
			if (headers.ContainsKey("X-Amz-Decoded-Content-Length"))
			{
				headers["Content-Length"] = headers["X-Amz-Decoded-Content-Length"];
				headers.Remove("X-Amz-Decoded-Content-Length");
			}
		}

		public static AWS4SigningResult ComputeSignature(ImmutableCredentials credentials, string region, DateTime signedAt, string service, string signedHeaders, string canonicalRequest)
		{
			return ComputeSignature(credentials.AccessKey, credentials.SecretKey, region, signedAt, service, signedHeaders, canonicalRequest);
		}

		public static AWS4SigningResult ComputeSignature(string awsAccessKey, string awsSecretAccessKey, string region, DateTime signedAt, string service, string signedHeaders, string canonicalRequest)
		{
			return ComputeSignature(awsAccessKey, awsSecretAccessKey, region, signedAt, service, signedHeaders, canonicalRequest, null);
		}

		public static AWS4SigningResult ComputeSignature(string awsAccessKey, string awsSecretAccessKey, string region, DateTime signedAt, string service, string signedHeaders, string canonicalRequest, RequestMetrics metrics)
		{
			string text = FormatDateTime(signedAt, "yyyyMMdd");
			string text2 = string.Format(CultureInfo.InvariantCulture, "{0}/{1}/{2}/{3}", text, region, service, "aws4_request");
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}-{1}\n{2}\n{3}\n", "AWS4", "HMAC-SHA256", FormatDateTime(signedAt, "yyyyMMddTHHmmssZ"), text2);
			byte[] data = ComputeHash(canonicalRequest);
			stringBuilder.Append(AWSSDKUtils.ToHex(data, lowercase: true));
			metrics?.AddProperty(Metric.StringToSign, stringBuilder);
			byte[] array = ComposeSigningKey(awsSecretAccessKey, region, text, service);
			string data2 = stringBuilder.ToString();
			byte[] signature = ComputeKeyedHash(SigningAlgorithm.HmacSHA256, array, data2);
			return new AWS4SigningResult(awsAccessKey, signedAt, signedHeaders, text2, array, signature);
		}

		public static string FormatDateTime(DateTime dt, string formatString)
		{
			return dt.ToString(formatString, CultureInfo.InvariantCulture);
		}

		public static byte[] ComposeSigningKey(string awsSecretAccessKey, string region, string date, string service)
		{
			char[] array = null;
			try
			{
				array = ("AWS4" + awsSecretAccessKey).ToCharArray();
				byte[] key = ComputeKeyedHash(SigningAlgorithm.HmacSHA256, Encoding.UTF8.GetBytes(array), Encoding.UTF8.GetBytes(date));
				byte[] key2 = ComputeKeyedHash(SigningAlgorithm.HmacSHA256, key, Encoding.UTF8.GetBytes(region));
				byte[] key3 = ComputeKeyedHash(SigningAlgorithm.HmacSHA256, key2, Encoding.UTF8.GetBytes(service));
				return ComputeKeyedHash(SigningAlgorithm.HmacSHA256, key3, TerminatorBytes);
			}
			finally
			{
				if (array != null)
				{
					Array.Clear(array, 0, array.Length);
				}
			}
		}

		public static string SetRequestBodyHash(IRequest request)
		{
			string value = null;
			bool flag = request.Headers.TryGetValue("X-Amz-Content-SHA256", out value);
			if (flag && !request.UseChunkEncoding)
			{
				return value;
			}
			if (request.UseChunkEncoding)
			{
				value = "STREAMING-AWS4-HMAC-SHA256-PAYLOAD";
				if (request.Headers.ContainsKey("Content-Length"))
				{
					request.Headers["X-Amz-Decoded-Content-Length"] = request.Headers["Content-Length"];
					long originalLength = long.Parse(request.Headers["Content-Length"], CultureInfo.InvariantCulture);
					request.Headers["Content-Length"] = ChunkedUploadWrapperStream.ComputeChunkedContentLength(originalLength).ToString(CultureInfo.InvariantCulture);
				}
				if (request.Headers.ContainsKey("Content-Encoding"))
				{
					string text = request.Headers["Content-Encoding"];
					if (!text.Contains("aws-chunked"))
					{
						request.Headers["Content-Encoding"] = text + ", " + "aws-chunked";
					}
				}
				else
				{
					request.Headers["Content-Encoding"] = "aws-chunked";
				}
			}
			else if (request.ContentStream != null)
			{
				value = request.ComputeContentStreamHash();
			}
			else
			{
				byte[] requestPayloadBytes = GetRequestPayloadBytes(request);
				value = AWSSDKUtils.ToHex(CryptoUtilFactory.CryptoInstance.ComputeSHA256Hash(requestPayloadBytes), lowercase: true);
			}
			if (value != null)
			{
				if (flag)
				{
					request.Headers["X-Amz-Content-SHA256"] = value;
				}
				else
				{
					request.Headers.Add("X-Amz-Content-SHA256", value);
				}
			}
			return value;
		}

		public static byte[] SignBlob(byte[] key, string data)
		{
			return SignBlob(key, Encoding.UTF8.GetBytes(data));
		}

		public static byte[] SignBlob(byte[] key, byte[] data)
		{
			return CryptoUtilFactory.CryptoInstance.HMACSignBinary(data, key, SigningAlgorithm.HmacSHA256);
		}

		public static byte[] ComputeKeyedHash(SigningAlgorithm algorithm, byte[] key, string data)
		{
			return ComputeKeyedHash(algorithm, key, Encoding.UTF8.GetBytes(data));
		}

		public static byte[] ComputeKeyedHash(SigningAlgorithm algorithm, byte[] key, byte[] data)
		{
			return CryptoUtilFactory.CryptoInstance.HMACSignBinary(data, key, algorithm);
		}

		public static byte[] ComputeHash(string data)
		{
			return ComputeHash(Encoding.UTF8.GetBytes(data));
		}

		public static byte[] ComputeHash(byte[] data)
		{
			return CryptoUtilFactory.CryptoInstance.ComputeSHA256Hash(data);
		}

		public static string DetermineSigningRegion(IClientConfig clientConfig, string serviceName, RegionEndpoint alternateEndpoint, IRequest request)
		{
			if (alternateEndpoint != null)
			{
				RegionEndpoint.Endpoint endpointForService = alternateEndpoint.GetEndpointForService(serviceName, clientConfig.UseDualstackEndpoint);
				if (endpointForService.AuthRegion != null)
				{
					return endpointForService.AuthRegion;
				}
				return alternateEndpoint.SystemName;
			}
			string authenticationRegion = clientConfig.AuthenticationRegion;
			if (request != null && request.AuthenticationRegion != null)
			{
				authenticationRegion = request.AuthenticationRegion;
			}
			if (!string.IsNullOrEmpty(authenticationRegion))
			{
				return authenticationRegion.ToLowerInvariant();
			}
			if (!string.IsNullOrEmpty(clientConfig.ServiceURL))
			{
				string text = AWSSDKUtils.DetermineRegion(clientConfig.ServiceURL);
				if (!string.IsNullOrEmpty(text))
				{
					return text.ToLowerInvariant();
				}
			}
			RegionEndpoint regionEndpoint = clientConfig.RegionEndpoint;
			if (regionEndpoint != null)
			{
				RegionEndpoint.Endpoint endpointForService2 = regionEndpoint.GetEndpointForService(serviceName, clientConfig.UseDualstackEndpoint);
				if (!string.IsNullOrEmpty(endpointForService2.AuthRegion))
				{
					return endpointForService2.AuthRegion;
				}
				return regionEndpoint.SystemName;
			}
			return string.Empty;
		}

		internal static string DetermineService(IClientConfig clientConfig)
		{
			if (string.IsNullOrEmpty(clientConfig.AuthenticationServiceName))
			{
				return AWSSDKUtils.DetermineService(clientConfig.DetermineServiceURL());
			}
			return clientConfig.AuthenticationServiceName;
		}

		protected static string CanonicalizeRequest(Uri endpoint, string resourcePath, string httpMethod, IDictionary<string, string> sortedHeaders, string canonicalQueryString, string precomputedBodyHash)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("{0}\n", httpMethod);
			stringBuilder.AppendFormat("{0}\n", AWSSDKUtils.CanonicalizeResourcePath(endpoint, resourcePath));
			stringBuilder.AppendFormat("{0}\n", canonicalQueryString);
			stringBuilder.AppendFormat("{0}\n", CanonicalizeHeaders(sortedHeaders));
			stringBuilder.AppendFormat("{0}\n", CanonicalizeHeaderNames(sortedHeaders));
			string value;
			if (precomputedBodyHash != null)
			{
				stringBuilder.Append(precomputedBodyHash);
			}
			else if (sortedHeaders.TryGetValue("X-Amz-Content-SHA256", out value))
			{
				stringBuilder.Append(value);
			}
			return stringBuilder.ToString();
		}

		protected static IDictionary<string, string> SortHeaders(IEnumerable<KeyValuePair<string, string>> requestHeaders)
		{
			SortedDictionary<string, string> sortedDictionary = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			foreach (KeyValuePair<string, string> requestHeader in requestHeaders)
			{
				sortedDictionary.Add(requestHeader.Key, requestHeader.Value);
			}
			return sortedDictionary;
		}

		protected static string CanonicalizeHeaders(IEnumerable<KeyValuePair<string, string>> sortedHeaders)
		{
			if (sortedHeaders == null || sortedHeaders.Count() == 0)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (AWSSDKUtils.IsIL2CPP)
			{
				foreach (KeyValuePair<string, string> item in from kvp in sortedHeaders
				orderby kvp.Key.ToLowerInvariant()
				select kvp)
				{
					stringBuilder.Append(item.Key.ToLowerInvariant());
					stringBuilder.Append(":");
					stringBuilder.Append(CompressSpaces(item.Value));
					stringBuilder.Append("\n");
				}
				return stringBuilder.ToString();
			}
			foreach (KeyValuePair<string, string> sortedHeader in sortedHeaders)
			{
				stringBuilder.Append(sortedHeader.Key.ToLowerInvariant());
				stringBuilder.Append(":");
				stringBuilder.Append(CompressSpaces(sortedHeader.Value));
				stringBuilder.Append("\n");
			}
			return stringBuilder.ToString();
		}

		protected static string CanonicalizeHeaderNames(IEnumerable<KeyValuePair<string, string>> sortedHeaders)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (AWSSDKUtils.IsIL2CPP)
			{
				foreach (KeyValuePair<string, string> item in from kvp in sortedHeaders
				orderby kvp.Key.ToLowerInvariant()
				select kvp)
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(";");
					}
					stringBuilder.Append(item.Key.ToLowerInvariant());
				}
				return stringBuilder.ToString();
			}
			foreach (KeyValuePair<string, string> sortedHeader in sortedHeaders)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(";");
				}
				stringBuilder.Append(sortedHeader.Key.ToLowerInvariant());
			}
			return stringBuilder.ToString();
		}

		protected static IDictionary<string, string> GetParametersToCanonicalize(IRequest request)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (request.SubResources != null && request.SubResources.Count > 0)
			{
				foreach (KeyValuePair<string, string> subResource in request.SubResources)
				{
					dictionary.Add(subResource.Key, subResource.Value);
				}
			}
			if (request.UseQueryString && request.Parameters != null && request.Parameters.Count > 0)
			{
				foreach (KeyValuePair<string, string> item in from queryParameter in request.Parameters
				where queryParameter.Value != null
				select queryParameter)
				{
					dictionary.Add(item.Key, item.Value);
				}
				return dictionary;
			}
			return dictionary;
		}

		protected static string CanonicalizeQueryParameters(string queryString)
		{
			return CanonicalizeQueryParameters(queryString, uriEncodeParameters: true);
		}

		protected static string CanonicalizeQueryParameters(string queryString, bool uriEncodeParameters)
		{
			if (string.IsNullOrEmpty(queryString))
			{
				return string.Empty;
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			int num = queryString.IndexOf('?');
			string text = queryString.Substring(++num);
			int num2 = 0;
			int num3 = text.IndexOfAny(new char[2]
			{
				'&',
				';'
			}, 0);
			if (num3 == -1 && num2 < text.Length)
			{
				num3 = text.Length;
			}
			while (num3 != -1)
			{
				string text2 = text.Substring(num2, num3 - num2);
				if (num3 + 1 >= text.Length || text[num3 + 1] != ' ')
				{
					int num4 = text2.IndexOf('=');
					if (num4 == -1)
					{
						dictionary.Add(text2, null);
					}
					else
					{
						dictionary.Add(text2.Substring(0, num4), text2.Substring(num4 + 1));
					}
					num2 = num3 + 1;
				}
				if (text.Length <= num3 + 1)
				{
					break;
				}
				num3 = text.IndexOfAny(new char[2]
				{
					'&',
					';'
				}, num3 + 1);
				if (num3 == -1 && num2 < text.Length)
				{
					num3 = text.Length;
				}
			}
			return CanonicalizeQueryParameters(dictionary, uriEncodeParameters);
		}

		protected static string CanonicalizeQueryParameters(IDictionary<string, string> parameters)
		{
			return CanonicalizeQueryParameters(parameters, uriEncodeParameters: true);
		}

		protected static string CanonicalizeQueryParameters(IDictionary<string, string> parameters, bool uriEncodeParameters)
		{
			if (parameters == null || parameters.Count == 0)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, string> item in new SortedDictionary<string, string>(parameters, StringComparer.Ordinal))
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append("&");
				}
				if (uriEncodeParameters)
				{
					if (string.IsNullOrEmpty(item.Value))
					{
						stringBuilder.AppendFormat("{0}=", AWSSDKUtils.UrlEncode(item.Key, path: false));
					}
					else
					{
						stringBuilder.AppendFormat("{0}={1}", AWSSDKUtils.UrlEncode(item.Key, path: false), AWSSDKUtils.UrlEncode(item.Value, path: false));
					}
				}
				else if (string.IsNullOrEmpty(item.Value))
				{
					stringBuilder.AppendFormat("{0}=", item.Key);
				}
				else
				{
					stringBuilder.AppendFormat("{0}={1}", item.Key, item.Value);
				}
			}
			return stringBuilder.ToString();
		}

		private static string CompressSpaces(string data)
		{
			if (data == null || !data.Contains(" "))
			{
				return data;
			}
			return CompressWhitespaceRegex.Replace(data, " ");
		}

		private static byte[] GetRequestPayloadBytes(IRequest request)
		{
			if (request.Content != null)
			{
				return request.Content;
			}
			string s = request.UseQueryString ? string.Empty : AWSSDKUtils.GetParametersAsString(request.Parameters);
			return Encoding.UTF8.GetBytes(s);
		}
	}
}
