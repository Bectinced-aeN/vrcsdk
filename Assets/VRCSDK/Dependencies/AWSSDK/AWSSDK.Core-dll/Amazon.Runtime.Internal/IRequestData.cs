using Amazon.Runtime.Internal.Auth;
using Amazon.Runtime.Internal.Transform;
using Amazon.Runtime.Internal.Util;

namespace Amazon.Runtime.Internal
{
	public interface IRequestData
	{
		ResponseUnmarshaller Unmarshaller
		{
			get;
		}

		RequestMetrics Metrics
		{
			get;
		}

		IRequest Request
		{
			get;
		}

		AbstractAWSSigner Signer
		{
			get;
		}

		int RetriesAttempt
		{
			get;
		}
	}
}
