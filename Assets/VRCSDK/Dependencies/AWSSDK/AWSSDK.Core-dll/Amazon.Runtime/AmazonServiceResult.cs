using System;

namespace Amazon.Runtime
{
	public class AmazonServiceResult<TRequest, TResponse> where TRequest : AmazonWebServiceRequest where TResponse : AmazonWebServiceResponse
	{
		public TRequest Request
		{
			get;
			internal set;
		}

		public TResponse Response
		{
			get;
			internal set;
		}

		public Exception Exception
		{
			get;
			internal set;
		}

		public object state
		{
			get;
			internal set;
		}

		public AmazonServiceResult(TRequest request, TResponse response, Exception exception, object state)
		{
			Request = request;
			Response = response;
			Exception = exception;
			this.state = state;
		}
	}
}
