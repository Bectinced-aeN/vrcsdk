using System;

namespace Amazon.Runtime
{
	public interface IExceptionHandler
	{
		bool Handle(IExecutionContext executionContext, Exception exception);
	}
	public interface IExceptionHandler<T> : IExceptionHandler where T : Exception
	{
		bool HandleException(IExecutionContext executionContext, T exception);
	}
}
