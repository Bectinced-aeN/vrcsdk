using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using Amazon.Runtime.Internal.Util;
using Amazon.Util;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Amazon.Runtime
{
	public abstract class RetryPolicy
	{
		private static HashSet<string> clockSkewErrorCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"RequestTimeTooSkewed",
			"RequestExpired",
			"InvalidSignatureException",
			"SignatureDoesNotMatch",
			"AuthFailure",
			"RequestExpired",
			"RequestInTheFuture"
		};

		private const string clockSkewMessageFormat = "Identified clock skew: local time = {0}, local time with correction = {1}, current clock skew correction = {2}, server time = {3}.";

		private const string clockSkewUpdatedFormat = "Setting clock skew correction: new clock skew correction = {0}.";

		private const string clockSkewMessageParen = "(";

		private const string clockSkewMessagePlusSeparator = " + ";

		private const string clockSkewMessageMinusSeparator = " - ";

		private static TimeSpan clockSkewMaxThreshold = TimeSpan.FromMinutes(5.0);

		public int MaxRetries
		{
			get;
			protected set;
		}

		public ILogger Logger
		{
			get;
			set;
		}

		public bool Retry(IExecutionContext executionContext, Exception exception)
		{
			bool? flag = RetrySync(executionContext, exception);
			if ((!flag.HasValue) ? RetryForException(executionContext, exception) : flag.Value)
			{
				return OnRetry(executionContext);
			}
			return false;
		}

		public abstract bool CanRetry(IExecutionContext executionContext);

		public abstract bool RetryForException(IExecutionContext executionContext, Exception exception);

		public abstract bool RetryLimitReached(IExecutionContext executionContext);

		public abstract void WaitBeforeRetry(IExecutionContext executionContext);

		public virtual void NotifySuccess(IExecutionContext executionContext)
		{
		}

		public virtual bool OnRetry(IExecutionContext executionContext)
		{
			return true;
		}

		private bool? RetrySync(IExecutionContext executionContext, Exception exception)
		{
			if (!RetryLimitReached(executionContext) && CanRetry(executionContext))
			{
				if (IsClockskew(executionContext, exception))
				{
					return true;
				}
				return null;
			}
			return false;
		}

		private bool IsClockskew(IExecutionContext executionContext, Exception exception)
		{
			AmazonServiceException ex = exception as AmazonServiceException;
			bool num = executionContext.RequestContext.Request != null && string.Equals(executionContext.RequestContext.Request.HttpMethod, "HEAD", StringComparison.Ordinal);
			bool flag = ex != null && (ex.ErrorCode == null || clockSkewErrorCodes.Contains(ex.ErrorCode));
			if (num | flag)
			{
				DateTime utcNow = DateTime.UtcNow;
				DateTime correctedUtcNow = AWSSDKUtils.CorrectedUtcNow;
				DateTime serverTime;
				bool flag2 = TryParseDateHeader(ex, out serverTime);
				if (!flag2)
				{
					flag2 = TryParseExceptionMessage(ex, out serverTime);
				}
				if (flag2)
				{
					serverTime = serverTime.ToUniversalTime();
					TimeSpan timeSpan = correctedUtcNow - serverTime;
					if (((timeSpan.Ticks < 0) ? (-timeSpan) : timeSpan) > clockSkewMaxThreshold)
					{
						TimeSpan timeSpan2 = serverTime - utcNow;
						Logger.InfoFormat("Identified clock skew: local time = {0}, local time with correction = {1}, current clock skew correction = {2}, server time = {3}.", utcNow, correctedUtcNow, AWSConfigs.ClockOffset, serverTime);
						AWSConfigs.ClockOffset = timeSpan2;
						if (AWSConfigs.CorrectForClockSkew)
						{
							Logger.InfoFormat("Setting clock skew correction: new clock skew correction = {0}.", timeSpan2);
							executionContext.RequestContext.IsSigned = false;
							return true;
						}
					}
				}
			}
			return false;
		}

		private static bool TryParseDateHeader(AmazonServiceException ase, out DateTime serverTime)
		{
			IWebResponseData webData = GetWebData(ase);
			if (webData != null)
			{
				string headerValue = webData.GetHeaderValue("Date");
				if (!string.IsNullOrEmpty(headerValue) && DateTime.TryParseExact(headerValue, "ddd, dd MMM yyyy HH:mm:ss \\G\\M\\T", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out serverTime))
				{
					return true;
				}
			}
			serverTime = DateTime.MinValue;
			return false;
		}

		private static bool TryParseExceptionMessage(AmazonServiceException ase, out DateTime serverTime)
		{
			if (ase != null && !string.IsNullOrEmpty(ase.Message))
			{
				string message = ase.Message;
				int num = message.IndexOf("(", StringComparison.Ordinal);
				if (num >= 0)
				{
					num++;
					int num2 = message.IndexOf(" + ", num, StringComparison.Ordinal);
					if (num2 < 0)
					{
						num2 = message.IndexOf(" - ", num, StringComparison.Ordinal);
					}
					if (num2 > num && DateTime.TryParseExact(message.Substring(num, num2 - num), "yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out serverTime))
					{
						return true;
					}
				}
			}
			serverTime = DateTime.MinValue;
			return false;
		}

		private static IWebResponseData GetWebData(AmazonServiceException ase)
		{
			if (ase != null)
			{
				Exception ex = ase;
				do
				{
					HttpErrorResponseException ex2 = ex as HttpErrorResponseException;
					if (ex2 != null)
					{
						return ex2.Response;
					}
					ex = ex.InnerException;
				}
				while (ex != null);
			}
			return null;
		}
	}
}
