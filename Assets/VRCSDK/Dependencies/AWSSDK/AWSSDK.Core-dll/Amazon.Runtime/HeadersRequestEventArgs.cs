using System.Collections.Generic;

namespace Amazon.Runtime
{
	public class HeadersRequestEventArgs : RequestEventArgs
	{
		public IDictionary<string, string> Headers
		{
			get;
			protected set;
		}

		protected HeadersRequestEventArgs()
		{
		}

		internal static HeadersRequestEventArgs Create(IDictionary<string, string> headers)
		{
			return new HeadersRequestEventArgs
			{
				Headers = headers
			};
		}
	}
}
