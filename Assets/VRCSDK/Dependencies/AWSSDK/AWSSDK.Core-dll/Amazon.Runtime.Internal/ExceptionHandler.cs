using Amazon.Runtime.Internal.Util;
using System;

namespace Amazon.Runtime.Internal
{
	public abstract class ExceptionHandler<T> : IExceptionHandler<T>, IExceptionHandler where T : Exception
	{
		private ILogger _logger;

		protected ILogger Logger => _logger;

		protected ExceptionHandler(ILogger logger)
		{
			_logger = logger;
		}

		public bool Handle(IExecutionContext executionContext, Exception exception)
		{
			return HandleException(executionContext, exception as T);
		}

		public abstract bool HandleException(IExecutionContext executionContext, T exception);
	}
}
