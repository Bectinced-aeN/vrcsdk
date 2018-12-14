namespace Amazon.Runtime.Internal
{
	public class AsyncResponseContext : ResponseContext, IAsyncResponseContext, IResponseContext
	{
		public RuntimeAsyncResult AsyncResult
		{
			get;
			set;
		}
	}
}
