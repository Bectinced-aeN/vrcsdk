using System;

namespace Amazon.Runtime.Internal
{
	public class EndpointResolver : PipelineHandler
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

		protected void PreInvoke(IExecutionContext executionContext)
		{
			IRequestContext requestContext = executionContext.RequestContext;
			if (requestContext.Request.Endpoint == null)
			{
				requestContext.Request.Endpoint = DetermineEndpoint(executionContext.RequestContext);
			}
		}

		public virtual Uri DetermineEndpoint(IRequestContext requestContext)
		{
			return DetermineEndpoint(requestContext.ClientConfig, requestContext.Request);
		}

		public static Uri DetermineEndpoint(IClientConfig config, IRequest request)
		{
			if (request.AlternateEndpoint == null)
			{
				return new Uri(config.DetermineServiceURL());
			}
			return new Uri(ClientConfig.GetUrl(request.AlternateEndpoint, config.RegionEndpointServiceName, config.UseHttp, config.UseDualstackEndpoint));
		}
	}
}
