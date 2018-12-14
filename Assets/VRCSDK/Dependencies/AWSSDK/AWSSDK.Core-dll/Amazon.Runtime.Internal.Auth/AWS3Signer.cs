using Amazon.Runtime.Internal.Util;
using Amazon.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amazon.Runtime.Internal.Auth
{
	public class AWS3Signer : AbstractAWSSigner
	{
		private const string HTTP_SCHEME = "AWS3";

		private const string HTTPS_SCHEME = "AWS3-HTTPS";

		private bool UseAws3Https
		{
			get;
			set;
		}

		public override ClientProtocol Protocol => ClientProtocol.RestProtocol;

		public AWS3Signer(bool useAws3Https)
		{
			UseAws3Https = useAws3Https;
		}

		public AWS3Signer()
			: this(useAws3Https: false)
		{
		}

		public override void Sign(IRequest request, IClientConfig clientConfig, RequestMetrics metrics, string awsAccessKeyId, string awsSecretAccessKey)
		{
			AbstractAWSSigner abstractAWSSigner = SelectSigner(request, clientConfig);
			if (abstractAWSSigner is AWS4Signer)
			{
				abstractAWSSigner.Sign(request, clientConfig, metrics, awsAccessKeyId, awsSecretAccessKey);
			}
			else if (UseAws3Https)
			{
				SignHttps(request, clientConfig, metrics, awsAccessKeyId, awsSecretAccessKey);
			}
			else
			{
				SignHttp(request, metrics, awsAccessKeyId, awsSecretAccessKey);
			}
		}

		private static void SignHttps(IRequest request, IClientConfig clientConfig, RequestMetrics metrics, string awsAccessKeyId, string awsSecretAccessKey)
		{
			string text = Guid.NewGuid().ToString();
			string formattedCurrentTimestampRFC = AWSSDKUtils.FormattedCurrentTimestampRFC822;
			string text2 = formattedCurrentTimestampRFC + text;
			metrics.AddProperty(Metric.StringToSign, text2);
			string str = AbstractAWSSigner.ComputeHash(text2, awsSecretAccessKey, clientConfig.SignatureMethod);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("AWS3-HTTPS").Append(" ");
			stringBuilder.Append("AWSAccessKeyId=" + awsAccessKeyId + ",");
			stringBuilder.Append("Algorithm=" + clientConfig.SignatureMethod.ToString() + ",");
			stringBuilder.Append("SignedHeaders=x-amz-date;x-amz-nonce,");
			stringBuilder.Append("Signature=" + str);
			request.Headers["X-Amzn-Authorization"] = stringBuilder.ToString();
			request.Headers["x-amz-nonce"] = text;
			request.Headers["X-Amz-Date"] = formattedCurrentTimestampRFC;
		}

		private static void SignHttp(IRequest request, RequestMetrics metrics, string awsAccessKeyId, string awsSecretAccessKey)
		{
			SigningAlgorithm algorithm = SigningAlgorithm.HmacSHA256;
			string text = Guid.NewGuid().ToString();
			string formattedCurrentTimestampRFC = AWSSDKUtils.FormattedCurrentTimestampRFC822;
			bool flag = IsHttpsRequest(request);
			flag = false;
			request.Headers["Date"] = formattedCurrentTimestampRFC;
			request.Headers["X-Amz-Date"] = formattedCurrentTimestampRFC;
			request.Headers.Remove("X-Amzn-Authorization");
			string text2 = request.Endpoint.Host;
			if (!request.Endpoint.IsDefaultPort)
			{
				text2 = text2 + ":" + request.Endpoint.Port;
			}
			request.Headers["host"] = text2;
			byte[] array = null;
			string text3;
			if (flag)
			{
				request.Headers["x-amz-nonce"] = text;
				text3 = formattedCurrentTimestampRFC + text;
				array = Encoding.UTF8.GetBytes(text3);
			}
			else
			{
				Uri endpoint = request.Endpoint;
				if (!string.IsNullOrEmpty(request.ResourcePath))
				{
					endpoint = new Uri(request.Endpoint, request.ResourcePath);
				}
				text3 = request.HttpMethod + "\n" + GetCanonicalizedResourcePath(endpoint) + "\n" + GetCanonicalizedQueryString(request.Parameters) + "\n" + GetCanonicalizedHeadersForStringToSign(request) + "\n" + GetRequestPayload(request);
				array = CryptoUtilFactory.CryptoInstance.ComputeSHA256Hash(Encoding.UTF8.GetBytes(text3));
			}
			metrics.AddProperty(Metric.StringToSign, text3);
			string str = AbstractAWSSigner.ComputeHash(array, awsSecretAccessKey, algorithm);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(flag ? "AWS3-HTTPS" : "AWS3");
			stringBuilder.Append(" ");
			stringBuilder.Append("AWSAccessKeyId=" + awsAccessKeyId + ",");
			stringBuilder.Append("Algorithm=" + algorithm.ToString() + ",");
			if (!flag)
			{
				stringBuilder.Append(GetSignedHeadersComponent(request) + ",");
			}
			stringBuilder.Append("Signature=" + str);
			string value = stringBuilder.ToString();
			request.Headers["X-Amzn-Authorization"] = value;
		}

		private static string GetCanonicalizedResourcePath(Uri endpoint)
		{
			string absolutePath = endpoint.AbsolutePath;
			if (string.IsNullOrEmpty(absolutePath))
			{
				return "/";
			}
			return AWSSDKUtils.UrlEncode(absolutePath, path: true);
		}

		private static bool IsHttpsRequest(IRequest request)
		{
			string scheme = request.Endpoint.Scheme;
			if (scheme.Equals("http", StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			if (scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			throw new AmazonServiceException("Unknown request endpoint protocol encountered while signing request: " + scheme);
		}

		private static string GetCanonicalizedQueryString(IDictionary<string, string> parameters)
		{
			SortedDictionary<string, string> sortedDictionary = new SortedDictionary<string, string>(parameters, StringComparer.Ordinal);
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, string> item in (IEnumerable<KeyValuePair<string, string>>)sortedDictionary)
			{
				if (item.Value != null)
				{
					string key = item.Key;
					string value = item.Value;
					stringBuilder.Append(AWSSDKUtils.UrlEncode(key, path: false));
					stringBuilder.Append("=");
					stringBuilder.Append(AWSSDKUtils.UrlEncode(value, path: false));
					stringBuilder.Append("&");
				}
			}
			string text = stringBuilder.ToString();
			if (!string.IsNullOrEmpty(text))
			{
				return text.Substring(0, text.Length - 1);
			}
			return string.Empty;
		}

		private static string GetRequestPayload(IRequest request)
		{
			if (request.Content == null)
			{
				return string.Empty;
			}
			return Encoding.UTF8.GetString(request.Content, 0, request.Content.Length);
		}

		private static string GetSignedHeadersComponent(IRequest request)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("SignedHeaders=");
			bool flag = true;
			foreach (string item in GetHeadersForStringToSign(request))
			{
				if (!flag)
				{
					stringBuilder.Append(";");
				}
				stringBuilder.Append(item);
				flag = false;
			}
			return stringBuilder.ToString();
		}

		private static List<string> GetHeadersForStringToSign(IRequest request)
		{
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, string> header in request.Headers)
			{
				string key = header.Key;
				if (key.StartsWith("x-amz", StringComparison.OrdinalIgnoreCase) || key.Equals("content-encoding", StringComparison.OrdinalIgnoreCase) || key.Equals("host", StringComparison.OrdinalIgnoreCase))
				{
					list.Add(key);
				}
			}
			list.Sort(StringComparer.OrdinalIgnoreCase);
			return list;
		}

		private static string GetCanonicalizedHeadersForStringToSign(IRequest request)
		{
			List<string> headersForStringToSign = GetHeadersForStringToSign(request);
			for (int i = 0; i < headersForStringToSign.Count; i++)
			{
				headersForStringToSign[i] = headersForStringToSign[i].ToLowerInvariant();
			}
			SortedDictionary<string, string> sortedDictionary = new SortedDictionary<string, string>();
			foreach (KeyValuePair<string, string> header in request.Headers)
			{
				if (headersForStringToSign.Contains(header.Key.ToLowerInvariant()))
				{
					sortedDictionary[header.Key] = header.Value;
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, string> item in sortedDictionary)
			{
				stringBuilder.Append(item.Key.ToLowerInvariant());
				stringBuilder.Append(":");
				stringBuilder.Append(item.Value);
				stringBuilder.Append("\n");
			}
			return stringBuilder.ToString();
		}
	}
}
