using Amazon.Runtime.Internal.Util;
using System;

namespace Amazon.Runtime
{
	public interface IPipelineHandler
	{
		ILogger Logger
		{
			get;
			set;
		}

		IPipelineHandler InnerHandler
		{
			get;
			set;
		}

		IPipelineHandler OuterHandler
		{
			get;
			set;
		}

		void InvokeSync(IExecutionContext executionContext);

		IAsyncResult InvokeAsync(IAsyncExecutionContext executionContext);

		void AsyncCallback(IAsyncExecutionContext executionContext);
	}
}
