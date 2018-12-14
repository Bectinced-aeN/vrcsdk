using Amazon.Runtime.Internal.Util;
using System;
using System.IO;
using System.Net;

namespace Amazon.Runtime.Internal
{
	public class RetryHandler : PipelineHandler
	{
		private ILogger _logger;

		public override ILogger Logger
		{
			get
			{
				return _logger;
			}
			set
			{
				_logger = value;
				RetryPolicy.Logger = value;
			}
		}

		public RetryPolicy RetryPolicy
		{
			get;
			private set;
		}

		public RetryHandler(RetryPolicy retryPolicy)
		{
			RetryPolicy = retryPolicy;
		}

		public override void InvokeSync(IExecutionContext executionContext)
		{
			IRequestContext requestContext = executionContext.RequestContext;
			IResponseContext responseContext = executionContext.ResponseContext;
			bool flag = false;
			do
			{
				try
				{
					base.InvokeSync(executionContext);
					RetryPolicy.NotifySuccess(executionContext);
					return;
				}
				catch (Exception exception)
				{
					flag = RetryPolicy.Retry(executionContext, exception);
					if (!flag)
					{
						LogForError(requestContext, exception);
						throw;
					}
					requestContext.Retries++;
					requestContext.Metrics.SetCounter(Metric.AttemptCount, requestContext.Retries);
					LogForRetry(requestContext, exception);
				}
				PrepareForRetry(requestContext);
				using (requestContext.Metrics.StartEvent(Metric.RetryPauseTime))
				{
					RetryPolicy.WaitBeforeRetry(executionContext);
				}
			}
			while (flag);
		}

		protected override void InvokeAsyncCallback(IAsyncExecutionContext executionContext)
		{
			IAsyncRequestContext requestContext = executionContext.RequestContext;
			IAsyncResponseContext responseContext = executionContext.ResponseContext;
			Exception exception = responseContext.AsyncResult.Exception;
			IExecutionContext executionContext2 = ExecutionContext.CreateFromAsyncContext(executionContext);
			if (exception != null)
			{
				if (RetryPolicy.Retry(executionContext2, exception))
				{
					requestContext.Retries++;
					requestContext.Metrics.SetCounter(Metric.AttemptCount, requestContext.Retries);
					LogForRetry(requestContext, exception);
					PrepareForRetry(requestContext);
					responseContext.AsyncResult.Exception = null;
					using (requestContext.Metrics.StartEvent(Metric.RetryPauseTime))
					{
						RetryPolicy.WaitBeforeRetry(executionContext2);
					}
					InvokeAsync(executionContext);
					return;
				}
				LogForError(requestContext, exception);
			}
			else
			{
				RetryPolicy.NotifySuccess(executionContext2);
			}
			base.InvokeAsyncCallback(executionContext);
		}

		internal static void PrepareForRetry(IRequestContext requestContext)
		{
			if (requestContext.Request.ContentStream != null && requestContext.Request.OriginalStreamPosition >= 0)
			{
				Stream stream = requestContext.Request.ContentStream;
				HashStream hashStream = stream as HashStream;
				if (hashStream != null)
				{
					hashStream.Reset();
					stream = hashStream.GetSeekableBaseStream();
				}
				stream.Position = requestContext.Request.OriginalStreamPosition;
			}
		}

		private void LogForRetry(IRequestContext requestContext, Exception exception)
		{
			WebException ex = exception as WebException;
			if (ex != null)
			{
				Logger.InfoFormat("WebException ({1}) making request {2} to {3}. Attempting retry {4} of {5}.", ex.Status, requestContext.RequestName, requestContext.Request.Endpoint.ToString(), requestContext.Retries, RetryPolicy.MaxRetries);
			}
			else
			{
				Logger.InfoFormat("{0} making request {1} to {2}. Attempting retry {3} of {4}.", exception.GetType().Name, requestContext.RequestName, requestContext.Request.Endpoint.ToString(), requestContext.Retries, RetryPolicy.MaxRetries);
			}
		}

		private void LogForError(IRequestContext requestContext, Exception exception)
		{
			Logger.Error(exception, "{0} making request {1} to {2}. Attempt {3}.", exception.GetType().Name, requestContext.RequestName, requestContext.Request.Endpoint.ToString(), requestContext.Retries + 1);
		}
	}
}
