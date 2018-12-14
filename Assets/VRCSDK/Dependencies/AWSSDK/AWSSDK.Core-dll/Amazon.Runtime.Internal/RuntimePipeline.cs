using Amazon.Runtime.Internal.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Amazon.Runtime.Internal
{
	public class RuntimePipeline : IDisposable
	{
		private bool _disposed;

		private ILogger _logger;

		private IPipelineHandler _handler;

		public IPipelineHandler Handler => _handler;

		public List<IPipelineHandler> Handlers => EnumerateHandlers().ToList();

		public RuntimePipeline(IPipelineHandler handler)
		{
			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}
			Logger logger = Logger.GetLogger(typeof(RuntimePipeline));
			_handler = handler;
			_handler.Logger = logger;
			_logger = logger;
		}

		public RuntimePipeline(IList<IPipelineHandler> handlers)
			: this(handlers, Logger.GetLogger(typeof(RuntimePipeline)))
		{
		}

		public RuntimePipeline(IList<IPipelineHandler> handlers, ILogger logger)
		{
			if (handlers == null || handlers.Count == 0)
			{
				throw new ArgumentNullException("handlers");
			}
			if (logger == null)
			{
				throw new ArgumentNullException("logger");
			}
			_logger = logger;
			foreach (IPipelineHandler handler in handlers)
			{
				AddHandler(handler);
			}
		}

		public RuntimePipeline(IPipelineHandler handler, ILogger logger)
		{
			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}
			if (logger == null)
			{
				throw new ArgumentNullException("logger");
			}
			_handler = handler;
			_handler.Logger = logger;
			_logger = logger;
		}

		public IResponseContext InvokeSync(IExecutionContext executionContext)
		{
			ThrowIfDisposed();
			_handler.InvokeSync(executionContext);
			return executionContext.ResponseContext;
		}

		public IAsyncResult InvokeAsync(IAsyncExecutionContext executionContext)
		{
			ThrowIfDisposed();
			return _handler.InvokeAsync(executionContext);
		}

		public void AddHandler(IPipelineHandler handler)
		{
			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}
			ThrowIfDisposed();
			IPipelineHandler innermostHandler = GetInnermostHandler(handler);
			if (_handler != null)
			{
				innermostHandler.InnerHandler = _handler;
				_handler.OuterHandler = innermostHandler;
			}
			_handler = handler;
			SetHandlerProperties(handler);
		}

		public void AddHandlerAfter<T>(IPipelineHandler handler) where T : IPipelineHandler
		{
			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}
			ThrowIfDisposed();
			Type typeFromHandle = typeof(T);
			for (IPipelineHandler pipelineHandler = _handler; pipelineHandler != null; pipelineHandler = pipelineHandler.InnerHandler)
			{
				if (pipelineHandler.GetType() == typeFromHandle)
				{
					InsertHandler(handler, pipelineHandler);
					SetHandlerProperties(handler);
					return;
				}
			}
			throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Cannot find a handler of type {0}", typeFromHandle.Name));
		}

		public void AddHandlerBefore<T>(IPipelineHandler handler) where T : IPipelineHandler
		{
			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}
			ThrowIfDisposed();
			Type typeFromHandle = typeof(T);
			if (_handler.GetType() == typeFromHandle)
			{
				AddHandler(handler);
				SetHandlerProperties(handler);
				return;
			}
			for (IPipelineHandler pipelineHandler = _handler; pipelineHandler != null; pipelineHandler = pipelineHandler.InnerHandler)
			{
				if (pipelineHandler.InnerHandler != null && pipelineHandler.InnerHandler.GetType() == typeFromHandle)
				{
					InsertHandler(handler, pipelineHandler);
					SetHandlerProperties(handler);
					return;
				}
			}
			throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Cannot find a handler of type {0}", typeFromHandle.Name));
		}

		public void RemoveHandler<T>()
		{
			ThrowIfDisposed();
			Type typeFromHandle = typeof(T);
			for (IPipelineHandler pipelineHandler = _handler; pipelineHandler != null; pipelineHandler = pipelineHandler.InnerHandler)
			{
				if (pipelineHandler.GetType() == typeFromHandle)
				{
					if (pipelineHandler == _handler && _handler.InnerHandler == null)
					{
						throw new InvalidOperationException("The pipeline contains a single handler, cannot remove the only handler in the pipeline.");
					}
					if (pipelineHandler == _handler)
					{
						_handler = pipelineHandler.InnerHandler;
					}
					if (pipelineHandler.OuterHandler != null)
					{
						pipelineHandler.OuterHandler.InnerHandler = pipelineHandler.InnerHandler;
					}
					if (pipelineHandler.InnerHandler != null)
					{
						pipelineHandler.InnerHandler.OuterHandler = pipelineHandler.OuterHandler;
					}
					pipelineHandler.InnerHandler = null;
					pipelineHandler.OuterHandler = null;
					return;
				}
			}
			throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Cannot find a handler of type {0}", typeFromHandle.Name));
		}

		public void ReplaceHandler<T>(IPipelineHandler handler) where T : IPipelineHandler
		{
			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}
			ThrowIfDisposed();
			Type typeFromHandle = typeof(T);
			IPipelineHandler pipelineHandler = null;
			for (IPipelineHandler pipelineHandler2 = _handler; pipelineHandler2 != null; pipelineHandler2 = pipelineHandler2.InnerHandler)
			{
				if (pipelineHandler2.GetType() == typeFromHandle)
				{
					handler.InnerHandler = pipelineHandler2.InnerHandler;
					handler.OuterHandler = pipelineHandler2.OuterHandler;
					if (pipelineHandler != null)
					{
						pipelineHandler.InnerHandler = handler;
					}
					else
					{
						_handler = handler;
					}
					if (pipelineHandler2.InnerHandler != null)
					{
						pipelineHandler2.InnerHandler.OuterHandler = handler;
					}
					pipelineHandler2.InnerHandler = null;
					pipelineHandler2.OuterHandler = null;
					SetHandlerProperties(handler);
					return;
				}
				pipelineHandler = pipelineHandler2;
			}
			throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Cannot find a handler of type {0}", typeFromHandle.Name));
		}

		private static void InsertHandler(IPipelineHandler handler, IPipelineHandler current)
		{
			IPipelineHandler innerHandler = current.InnerHandler;
			current.InnerHandler = handler;
			handler.OuterHandler = current;
			if (innerHandler != null)
			{
				IPipelineHandler innermostHandler = GetInnermostHandler(handler);
				innermostHandler.InnerHandler = innerHandler;
				innerHandler.OuterHandler = innermostHandler;
			}
		}

		private static IPipelineHandler GetInnermostHandler(IPipelineHandler handler)
		{
			IPipelineHandler pipelineHandler = handler;
			while (pipelineHandler.InnerHandler != null)
			{
				pipelineHandler = pipelineHandler.InnerHandler;
			}
			return pipelineHandler;
		}

		private void SetHandlerProperties(IPipelineHandler handler)
		{
			ThrowIfDisposed();
			handler.Logger = _logger;
		}

		public IEnumerable<IPipelineHandler> EnumerateHandlers()
		{
			for (IPipelineHandler handler = Handler; handler != null; handler = handler.InnerHandler)
			{
				yield return handler;
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
				IPipelineHandler innerHandler;
				for (IPipelineHandler pipelineHandler = Handler; pipelineHandler != null; pipelineHandler = innerHandler)
				{
					innerHandler = pipelineHandler.InnerHandler;
					(pipelineHandler as IDisposable)?.Dispose();
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
	}
}
