using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Amazon.Runtime.Internal.Util
{
	public class Logger : ILogger
	{
		private static IDictionary<Type, Logger> cachedLoggers = new Dictionary<Type, Logger>();

		private List<InternalLogger> loggers;

		private static Logger emptyLogger = new Logger();

		public static Logger EmptyLogger => emptyLogger;

		private Logger()
		{
			loggers = new List<InternalLogger>();
		}

		private Logger(Type type)
		{
			loggers = new List<InternalLogger>();
			InternalLog4netLogger item = new InternalLog4netLogger(type);
			loggers.Add(item);
			loggers.Add(new InternalConsoleLogger(type));
			UnityDebugLogger item2 = new UnityDebugLogger(type);
			loggers.Add(item2);
			ConfigureLoggers();
			AWSConfigs.PropertyChanged += ConfigsChanged;
		}

		private void ConfigsChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e != null && string.Equals(e.PropertyName, "LogTo", StringComparison.Ordinal))
			{
				ConfigureLoggers();
			}
		}

		private void ConfigureLoggers()
		{
			LoggingOptions logTo = AWSConfigs.LoggingConfig.LogTo;
			foreach (InternalLogger logger in loggers)
			{
				if (logger is InternalLog4netLogger)
				{
					logger.IsEnabled = ((logTo & LoggingOptions.Log4Net) == LoggingOptions.Log4Net);
				}
				if (logger is InternalConsoleLogger)
				{
					logger.IsEnabled = ((logTo & LoggingOptions.Console) == LoggingOptions.Console);
				}
				if (logger is UnityDebugLogger)
				{
					logger.IsEnabled = ((logTo & LoggingOptions.UnityLogger) == LoggingOptions.UnityLogger);
				}
			}
		}

		public static Logger GetLogger(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			lock (cachedLoggers)
			{
				if (cachedLoggers.TryGetValue(type, out Logger value))
				{
					return value;
				}
				value = new Logger(type);
				cachedLoggers[type] = value;
				return value;
			}
		}

		public static void ClearLoggerCache()
		{
			lock (cachedLoggers)
			{
				cachedLoggers = new Dictionary<Type, Logger>();
			}
		}

		public void Flush()
		{
			foreach (InternalLogger logger in loggers)
			{
				logger.Flush();
			}
		}

		public void Error(Exception exception, string messageFormat, params object[] args)
		{
			foreach (InternalLogger logger in loggers)
			{
				if (logger.IsEnabled && logger.IsErrorEnabled)
				{
					logger.Error(exception, messageFormat, args);
				}
			}
		}

		public void Debug(Exception exception, string messageFormat, params object[] args)
		{
			foreach (InternalLogger logger in loggers)
			{
				if (logger.IsEnabled && logger.IsDebugEnabled)
				{
					logger.Debug(exception, messageFormat, args);
				}
			}
		}

		public void DebugFormat(string messageFormat, params object[] args)
		{
			foreach (InternalLogger logger in loggers)
			{
				if (logger.IsEnabled && logger.IsDebugEnabled)
				{
					logger.DebugFormat(messageFormat, args);
				}
			}
		}

		public void InfoFormat(string messageFormat, params object[] args)
		{
			foreach (InternalLogger logger in loggers)
			{
				if (logger.IsEnabled && logger.IsInfoEnabled)
				{
					logger.InfoFormat(messageFormat, args);
				}
			}
		}
	}
}
