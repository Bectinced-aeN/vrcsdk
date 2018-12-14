using Amazon.Runtime;
using System.Net;

namespace Amazon.S3
{
	internal static class AmazonS3HttpUtil
	{
		internal static GetHeadResponse GetHead(IAmazonS3 s3Client, IClientConfig config, string url, string header)
		{
			HttpWebRequest headHttpRequest = GetHeadHttpRequest(config, url);
			try
			{
				using (HttpWebResponse httpResponse = headHttpRequest.GetResponse() as HttpWebResponse)
				{
					return HandleWebResponse(header, httpResponse);
				}
			}
			catch (WebException we)
			{
				return HandleWebException(header, we);
			}
		}

		internal static HttpWebRequest GetHeadHttpRequest(IClientConfig config, string url)
		{
			HttpWebRequest httpWebRequest = WebRequest.Create(url) as HttpWebRequest;
			httpWebRequest.Method = "HEAD";
			SetProxyIfAvailableAndConfigured(config, httpWebRequest);
			return httpWebRequest;
		}

		private static GetHeadResponse HandleWebResponse(string header, HttpWebResponse httpResponse)
		{
			return new GetHeadResponse
			{
				HeaderValue = httpResponse.Headers[header],
				StatusCode = httpResponse.StatusCode
			};
		}

		private static GetHeadResponse HandleWebException(string header, WebException we)
		{
			using (HttpWebResponse httpWebResponse = we.Response as HttpWebResponse)
			{
				if (httpWebResponse == null)
				{
					return new GetHeadResponse();
				}
				return new GetHeadResponse
				{
					HeaderValue = httpWebResponse.Headers[header],
					StatusCode = httpWebResponse.StatusCode
				};
			}
		}

		private static void SetProxyIfAvailableAndConfigured(IClientConfig config, HttpWebRequest httpWebRequest)
		{
			IWebProxy proxyIfAvailableAndConfigured = GetProxyIfAvailableAndConfigured(config);
			if (proxyIfAvailableAndConfigured != null)
			{
				httpWebRequest.Proxy = proxyIfAvailableAndConfigured;
			}
		}

		private static IWebProxy GetProxyIfAvailableAndConfigured(IClientConfig config)
		{
			return config.GetWebProxy();
		}
	}
}
