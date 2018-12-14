using Amazon.Runtime.Internal.Util;
using Amazon.Util;
using System;

namespace Amazon.Runtime.Internal.Auth
{
	public abstract class AbstractAWSSigner
	{
		private AWS4Signer _aws4Signer;

		private AWS4Signer AWS4SignerInstance
		{
			get
			{
				if (_aws4Signer == null)
				{
					lock (this)
					{
						if (_aws4Signer == null)
						{
							_aws4Signer = new AWS4Signer();
						}
					}
				}
				return _aws4Signer;
			}
		}

		public abstract ClientProtocol Protocol
		{
			get;
		}

		protected static string ComputeHash(string data, string secretkey, SigningAlgorithm algorithm)
		{
			try
			{
				return CryptoUtilFactory.CryptoInstance.HMACSign(data, secretkey, algorithm);
			}
			catch (Exception ex)
			{
				throw new Amazon.Runtime.SignatureException("Failed to generate signature: " + ex.Message, ex);
			}
		}

		protected static string ComputeHash(byte[] data, string secretkey, SigningAlgorithm algorithm)
		{
			try
			{
				return CryptoUtilFactory.CryptoInstance.HMACSign(data, secretkey, algorithm);
			}
			catch (Exception ex)
			{
				throw new Amazon.Runtime.SignatureException("Failed to generate signature: " + ex.Message, ex);
			}
		}

		public abstract void Sign(IRequest request, IClientConfig clientConfig, RequestMetrics metrics, string awsAccessKeyId, string awsSecretAccessKey);

		protected static bool UseV4Signing(bool useSigV4Setting, IRequest request, IClientConfig config)
		{
			if (useSigV4Setting || request.UseSigV4 || config.SignatureVersion == "4")
			{
				return true;
			}
			RegionEndpoint regionEndpoint = null;
			if (!string.IsNullOrEmpty(request.AuthenticationRegion))
			{
				regionEndpoint = RegionEndpoint.GetBySystemName(request.AuthenticationRegion);
			}
			if (regionEndpoint == null && !string.IsNullOrEmpty(config.ServiceURL))
			{
				string text = AWSSDKUtils.DetermineRegion(config.ServiceURL);
				if (!string.IsNullOrEmpty(text))
				{
					regionEndpoint = RegionEndpoint.GetBySystemName(text);
				}
			}
			if (regionEndpoint == null && config.RegionEndpoint != null)
			{
				regionEndpoint = config.RegionEndpoint;
			}
			if (regionEndpoint != null)
			{
				RegionEndpoint.Endpoint endpointForService = regionEndpoint.GetEndpointForService(config.RegionEndpointServiceName, config.UseDualstackEndpoint);
				if (endpointForService != null && (endpointForService.SignatureVersionOverride == "4" || string.IsNullOrEmpty(endpointForService.SignatureVersionOverride)))
				{
					return true;
				}
			}
			return false;
		}

		protected AbstractAWSSigner SelectSigner(IRequest request, IClientConfig config)
		{
			return SelectSigner(this, useSigV4Setting: false, request, config);
		}

		protected AbstractAWSSigner SelectSigner(AbstractAWSSigner defaultSigner, bool useSigV4Setting, IRequest request, IClientConfig config)
		{
			if (UseV4Signing(useSigV4Setting, request, config))
			{
				return AWS4SignerInstance;
			}
			return defaultSigner;
		}
	}
}
