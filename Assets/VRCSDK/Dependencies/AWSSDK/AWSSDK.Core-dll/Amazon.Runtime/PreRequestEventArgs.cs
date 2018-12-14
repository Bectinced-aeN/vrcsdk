using System;

namespace Amazon.Runtime
{
	public class PreRequestEventArgs : EventArgs
	{
		public AmazonWebServiceRequest Request
		{
			get;
			protected set;
		}

		protected PreRequestEventArgs()
		{
		}

		internal static PreRequestEventArgs Create(AmazonWebServiceRequest request)
		{
			return new PreRequestEventArgs
			{
				Request = request
			};
		}
	}
}
