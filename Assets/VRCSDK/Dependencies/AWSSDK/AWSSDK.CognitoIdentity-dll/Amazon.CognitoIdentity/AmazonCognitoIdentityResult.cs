using System;

namespace Amazon.CognitoIdentity
{
	public class AmazonCognitoIdentityResult<TResponse>
	{
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

		public object State
		{
			get;
			internal set;
		}

		public AmazonCognitoIdentityResult(object state)
		{
			State = state;
		}

		public AmazonCognitoIdentityResult(TResponse response, Exception exception, object state)
		{
			Response = response;
			Exception = exception;
			State = state;
		}
	}
}
