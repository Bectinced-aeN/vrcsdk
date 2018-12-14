using Amazon.Runtime.Internal.Transform;
using System;
using System.Net;

namespace Amazon.Runtime.Internal
{
	public class RedirectHandler : PipelineHandler
	{
		public override void InvokeSync(IExecutionContext executionContext)
		{
			do
			{
				base.InvokeSync(executionContext);
			}
			while (HandleRedirect(executionContext));
		}

		protected override void InvokeAsyncCallback(IAsyncExecutionContext executionContext)
		{
			if (executionContext.ResponseContext.AsyncResult.Exception == null && HandleRedirect(ExecutionContext.CreateFromAsyncContext(executionContext)))
			{
				base.InvokeAsync(executionContext);
			}
			else
			{
				base.InvokeAsyncCallback(executionContext);
			}
		}

		private bool HandleRedirect(IExecutionContext executionContext)
		{
			IWebResponseData httpResponse = executionContext.ResponseContext.HttpResponse;
			if (httpResponse.StatusCode >= HttpStatusCode.MultipleChoices && httpResponse.StatusCode < HttpStatusCode.BadRequest)
			{
				if (httpResponse.StatusCode == HttpStatusCode.TemporaryRedirect && httpResponse.IsHeaderPresent("location"))
				{
					IRequestContext requestContext = executionContext.RequestContext;
					string headerValue = httpResponse.GetHeaderValue("location");
					requestContext.Metrics.AddProperty(Metric.RedirectLocation, headerValue);
					if (executionContext.RequestContext.Request.IsRequestStreamRewindable() && !string.IsNullOrEmpty(headerValue))
					{
						FinalizeForRedirect(executionContext, headerValue);
						if (httpResponse.ResponseBody != null)
						{
							httpResponse.ResponseBody.Dispose();
						}
						return true;
					}
				}
				executionContext.ResponseContext.HttpResponse = null;
				throw new HttpErrorResponseException(httpResponse);
			}
			return false;
		}

		protected virtual void FinalizeForRedirect(IExecutionContext executionContext, string redirectedLocation)
		{
			Logger.InfoFormat("Request {0} is being redirected to {1}.", executionContext.RequestContext.RequestName, redirectedLocation);
			Uri uri = new Uri(redirectedLocation);
			executionContext.RequestContext.Request.Endpoint = new UriBuilder(uri.Scheme, uri.Host).Uri;
			RetryHandler.PrepareForRetry(executionContext.RequestContext);
		}
	}
}
