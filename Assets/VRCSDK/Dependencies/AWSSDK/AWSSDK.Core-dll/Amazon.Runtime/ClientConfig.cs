using Amazon.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;

namespace Amazon.Runtime
{
	public abstract class ClientConfig : IClientConfig
	{
		internal static readonly TimeSpan InfiniteTimeout = TimeSpan.FromMilliseconds(-1.0);

		public static readonly TimeSpan MaxTimeout = TimeSpan.FromMilliseconds(2147483647.0);

		private RegionEndpoint regionEndpoint;

		private bool probeForRegionEndpoint = true;

		private bool throttleRetries = true;

		private bool useHttp;

		private string serviceURL;

		private string authRegion;

		private string authServiceName;

		private string signatureVersion = "2";

		private SigningAlgorithm signatureMethod = SigningAlgorithm.HmacSHA256;

		private int maxErrorRetry = 4;

		private bool readEntireResponse;

		private bool logResponse;

		private int bufferSize = 8192;

		private long progressUpdateInterval = 102400L;

		private bool resignRetries;

		private ICredentials proxyCredentials;

		private bool logMetrics = AWSConfigs.LoggingConfig.LogMetrics;

		private bool disableLogging;

		private TimeSpan? timeout;

		private bool allowAutoRedirect = true;

		private bool useDualstackEndpoint;

		private string proxyHost;

		private int proxyPort = -1;

		private List<string> proxyBypassList;

		private int? connectionLimit;

		private int? maxIdleTime;

		private bool useNagleAlgorithm;

		private TimeSpan? readWriteTimeout;

		public abstract string ServiceVersion
		{
			get;
		}

		public SigningAlgorithm SignatureMethod
		{
			get
			{
				return signatureMethod;
			}
			set
			{
				signatureMethod = value;
			}
		}

		public string SignatureVersion
		{
			get
			{
				return signatureVersion;
			}
			set
			{
				signatureVersion = value;
			}
		}

		public abstract string UserAgent
		{
			get;
		}

		public RegionEndpoint RegionEndpoint
		{
			get
			{
				return regionEndpoint;
			}
			set
			{
				serviceURL = null;
				regionEndpoint = value;
				probeForRegionEndpoint = false;
				if (regionEndpoint != null)
				{
					RegionEndpoint.Endpoint endpointForService = regionEndpoint.GetEndpointForService(RegionEndpointServiceName, UseDualstackEndpoint);
					if (endpointForService != null && endpointForService.SignatureVersionOverride != null)
					{
						SignatureVersion = endpointForService.SignatureVersionOverride;
					}
				}
			}
		}

		public abstract string RegionEndpointServiceName
		{
			get;
		}

		public string ServiceURL
		{
			get
			{
				return serviceURL;
			}
			set
			{
				regionEndpoint = null;
				probeForRegionEndpoint = false;
				serviceURL = value;
			}
		}

		public bool UseHttp
		{
			get
			{
				return useHttp;
			}
			set
			{
				useHttp = value;
			}
		}

		public string AuthenticationRegion
		{
			get
			{
				return authRegion;
			}
			set
			{
				authRegion = value;
			}
		}

		public string AuthenticationServiceName
		{
			get
			{
				return authServiceName;
			}
			set
			{
				authServiceName = value;
			}
		}

		public int MaxErrorRetry
		{
			get
			{
				return maxErrorRetry;
			}
			set
			{
				maxErrorRetry = value;
			}
		}

		public bool LogResponse
		{
			get
			{
				return logResponse;
			}
			set
			{
				logResponse = value;
			}
		}

		[Obsolete("This property does not effect response processing and is deprecated.To enable response logging, the ClientConfig.LogResponse and AWSConfigs.LoggingConfig.LogResponses properties can be used.")]
		public bool ReadEntireResponse
		{
			get
			{
				return readEntireResponse;
			}
			set
			{
				readEntireResponse = value;
			}
		}

		public int BufferSize
		{
			get
			{
				return bufferSize;
			}
			set
			{
				bufferSize = value;
			}
		}

		public long ProgressUpdateInterval
		{
			get
			{
				return progressUpdateInterval;
			}
			set
			{
				progressUpdateInterval = value;
			}
		}

		public bool ResignRetries
		{
			get
			{
				return resignRetries;
			}
			set
			{
				resignRetries = value;
			}
		}

		public bool AllowAutoRedirect
		{
			get
			{
				return allowAutoRedirect;
			}
			set
			{
				allowAutoRedirect = value;
			}
		}

		public bool LogMetrics
		{
			get
			{
				return logMetrics;
			}
			set
			{
				logMetrics = value;
			}
		}

		public bool DisableLogging
		{
			get
			{
				return disableLogging;
			}
			set
			{
				disableLogging = value;
			}
		}

		public ICredentials ProxyCredentials
		{
			get
			{
				if (proxyCredentials == null && (!string.IsNullOrEmpty(AWSConfigs.ProxyConfig.Username) || !string.IsNullOrEmpty(AWSConfigs.ProxyConfig.Password)))
				{
					return new NetworkCredential(AWSConfigs.ProxyConfig.Username, AWSConfigs.ProxyConfig.Password ?? string.Empty);
				}
				return proxyCredentials;
			}
			set
			{
				proxyCredentials = value;
			}
		}

		public TimeSpan? Timeout
		{
			get
			{
				return timeout;
			}
			set
			{
				ValidateTimeout(value);
				timeout = value;
			}
		}

		public bool UseDualstackEndpoint
		{
			get
			{
				return useDualstackEndpoint;
			}
			set
			{
				useDualstackEndpoint = value;
			}
		}

		public bool ThrottleRetries
		{
			get
			{
				return throttleRetries;
			}
			set
			{
				throttleRetries = value;
			}
		}

		public string ProxyHost
		{
			get
			{
				if (string.IsNullOrEmpty(proxyHost))
				{
					return AWSConfigs.ProxyConfig.Host;
				}
				return proxyHost;
			}
			set
			{
				proxyHost = value;
			}
		}

		public int ProxyPort
		{
			get
			{
				if (proxyPort <= 0)
				{
					return AWSConfigs.ProxyConfig.Port.GetValueOrDefault();
				}
				return proxyPort;
			}
			set
			{
				proxyPort = value;
			}
		}

		public List<string> ProxyBypassList
		{
			get
			{
				if (proxyBypassList == null)
				{
					return AWSConfigs.ProxyConfig.BypassList;
				}
				return proxyBypassList;
			}
			set
			{
				proxyBypassList = ((value != null) ? new List<string>(value) : null);
			}
		}

		public bool ProxyBypassOnLocal
		{
			get;
			set;
		}

		public int MaxIdleTime
		{
			get
			{
				return AWSSDKUtils.GetMaxIdleTime(maxIdleTime);
			}
			set
			{
				maxIdleTime = value;
			}
		}

		public int ConnectionLimit
		{
			get
			{
				return AWSSDKUtils.GetConnectionLimit(connectionLimit);
			}
			set
			{
				connectionLimit = value;
			}
		}

		public bool UseNagleAlgorithm
		{
			get
			{
				return useNagleAlgorithm;
			}
			set
			{
				useNagleAlgorithm = value;
			}
		}

		public TimeSpan? ReadWriteTimeout
		{
			get
			{
				return readWriteTimeout;
			}
			set
			{
				ValidateTimeout(value);
				readWriteTimeout = value;
			}
		}

		public string DetermineServiceURL()
		{
			if (ServiceURL != null)
			{
				return ServiceURL;
			}
			return GetUrl(RegionEndpoint, RegionEndpointServiceName, UseHttp, UseDualstackEndpoint);
		}

		internal static string GetUrl(RegionEndpoint regionEndpoint, string regionEndpointServiceName, bool useHttp, bool useDualStack)
		{
			RegionEndpoint.Endpoint endpointForService = regionEndpoint.GetEndpointForService(regionEndpointServiceName, useDualStack);
			return new Uri(string.Format(CultureInfo.InvariantCulture, "{0}{1}", useHttp ? "http://" : "https://", endpointForService.Hostname)).AbsoluteUri;
		}

		public ClientConfig()
		{
			Initialize();
		}

		protected virtual void Initialize()
		{
		}

		public void SetUseNagleIfAvailable(bool useNagle)
		{
		}

		public virtual void Validate()
		{
			if (RegionEndpoint == null && string.IsNullOrEmpty(ServiceURL))
			{
				throw new AmazonClientException("No RegionEndpoint or ServiceURL configured");
			}
		}

		public static void ValidateTimeout(TimeSpan? timeout)
		{
			if (!timeout.HasValue)
			{
				throw new ArgumentNullException("timeout");
			}
			if (timeout != InfiniteTimeout && (timeout <= TimeSpan.Zero || timeout > MaxTimeout))
			{
				throw new ArgumentOutOfRangeException("timeout");
			}
		}

		public static TimeSpan? GetTimeoutValue(TimeSpan? clientTimeout, TimeSpan? requestTimeout)
		{
			return requestTimeout ?? clientTimeout ?? null;
		}

		private static RegionEndpoint GetDefaultRegionEndpoint()
		{
			return FallbackRegionFactory.GetRegionEndpoint();
		}

		public WebProxy GetWebProxy()
		{
			WebProxy webProxy = null;
			if (!string.IsNullOrEmpty(ProxyHost) && ProxyPort > 0)
			{
				webProxy = new WebProxy(ProxyHost.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ? ProxyHost.Substring("http://".Length) : ProxyHost, ProxyPort);
				if (ProxyCredentials != null)
				{
					webProxy.Credentials = ProxyCredentials;
				}
				if (ProxyBypassList != null)
				{
					webProxy.BypassList = ProxyBypassList.ToArray();
				}
				webProxy.BypassProxyOnLocal = ProxyBypassOnLocal;
			}
			return webProxy;
		}

		public void SetWebProxy(WebProxy proxy)
		{
			if (proxy == null)
			{
				throw new ArgumentNullException("proxy");
			}
			Uri address = proxy.Address;
			ProxyHost = address.Host;
			ProxyPort = address.Port;
			ProxyBypassList = new List<string>(proxy.BypassList);
			ProxyBypassOnLocal = proxy.BypassProxyOnLocal;
			ProxyCredentials = proxy.Credentials;
		}
	}
}
