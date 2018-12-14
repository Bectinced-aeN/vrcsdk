using Amazon.Runtime.Internal.Auth;
using System;
using System.IO;

namespace Amazon.Runtime.Internal
{
	public class Signer : PipelineHandler
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
			if (ShouldSign(executionContext.RequestContext))
			{
				SignRequest(executionContext.RequestContext);
				executionContext.RequestContext.IsSigned = true;
			}
		}

		private static bool ShouldSign(IRequestContext requestContext)
		{
			if (requestContext.IsSigned)
			{
				return requestContext.ClientConfig.ResignRetries;
			}
			return true;
		}

		public static void SignRequest(IRequestContext requestContext)
		{
			ImmutableCredentials immutableCredentials = requestContext.ImmutableCredentials;
			if (immutableCredentials != null)
			{
				using (requestContext.Metrics.StartEvent(Metric.RequestSigningTime))
				{
					if (immutableCredentials.UseToken)
					{
						switch (requestContext.Signer.Protocol)
						{
						case ClientProtocol.QueryStringProtocol:
							requestContext.Request.Parameters["SecurityToken"] = immutableCredentials.Token;
							break;
						case ClientProtocol.RestProtocol:
							requestContext.Request.Headers["x-amz-security-token"] = immutableCredentials.Token;
							break;
						default:
							throw new InvalidDataException("Cannot determine protocol");
						}
					}
					requestContext.Signer.Sign(requestContext.Request, requestContext.ClientConfig, requestContext.Metrics, immutableCredentials.AccessKey, immutableCredentials.SecretKey);
				}
			}
		}
	}
}
