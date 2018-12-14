using System;
using System.Globalization;

namespace Amazon.Runtime.Internal
{
	public class Marshaller : PipelineHandler
	{
		public override void InvokeSync(IExecutionContext executionContext)
		{
			PreInvoke(executionContext);
			base.InvokeSync(executionContext);
		}

		public override IAsyncResult InvokeAsync(IAsyncExecutionContext executionContext)
		{
			PreInvoke(ExecutionContext.CreateFromAsyncContext(executionContext));
			return base.InvokeAsync(executionContext);
		}

		protected static void PreInvoke(IExecutionContext executionContext)
		{
			IRequestContext requestContext = executionContext.RequestContext;
			requestContext.Request = requestContext.Marshaller.Marshall(requestContext.OriginalRequest);
			requestContext.Request.AuthenticationRegion = requestContext.ClientConfig.AuthenticationRegion;
			if (AWSConfigs.HttpClient == AWSConfigs.HttpClientOption.UnityWWW)
			{
				requestContext.Request.Headers["User-Agent"] = requestContext.ClientConfig.UserAgent + " " + (executionContext.RequestContext.IsAsync ? "ClientAsync" : "ClientSync") + " UnityWWW";
			}
			else
			{
				requestContext.Request.Headers["x-amz-user-agent"] = requestContext.ClientConfig.UserAgent + " " + (executionContext.RequestContext.IsAsync ? "ClientAsync" : "ClientSync") + " UnityWebRequest";
			}
			string a = requestContext.Request.HttpMethod.ToUpper(CultureInfo.InvariantCulture);
			if (a != "GET" && a != "DELETE" && a != "HEAD" && !requestContext.Request.Headers.ContainsKey("Content-Type"))
			{
				if (requestContext.Request.UseQueryString)
				{
					requestContext.Request.Headers["Content-Type"] = "application/x-amz-json-1.0";
				}
				else
				{
					requestContext.Request.Headers["Content-Type"] = "application/x-www-form-urlencoded; charset=utf-8";
				}
			}
		}
	}
}
