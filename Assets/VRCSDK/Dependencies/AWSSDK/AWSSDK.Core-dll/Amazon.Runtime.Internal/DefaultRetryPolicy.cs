using Amazon.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;

namespace Amazon.Runtime.Internal
{
	public class DefaultRetryPolicy : RetryPolicy
	{
		private const int THROTTLE_RETRY_REQUEST_COST = 5;

		private const int THROTTLED_RETRIES = 100;

		private const int THROTTLE_REQUEST_COST = 1;

		private static readonly CapacityManager _capacityManagerInstance = new CapacityManager(100, 5, 1);

		private int _maxBackoffInMilliseconds = (int)TimeSpan.FromSeconds(30.0).TotalMilliseconds;

		private RetryCapacity _retryCapacity;

		private ICollection<HttpStatusCode> _httpStatusCodesToRetryOn = new HashSet<HttpStatusCode>
		{
			HttpStatusCode.InternalServerError,
			HttpStatusCode.ServiceUnavailable,
			HttpStatusCode.BadGateway,
			HttpStatusCode.GatewayTimeout
		};

		private ICollection<WebExceptionStatus> _webExceptionStatusesToRetryOn = new HashSet<WebExceptionStatus>
		{
			WebExceptionStatus.ConnectFailure,
			WebExceptionStatus.ConnectionClosed,
			WebExceptionStatus.KeepAliveFailure,
			WebExceptionStatus.NameResolutionFailure,
			WebExceptionStatus.ReceiveFailure
		};

		private static readonly HashSet<string> _coreCLRRetryErrorMessages = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"The server returned an invalid or unrecognized response",
			"The connection with the server was terminated abnormally"
		};

		private ICollection<string> _errorCodesToRetryOn = new HashSet<string>
		{
			"Throttling",
			"ThrottlingException",
			"ProvisionedThroughputExceededException",
			"RequestTimeout"
		};

		public int MaxBackoffInMilliseconds
		{
			get
			{
				return _maxBackoffInMilliseconds;
			}
			set
			{
				_maxBackoffInMilliseconds = value;
			}
		}

		public ICollection<HttpStatusCode> HttpStatusCodesToRetryOn => _httpStatusCodesToRetryOn;

		public ICollection<string> ErrorCodesToRetryOn => _errorCodesToRetryOn;

		public ICollection<WebExceptionStatus> WebExceptionStatusesToRetryOn => _webExceptionStatusesToRetryOn;

		public DefaultRetryPolicy(int maxRetries)
		{
			base.MaxRetries = maxRetries;
		}

		public DefaultRetryPolicy(IClientConfig config)
		{
			base.MaxRetries = config.MaxErrorRetry;
			if (config.ThrottleRetries)
			{
				string serviceURL = config.DetermineServiceURL();
				_retryCapacity = _capacityManagerInstance.GetRetryCapacity(serviceURL);
			}
		}

		public override bool CanRetry(IExecutionContext executionContext)
		{
			return executionContext.RequestContext.Request.IsRequestStreamRewindable();
		}

		public override bool RetryForException(IExecutionContext executionContext, Exception exception)
		{
			return RetryForExceptionSync(exception);
		}

		public override bool OnRetry(IExecutionContext executionContext)
		{
			if (executionContext.RequestContext.ClientConfig.ThrottleRetries && _retryCapacity != null)
			{
				return _capacityManagerInstance.TryAcquireCapacity(_retryCapacity);
			}
			return true;
		}

		public override void NotifySuccess(IExecutionContext executionContext)
		{
			if (executionContext.RequestContext.ClientConfig.ThrottleRetries && _retryCapacity != null)
			{
				_capacityManagerInstance.TryReleaseCapacity((executionContext.RequestContext.Retries > 0) ? true : false, _retryCapacity);
			}
		}

		private bool RetryForExceptionSync(Exception exception)
		{
			if (exception is IOException)
			{
				if (IsInnerException<ThreadAbortException>(exception))
				{
					return false;
				}
				return true;
			}
			AmazonServiceException ex = exception as AmazonServiceException;
			if (ex != null)
			{
				if (HttpStatusCodesToRetryOn.Contains(ex.StatusCode))
				{
					return true;
				}
				if (ex.StatusCode == HttpStatusCode.BadRequest || ex.StatusCode == HttpStatusCode.ServiceUnavailable)
				{
					string errorCode = ex.ErrorCode;
					if (ErrorCodesToRetryOn.Contains(errorCode))
					{
						return true;
					}
				}
				if (IsInnerException(exception, out WebException inner) && WebExceptionStatusesToRetryOn.Contains(inner.Status))
				{
					return true;
				}
			}
			return false;
		}

		public override bool RetryLimitReached(IExecutionContext executionContext)
		{
			return executionContext.RequestContext.Retries >= base.MaxRetries;
		}

		public override void WaitBeforeRetry(IExecutionContext executionContext)
		{
			WaitBeforeRetry(executionContext.RequestContext.Retries, MaxBackoffInMilliseconds);
		}

		public static void WaitBeforeRetry(int retries, int maxBackoffInMilliseconds)
		{
			AWSSDKUtils.Sleep(CalculateRetryDelay(retries, maxBackoffInMilliseconds));
		}

		private static int CalculateRetryDelay(int retries, int maxBackoffInMilliseconds)
		{
			int num = (retries >= 12) ? 2147483647 : Convert.ToInt32(Math.Pow(4.0, (double)retries) * 100.0);
			if (retries > 0 && (num > maxBackoffInMilliseconds || num <= 0))
			{
				num = maxBackoffInMilliseconds;
			}
			return num;
		}

		protected static bool ContainErrorMessage(Exception exception)
		{
			if (exception == null)
			{
				return false;
			}
			if (_coreCLRRetryErrorMessages.Contains(exception.Message))
			{
				return true;
			}
			return ContainErrorMessage(exception.InnerException);
		}

		protected static bool IsInnerException<T>(Exception exception) where T : Exception
		{
			T inner;
			return IsInnerException(exception, out inner);
		}

		protected static bool IsInnerException<T>(Exception exception, out T inner) where T : Exception
		{
			inner = null;
			Exception ex = exception;
			while (ex.InnerException != null)
			{
				inner = (ex.InnerException as T);
				if (inner != null)
				{
					return true;
				}
				ex = ex.InnerException;
			}
			return false;
		}
	}
}
