using Amazon.Runtime.Internal;

namespace Amazon.Runtime
{
	public interface IAsyncResponseContext : IResponseContext
	{
		RuntimeAsyncResult AsyncResult
		{
			get;
			set;
		}
	}
}
