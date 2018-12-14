using Amazon.Runtime.Internal.Auth;
using System;
using System.Collections.Generic;
using System.IO;

namespace Amazon.Runtime.Internal
{
	public interface IRequest
	{
		string RequestName
		{
			get;
		}

		IDictionary<string, string> Headers
		{
			get;
		}

		bool UseQueryString
		{
			get;
			set;
		}

		IDictionary<string, string> Parameters
		{
			get;
		}

		IDictionary<string, string> SubResources
		{
			get;
		}

		string HttpMethod
		{
			get;
			set;
		}

		Uri Endpoint
		{
			get;
			set;
		}

		string ResourcePath
		{
			get;
			set;
		}

		byte[] Content
		{
			get;
			set;
		}

		bool SetContentFromParameters
		{
			get;
			set;
		}

		Stream ContentStream
		{
			get;
			set;
		}

		long OriginalStreamPosition
		{
			get;
			set;
		}

		string ServiceName
		{
			get;
		}

		AmazonWebServiceRequest OriginalRequest
		{
			get;
		}

		RegionEndpoint AlternateEndpoint
		{
			get;
			set;
		}

		bool Suppress404Exceptions
		{
			get;
			set;
		}

		AWS4SigningResult AWS4SignerResult
		{
			get;
			set;
		}

		bool UseChunkEncoding
		{
			get;
			set;
		}

		string CanonicalResourcePrefix
		{
			get;
			set;
		}

		bool UseSigV4
		{
			get;
			set;
		}

		string AuthenticationRegion
		{
			get;
			set;
		}

		void AddSubResource(string subResource);

		void AddSubResource(string subResource, string value);

		string ComputeContentStreamHash();

		bool IsRequestStreamRewindable();

		bool MayContainRequestBody();

		bool HasRequestBody();
	}
}
