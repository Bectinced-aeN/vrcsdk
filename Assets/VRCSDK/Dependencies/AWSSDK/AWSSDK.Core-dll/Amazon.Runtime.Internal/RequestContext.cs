using Amazon.Runtime.Internal.Auth;
using Amazon.Runtime.Internal.Transform;
using Amazon.Runtime.Internal.Util;

namespace Amazon.Runtime.Internal
{
	public class RequestContext : IRequestContext
	{
		public IRequest Request
		{
			get;
			set;
		}

		public RequestMetrics Metrics
		{
			get;
			private set;
		}

		public AbstractAWSSigner Signer
		{
			get;
			set;
		}

		public IClientConfig ClientConfig
		{
			get;
			set;
		}

		public int Retries
		{
			get;
			set;
		}

		public bool IsSigned
		{
			get;
			set;
		}

		public bool IsAsync
		{
			get;
			set;
		}

		public AmazonWebServiceRequest OriginalRequest
		{
			get;
			set;
		}

		public IMarshaller<IRequest, AmazonWebServiceRequest> Marshaller
		{
			get;
			set;
		}

		public ResponseUnmarshaller Unmarshaller
		{
			get;
			set;
		}

		public ImmutableCredentials ImmutableCredentials
		{
			get;
			set;
		}

		public string RequestName => OriginalRequest.GetType().Name;

		public RequestContext(bool enableMetrics)
		{
			Metrics = new RequestMetrics();
			Metrics.IsEnabled = enableMetrics;
		}
	}
}
