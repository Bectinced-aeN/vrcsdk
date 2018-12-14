using System;
using System.Net;

namespace Amazon.Runtime
{
	public interface IClientConfig
	{
		RegionEndpoint RegionEndpoint
		{
			get;
		}

		string RegionEndpointServiceName
		{
			get;
		}

		string ServiceURL
		{
			get;
		}

		bool UseHttp
		{
			get;
		}

		string ServiceVersion
		{
			get;
		}

		SigningAlgorithm SignatureMethod
		{
			get;
		}

		string SignatureVersion
		{
			get;
		}

		string AuthenticationRegion
		{
			get;
		}

		string AuthenticationServiceName
		{
			get;
		}

		string UserAgent
		{
			get;
		}

		bool DisableLogging
		{
			get;
		}

		bool LogMetrics
		{
			get;
		}

		bool LogResponse
		{
			get;
		}

		bool ReadEntireResponse
		{
			get;
		}

		bool AllowAutoRedirect
		{
			get;
		}

		int BufferSize
		{
			get;
		}

		int MaxErrorRetry
		{
			get;
		}

		long ProgressUpdateInterval
		{
			get;
		}

		bool ResignRetries
		{
			get;
		}

		ICredentials ProxyCredentials
		{
			get;
		}

		TimeSpan? Timeout
		{
			get;
		}

		bool UseDualstackEndpoint
		{
			get;
		}

		bool ThrottleRetries
		{
			get;
		}

		string ProxyHost
		{
			get;
		}

		int ProxyPort
		{
			get;
		}

		int MaxIdleTime
		{
			get;
		}

		TimeSpan? ReadWriteTimeout
		{
			get;
		}

		int ConnectionLimit
		{
			get;
		}

		bool UseNagleAlgorithm
		{
			get;
		}

		string DetermineServiceURL();

		void Validate();

		WebProxy GetWebProxy();
	}
}
