using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Util;
using System;
using System.Threading;

namespace Amazon.CognitoIdentity
{
	internal class CognitoIdentityAsyncExecutor
	{
		private static Logger Logger = Logger.GetLogger(typeof(CognitoIdentityAsyncExecutor));

		public static void ExecuteAsync<T>(Func<T> function, AsyncOptions options, AmazonCognitoIdentityCallback<T> callback)
		{
			ThreadPool.QueueUserWorkItem(delegate
			{
				T result = (T)default(T);
				Exception exception = null;
				try
				{
					result = (T)function();
				}
				catch (Exception ex)
				{
					Exception ex2 = exception = ex;
				}
				if (callback != null)
				{
					if (options.ExecuteCallbackOnMainThread)
					{
						UnityRequestQueue.Instance.ExecuteOnMainThread(delegate
						{
							callback(new AmazonCognitoIdentityResult<T>((T)result, exception, options.State));
						});
					}
					else
					{
						try
						{
							callback(new AmazonCognitoIdentityResult<T>((T)result, exception, options.State));
						}
						catch (Exception exception2)
						{
							Logger.Error(exception2, "An unhandled exception was thrown from the callback method {0}.", callback.Method.Name);
						}
					}
				}
			});
		}
	}
}
