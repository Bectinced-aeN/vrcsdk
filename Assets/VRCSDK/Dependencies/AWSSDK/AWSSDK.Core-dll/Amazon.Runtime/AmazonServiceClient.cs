using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Auth;
using Amazon.Runtime.Internal.Transform;
using Amazon.Runtime.Internal.Util;
using Amazon.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Amazon.Runtime
{
	public abstract class AmazonServiceClient : IDisposable
	{
		private bool _disposed;

		private Logger _logger;

		private PreRequestEventHandler mBeforeMarshallingEvent;

		private RequestEventHandler mBeforeRequestEvent;

		private ResponseEventHandler mAfterResponseEvent;

		private ExceptionEventHandler mExceptionEvent;

		protected RuntimePipeline RuntimePipeline
		{
			get;
			set;
		}

		protected internal AWSCredentials Credentials
		{
			get;
			private set;
		}

		public IClientConfig Config
		{
			get;
			private set;
		}

		protected virtual bool SupportResponseLogging => true;

		protected AbstractAWSSigner Signer
		{
			get;
			private set;
		}

		internal event PreRequestEventHandler BeforeMarshallingEvent
		{
			add
			{
				lock (this)
				{
					mBeforeMarshallingEvent = (PreRequestEventHandler)Delegate.Combine(mBeforeMarshallingEvent, value);
				}
			}
			remove
			{
				lock (this)
				{
					mBeforeMarshallingEvent = (PreRequestEventHandler)Delegate.Remove(mBeforeMarshallingEvent, value);
				}
			}
		}

		public event RequestEventHandler BeforeRequestEvent
		{
			add
			{
				lock (this)
				{
					mBeforeRequestEvent = (RequestEventHandler)Delegate.Combine(mBeforeRequestEvent, value);
				}
			}
			remove
			{
				lock (this)
				{
					mBeforeRequestEvent = (RequestEventHandler)Delegate.Remove(mBeforeRequestEvent, value);
				}
			}
		}

		public event ResponseEventHandler AfterResponseEvent
		{
			add
			{
				lock (this)
				{
					mAfterResponseEvent = (ResponseEventHandler)Delegate.Combine(mAfterResponseEvent, value);
				}
			}
			remove
			{
				lock (this)
				{
					mAfterResponseEvent = (ResponseEventHandler)Delegate.Remove(mAfterResponseEvent, value);
				}
			}
		}

		public event ExceptionEventHandler ExceptionEvent
		{
			add
			{
				lock (this)
				{
					mExceptionEvent = (ExceptionEventHandler)Delegate.Combine(mExceptionEvent, value);
				}
			}
			remove
			{
				lock (this)
				{
					mExceptionEvent = (ExceptionEventHandler)Delegate.Remove(mExceptionEvent, value);
				}
			}
		}

		protected AmazonServiceClient(AWSCredentials credentials, ClientConfig config)
		{
			if (config.DisableLogging)
			{
				_logger = Logger.EmptyLogger;
			}
			else
			{
				_logger = Logger.GetLogger(GetType());
			}
			config.Validate();
			Config = config;
			Credentials = credentials;
			Signer = CreateSigner();
			Initialize();
			BuildRuntimePipeline();
		}

		protected AmazonServiceClient(string awsAccessKeyId, string awsSecretAccessKey, string awsSessionToken, ClientConfig config)
			: this(new SessionAWSCredentials(awsAccessKeyId, awsSecretAccessKey, awsSessionToken), config)
		{
		}

		protected AmazonServiceClient(string awsAccessKeyId, string awsSecretAccessKey, ClientConfig config)
			: this(new BasicAWSCredentials(awsAccessKeyId, awsSecretAccessKey), config)
		{
		}

		protected virtual void Initialize()
		{
		}

		protected TResponse Invoke<TRequest, TResponse>(TRequest request, IMarshaller<IRequest, AmazonWebServiceRequest> marshaller, ResponseUnmarshaller unmarshaller) where TRequest : AmazonWebServiceRequest where TResponse : AmazonWebServiceResponse
		{
			ThrowIfDisposed();
			ExecutionContext executionContext = new ExecutionContext(new RequestContext(Config.LogMetrics)
			{
				ClientConfig = Config,
				Marshaller = marshaller,
				OriginalRequest = request,
				Signer = Signer,
				Unmarshaller = unmarshaller,
				IsAsync = false
			}, new ResponseContext());
			return (TResponse)RuntimePipeline.InvokeSync(executionContext).Response;
		}

		protected IAsyncResult BeginInvoke<TRequest>(TRequest request, IMarshaller<IRequest, AmazonWebServiceRequest> marshaller, ResponseUnmarshaller unmarshaller, AsyncOptions asyncOptions, Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper) where TRequest : AmazonWebServiceRequest
		{
			ThrowIfDisposed();
			asyncOptions = (asyncOptions ?? new AsyncOptions());
			AsyncExecutionContext executionContext = new AsyncExecutionContext(new AsyncRequestContext(Config.LogMetrics)
			{
				ClientConfig = Config,
				Marshaller = marshaller,
				OriginalRequest = request,
				Signer = Signer,
				Unmarshaller = unmarshaller,
				Action = callbackHelper,
				AsyncOptions = asyncOptions,
				IsAsync = true
			}, new AsyncResponseContext());
			return RuntimePipeline.InvokeAsync(executionContext);
		}

		protected IAsyncResult BeginInvoke<TRequest>(TRequest request, IMarshaller<IRequest, AmazonWebServiceRequest> marshaller, ResponseUnmarshaller unmarshaller, AsyncCallback callback, object state) where TRequest : AmazonWebServiceRequest
		{
			ThrowIfDisposed();
			AsyncExecutionContext executionContext = new AsyncExecutionContext(new AsyncRequestContext(Config.LogMetrics)
			{
				ClientConfig = Config,
				Marshaller = marshaller,
				OriginalRequest = request,
				Signer = Signer,
				Unmarshaller = unmarshaller,
				Callback = callback,
				State = state,
				IsAsync = true
			}, new AsyncResponseContext());
			return RuntimePipeline.InvokeAsync(executionContext);
		}

		protected static TResponse EndInvoke<TResponse>(IAsyncResult result) where TResponse : AmazonWebServiceResponse
		{
			if (result == null)
			{
				throw new ArgumentNullException("result", "Parameter result cannot be null.");
			}
			RuntimeAsyncResult runtimeAsyncResult = result as RuntimeAsyncResult;
			if (runtimeAsyncResult == null)
			{
				throw new ArgumentOutOfRangeException("result", "Parameter result is not of type RuntimeAsyncResult.");
			}
			using (runtimeAsyncResult)
			{
				if (!runtimeAsyncResult.IsCompleted)
				{
					runtimeAsyncResult.AsyncWaitHandle.WaitOne();
				}
				if (runtimeAsyncResult.Exception != null)
				{
					AWSSDKUtils.PreserveStackTrace(runtimeAsyncResult.Exception);
					throw runtimeAsyncResult.Exception;
				}
				return (TResponse)runtimeAsyncResult.Response;
			}
		}

		protected void ProcessPreRequestHandlers(IExecutionContext executionContext)
		{
			if (mBeforeMarshallingEvent != null)
			{
				PreRequestEventArgs e = PreRequestEventArgs.Create(executionContext.RequestContext.OriginalRequest);
				mBeforeMarshallingEvent(this, e);
			}
		}

		protected void ProcessRequestHandlers(IExecutionContext executionContext)
		{
			IRequest request = executionContext.RequestContext.Request;
			WebServiceRequestEventArgs webServiceRequestEventArgs = WebServiceRequestEventArgs.Create(request);
			if (request.OriginalRequest != null)
			{
				request.OriginalRequest.FireBeforeRequestEvent(this, webServiceRequestEventArgs);
			}
			if (mBeforeRequestEvent != null)
			{
				mBeforeRequestEvent(this, webServiceRequestEventArgs);
			}
		}

		protected void ProcessResponseHandlers(IExecutionContext executionContext)
		{
			if (mAfterResponseEvent != null)
			{
				WebServiceResponseEventArgs e = WebServiceResponseEventArgs.Create(executionContext.ResponseContext.Response, executionContext.RequestContext.Request, executionContext.ResponseContext.HttpResponse);
				mAfterResponseEvent(this, e);
			}
		}

		protected virtual void ProcessExceptionHandlers(IExecutionContext executionContext, Exception exception)
		{
			if (mExceptionEvent != null)
			{
				WebServiceExceptionEventArgs e = WebServiceExceptionEventArgs.Create(exception, executionContext.RequestContext.Request);
				mExceptionEvent(this, e);
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed && disposing)
			{
				if (RuntimePipeline != null)
				{
					RuntimePipeline.Dispose();
				}
				_disposed = true;
			}
		}

		private void ThrowIfDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().FullName);
			}
		}

		protected abstract AbstractAWSSigner CreateSigner();

		protected virtual void CustomizeRuntimePipeline(RuntimePipeline pipeline)
		{
		}

		private void BuildRuntimePipeline()
		{
			HttpHandler<string> httpHandler = null;
			httpHandler = ((AWSConfigs.HttpClient != 0) ? new HttpHandler<string>(new UnityWebRequestFactory(), this) : new HttpHandler<string>(new UnityWwwRequestFactory(), this));
			CallbackHandler callbackHandler = new CallbackHandler();
			callbackHandler.OnPreInvoke = ProcessPreRequestHandlers;
			CallbackHandler callbackHandler2 = new CallbackHandler();
			callbackHandler2.OnPreInvoke = ProcessRequestHandlers;
			CallbackHandler callbackHandler3 = new CallbackHandler();
			callbackHandler3.OnPostInvoke = ProcessResponseHandlers;
			ErrorCallbackHandler errorCallbackHandler = new ErrorCallbackHandler();
			errorCallbackHandler.OnError = ProcessExceptionHandlers;
			RuntimePipeline = new RuntimePipeline(new List<IPipelineHandler>
			{
				httpHandler,
				new Unmarshaller(SupportResponseLogging),
				new ErrorHandler(_logger),
				callbackHandler3,
				new Signer(),
				new CredentialsRetriever(Credentials),
				new RetryHandler(new DefaultRetryPolicy(Config)),
				callbackHandler2,
				new EndpointResolver(),
				new Marshaller(),
				callbackHandler,
				errorCallbackHandler,
				new MetricsHandler(),
				new ThreadPoolExecutionHandler(10)
			}, _logger);
			CustomizeRuntimePipeline(RuntimePipeline);
		}

		public static Uri ComposeUrl(IRequest iRequest)
		{
			Uri endpoint = iRequest.Endpoint;
			string text = iRequest.ResourcePath;
			if (text == null)
			{
				text = string.Empty;
			}
			else if (text.StartsWith("/", StringComparison.Ordinal))
			{
				text = text.Substring(1);
			}
			string arg = "?";
			StringBuilder stringBuilder = new StringBuilder();
			if (iRequest.SubResources.Count > 0)
			{
				foreach (KeyValuePair<string, string> subResource in iRequest.SubResources)
				{
					stringBuilder.AppendFormat("{0}{1}", arg, subResource.Key);
					if (subResource.Value != null)
					{
						stringBuilder.AppendFormat("={0}", subResource.Value);
					}
					arg = "&";
				}
			}
			if (iRequest.UseQueryString && iRequest.Parameters.Count > 0)
			{
				string parametersAsString = AWSSDKUtils.GetParametersAsString(iRequest.Parameters);
				stringBuilder.AppendFormat("{0}{1}", arg, parametersAsString);
			}
			if (AWSSDKUtils.HasBidiControlCharacters(text))
			{
				throw new AmazonClientException(string.Format(CultureInfo.InvariantCulture, "Target resource path [{0}] has bidirectional characters, which are not supportedby System.Uri and thus cannot be handled by the .NET SDK.", text));
			}
			string str = AWSSDKUtils.UrlEncode(text, path: true) + stringBuilder;
			Uri uri = new Uri(endpoint.AbsoluteUri + str);
			DontUnescapePathDotsAndSlashes(uri);
			return uri;
		}

		private static void DontUnescapePathDotsAndSlashes(Uri uri)
		{
		}

		internal C CloneConfig<C>() where C : ClientConfig, new()
		{
			C val = new C();
			CloneConfig(val);
			return val;
		}

		internal void CloneConfig(ClientConfig newConfig)
		{
			if (!string.IsNullOrEmpty(Config.ServiceURL))
			{
				RegionEndpoint regionEndpoint = newConfig.RegionEndpoint = RegionEndpoint.GetBySystemName(AWSSDKUtils.DetermineRegion(Config.ServiceURL));
			}
			else
			{
				newConfig.RegionEndpoint = Config.RegionEndpoint;
			}
			newConfig.UseHttp = Config.UseHttp;
			newConfig.ProxyCredentials = Config.ProxyCredentials;
		}
	}
}
