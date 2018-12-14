using Amazon.Runtime.Internal;
using System;
using System.Collections.Generic;

namespace Amazon.Runtime
{
	public class WebServiceExceptionEventArgs : ExceptionEventArgs
	{
		public IDictionary<string, string> Headers
		{
			get;
			protected set;
		}

		public IDictionary<string, string> Parameters
		{
			get;
			protected set;
		}

		public string ServiceName
		{
			get;
			protected set;
		}

		public Uri Endpoint
		{
			get;
			protected set;
		}

		public AmazonWebServiceRequest Request
		{
			get;
			protected set;
		}

		public Exception Exception
		{
			get;
			protected set;
		}

		protected WebServiceExceptionEventArgs()
		{
		}

		internal static WebServiceExceptionEventArgs Create(Exception exception, IRequest request)
		{
			if (request == null)
			{
				return new WebServiceExceptionEventArgs
				{
					Exception = exception
				};
			}
			return new WebServiceExceptionEventArgs
			{
				Headers = request.Headers,
				Parameters = request.Parameters,
				ServiceName = request.ServiceName,
				Request = request.OriginalRequest,
				Endpoint = request.Endpoint,
				Exception = exception
			};
		}
	}
}
