using Amazon.Runtime.Internal.Util;
using Amazon.Util.Internal;
using System;
using System.Collections.Generic;

namespace Amazon.Runtime.Internal
{
	public class ErrorHandler : PipelineHandler
	{
		private IDictionary<Type, IExceptionHandler> _exceptionHandlers;

		public IDictionary<Type, IExceptionHandler> ExceptionHandlers => _exceptionHandlers;

		public ErrorHandler(ILogger logger)
		{
			Logger = logger;
			_exceptionHandlers = new Dictionary<Type, IExceptionHandler>
			{
				{
					typeof(HttpErrorResponseException),
					new HttpErrorResponseExceptionHandler(Logger)
				}
			};
		}

		public override void InvokeSync(IExecutionContext executionContext)
		{
			try
			{
				base.InvokeSync(executionContext);
			}
			catch (Exception exception)
			{
				DisposeReponse(executionContext.ResponseContext);
				if (ProcessException(executionContext, exception))
				{
					throw;
				}
			}
		}

		protected override void InvokeAsyncCallback(IAsyncExecutionContext executionContext)
		{
			IAsyncRequestContext requestContext = executionContext.RequestContext;
			IAsyncResponseContext responseContext = executionContext.ResponseContext;
			Exception exception = responseContext.AsyncResult.Exception;
			if (exception != null)
			{
				try
				{
					DisposeReponse(executionContext.ResponseContext);
					if (!ProcessException(ExecutionContext.CreateFromAsyncContext(executionContext), exception))
					{
						responseContext.AsyncResult.Exception = null;
					}
				}
				catch (Exception exception2)
				{
					responseContext.AsyncResult.Exception = exception2;
				}
			}
			base.InvokeAsyncCallback(executionContext);
		}

		private static void DisposeReponse(IResponseContext responseContext)
		{
			if (responseContext.HttpResponse != null && responseContext.HttpResponse.ResponseBody != null)
			{
				responseContext.HttpResponse.ResponseBody.Dispose();
			}
		}

		private bool ProcessException(IExecutionContext executionContext, Exception exception)
		{
			Logger.Error(exception, "An exception of type {0} was handled in ErrorHandler.", exception.GetType().Name);
			executionContext.RequestContext.Metrics.AddProperty(Metric.Exception, exception);
			Type type = exception.GetType();
			ITypeInfo typeInfo = TypeFactory.GetTypeInfo(exception.GetType());
			do
			{
				IExceptionHandler value = null;
				if (ExceptionHandlers.TryGetValue(type, out value))
				{
					return value.Handle(executionContext, exception);
				}
				type = typeInfo.BaseType;
				typeInfo = TypeFactory.GetTypeInfo(typeInfo.BaseType);
			}
			while (type != typeof(Exception));
			return true;
		}
	}
}
