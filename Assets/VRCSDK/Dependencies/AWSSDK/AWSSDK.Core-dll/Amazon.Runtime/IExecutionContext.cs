namespace Amazon.Runtime
{
	public interface IExecutionContext
	{
		IResponseContext ResponseContext
		{
			get;
		}

		IRequestContext RequestContext
		{
			get;
		}
	}
}
