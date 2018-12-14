using System;

namespace Amazon.Runtime.Internal
{
	public class CallbackHandler : PipelineHandler
	{
		public Action<IExecutionContext> OnPreInvoke
		{
			get;
			set;
		}

		public Action<IExecutionContext> OnPostInvoke
		{
			get;
			set;
		}

		public override void InvokeSync(IExecutionContext executionContext)
		{
			PreInvoke(executionContext);
			base.InvokeSync(executionContext);
			PostInvoke(executionContext);
		}

		public override IAsyncResult InvokeAsync(IAsyncExecutionContext executionContext)
		{
			PreInvoke(ExecutionContext.CreateFromAsyncContext(executionContext));
			return base.InvokeAsync(executionContext);
		}

		protected override void InvokeAsyncCallback(IAsyncExecutionContext executionContext)
		{
			if (executionContext.ResponseContext.AsyncResult.Exception == null)
			{
				PostInvoke(ExecutionContext.CreateFromAsyncContext(executionContext));
			}
			base.InvokeAsyncCallback(executionContext);
		}

		protected void PreInvoke(IExecutionContext executionContext)
		{
			RaiseOnPreInvoke(executionContext);
		}

		protected void PostInvoke(IExecutionContext executionContext)
		{
			RaiseOnPostInvoke(executionContext);
		}

		private void RaiseOnPreInvoke(IExecutionContext context)
		{
			if (OnPreInvoke != null)
			{
				OnPreInvoke(context);
			}
		}

		private void RaiseOnPostInvoke(IExecutionContext context)
		{
			if (OnPostInvoke != null)
			{
				OnPostInvoke(context);
			}
		}
	}
}
