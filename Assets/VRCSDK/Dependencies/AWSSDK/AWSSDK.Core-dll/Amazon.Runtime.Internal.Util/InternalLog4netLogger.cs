using Amazon.Util.Internal;
using System;
using System.Globalization;
using System.Reflection;

namespace Amazon.Runtime.Internal.Util
{
	internal class InternalLog4netLogger : InternalLogger
	{
		private enum LoadState
		{
			Uninitialized,
			Failed,
			Loading,
			Success
		}

		private static LoadState loadState = LoadState.Uninitialized;

		private static readonly object LOCK = new object();

		private static Type logMangerType;

		private static ITypeInfo logMangerTypeInfo;

		private static MethodInfo getLoggerWithTypeMethod;

		private static Type logType;

		private static ITypeInfo logTypeInfo;

		private static MethodInfo logMethod;

		private static Type levelType;

		private static ITypeInfo levelTypeInfo;

		private static object debugLevelPropertyValue;

		private static object infoLevelPropertyValue;

		private static object errorLevelPropertyValue;

		private static MethodInfo isEnabledForMethod;

		private static Type systemStringFormatType;

		private static Type loggerType;

		private object internalLogger;

		private bool? isErrorEnabled;

		private bool? isDebugEnabled;

		private bool? isInfoEnabled;

		public override bool IsErrorEnabled
		{
			get
			{
				if (!isErrorEnabled.HasValue)
				{
					if (loadState != LoadState.Success || internalLogger == null || loggerType == null || systemStringFormatType == null || errorLevelPropertyValue == null)
					{
						isErrorEnabled = false;
					}
					else
					{
						isErrorEnabled = Convert.ToBoolean(isEnabledForMethod.Invoke(internalLogger, new object[1]
						{
							errorLevelPropertyValue
						}), CultureInfo.InvariantCulture);
					}
				}
				return isErrorEnabled.Value;
			}
		}

		public override bool IsDebugEnabled
		{
			get
			{
				if (!isDebugEnabled.HasValue)
				{
					if (loadState != LoadState.Success || internalLogger == null || loggerType == null || systemStringFormatType == null || debugLevelPropertyValue == null)
					{
						isDebugEnabled = false;
					}
					else
					{
						isDebugEnabled = Convert.ToBoolean(isEnabledForMethod.Invoke(internalLogger, new object[1]
						{
							debugLevelPropertyValue
						}), CultureInfo.InvariantCulture);
					}
				}
				return isDebugEnabled.Value;
			}
		}

		public override bool IsInfoEnabled
		{
			get
			{
				if (!isInfoEnabled.HasValue)
				{
					if (loadState != LoadState.Success || internalLogger == null || loggerType == null || systemStringFormatType == null || infoLevelPropertyValue == null)
					{
						isInfoEnabled = false;
					}
					else
					{
						isInfoEnabled = Convert.ToBoolean(isEnabledForMethod.Invoke(internalLogger, new object[1]
						{
							infoLevelPropertyValue
						}), CultureInfo.InvariantCulture);
					}
				}
				return isInfoEnabled.Value;
			}
		}

		private static void loadStatics()
		{
			lock (LOCK)
			{
				if (loadState == LoadState.Uninitialized)
				{
					loadState = LoadState.Loading;
					try
					{
						loggerType = Type.GetType("Amazon.Runtime.Internal.Util.Logger");
						logMangerType = Type.GetType("log4net.Core.LoggerManager, log4net");
						logMangerTypeInfo = TypeFactory.GetTypeInfo(logMangerType);
						if (logMangerType == null)
						{
							loadState = LoadState.Failed;
						}
						else
						{
							getLoggerWithTypeMethod = logMangerTypeInfo.GetMethod("GetLogger", new ITypeInfo[2]
							{
								TypeFactory.GetTypeInfo(typeof(Assembly)),
								TypeFactory.GetTypeInfo(typeof(Type))
							});
							logType = Type.GetType("log4net.Core.ILogger, log4net");
							logTypeInfo = TypeFactory.GetTypeInfo(logType);
							levelType = Type.GetType("log4net.Core.Level, log4net");
							levelTypeInfo = TypeFactory.GetTypeInfo(levelType);
							debugLevelPropertyValue = levelTypeInfo.GetField("Debug").GetValue(null);
							infoLevelPropertyValue = levelTypeInfo.GetField("Info").GetValue(null);
							errorLevelPropertyValue = levelTypeInfo.GetField("Error").GetValue(null);
							systemStringFormatType = Type.GetType("log4net.Util.SystemStringFormat, log4net");
							logMethod = logTypeInfo.GetMethod("Log", new ITypeInfo[4]
							{
								TypeFactory.GetTypeInfo(typeof(Type)),
								levelTypeInfo,
								TypeFactory.GetTypeInfo(typeof(object)),
								TypeFactory.GetTypeInfo(typeof(Exception))
							});
							isEnabledForMethod = logTypeInfo.GetMethod("IsEnabledFor", new ITypeInfo[1]
							{
								levelTypeInfo
							});
							if (getLoggerWithTypeMethod == null || isEnabledForMethod == null || logType == null || levelType == null || logMethod == null)
							{
								loadState = LoadState.Failed;
							}
							else
							{
								if (AWSConfigs.XmlSectionExists("log4net") && (AWSConfigs.LoggingConfig.LogTo & LoggingOptions.Log4Net) == LoggingOptions.Log4Net)
								{
									TypeFactory.GetTypeInfo(Type.GetType("log4net.Config.XmlConfigurator, log4net"))?.GetMethod("Configure", new ITypeInfo[0])?.Invoke(null, null);
								}
								loadState = LoadState.Success;
							}
						}
					}
					catch
					{
						loadState = LoadState.Failed;
					}
				}
			}
		}

		public InternalLog4netLogger(Type declaringType)
			: base(declaringType)
		{
			if (loadState == LoadState.Uninitialized)
			{
				loadStatics();
			}
			if (logMangerType != null)
			{
				internalLogger = getLoggerWithTypeMethod.Invoke(null, new object[2]
				{
					TypeFactory.GetTypeInfo(declaringType).Assembly,
					declaringType
				});
			}
		}

		public override void Flush()
		{
		}

		public override void Error(Exception exception, string messageFormat, params object[] args)
		{
			logMethod.Invoke(internalLogger, new object[4]
			{
				loggerType,
				errorLevelPropertyValue,
				new LogMessage(CultureInfo.InvariantCulture, messageFormat, args),
				exception
			});
		}

		public override void Debug(Exception exception, string messageFormat, params object[] args)
		{
			logMethod.Invoke(internalLogger, new object[4]
			{
				loggerType,
				debugLevelPropertyValue,
				new LogMessage(CultureInfo.InvariantCulture, messageFormat, args),
				exception
			});
		}

		public override void DebugFormat(string message, params object[] arguments)
		{
			logMethod.Invoke(internalLogger, new object[4]
			{
				loggerType,
				debugLevelPropertyValue,
				new LogMessage(CultureInfo.InvariantCulture, message, arguments),
				null
			});
		}

		public override void InfoFormat(string message, params object[] arguments)
		{
			logMethod.Invoke(internalLogger, new object[4]
			{
				loggerType,
				infoLevelPropertyValue,
				new LogMessage(CultureInfo.InvariantCulture, message, arguments),
				null
			});
		}
	}
}
