using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Util;
using Amazon.Util;
using Amazon.Util.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml.Linq;
using UnityEngine;

namespace Amazon
{
	public static class AWSConfigs
	{
		public enum HttpClientOption
		{
			UnityWWW,
			UnityWebRequest
		}

		private static char[] validSeparators = new char[2]
		{
			' ',
			','
		};

		internal static string _awsRegion = GetConfig("AWSRegion");

		internal static LoggingOptions _logging = GetLoggingSetting();

		internal static ResponseLoggingOption _responseLogging = GetConfigEnum<ResponseLoggingOption>("AWSResponseLogging");

		internal static bool _logMetrics = GetConfigBool("AWSLogMetrics");

		internal static string _endpointDefinition = GetConfig("AWSEndpointDefinition");

		internal static string _awsProfileName = GetConfig("AWSProfileName");

		internal static string _awsAccountsLocation = GetConfig("AWSProfilesLocation");

		internal static bool _useSdkCache = GetConfigBool("AWSCache", defaultValue: true);

		private static object _lock = new object();

		private static List<string> standardConfigs = new List<string>
		{
			"region",
			"logging",
			"correctForClockSkew"
		};

		private static bool configPresent = true;

		private static RootConfig _rootConfig = new RootConfig();

		public const string AWSRegionKey = "AWSRegion";

		public const string AWSProfileNameKey = "AWSProfileName";

		public const string AWSProfilesLocationKey = "AWSProfilesLocation";

		public const string LoggingKey = "AWSLogging";

		public const string ResponseLoggingKey = "AWSResponseLogging";

		public const string LogMetricsKey = "AWSLogMetrics";

		public const string EndpointDefinitionKey = "AWSEndpointDefinition";

		public const string UseSdkCacheKey = "AWSCache";

		internal const string LoggingDestinationProperty = "LogTo";

		internal static PropertyChangedEventHandler mPropertyChanged;

		internal static readonly object propertyChangedLock = new object();

		private static HttpClientOption _httpClient;

		internal static bool UnityWebRequestInitialized;

		internal static Dictionary<string, List<TraceListener>> _traceListeners = new Dictionary<string, List<TraceListener>>(StringComparer.OrdinalIgnoreCase);

		private const string CONFIG_FILE = "awsconfig";

		internal static XDocument xmlDoc;

		public static bool CorrectForClockSkew
		{
			get
			{
				return _rootConfig.CorrectForClockSkew;
			}
			set
			{
				_rootConfig.CorrectForClockSkew = value;
			}
		}

		public static TimeSpan ClockOffset
		{
			get;
			internal set;
		}

		public static string AWSRegion
		{
			get
			{
				return _rootConfig.Region;
			}
			set
			{
				_rootConfig.Region = value;
			}
		}

		public static string AWSProfileName
		{
			get
			{
				return _rootConfig.ProfileName;
			}
			set
			{
				_rootConfig.ProfileName = value;
			}
		}

		public static string AWSProfilesLocation
		{
			get
			{
				return _rootConfig.ProfilesLocation;
			}
			set
			{
				_rootConfig.ProfilesLocation = value;
			}
		}

		[Obsolete("This property is obsolete. Use LoggingConfig.LogTo instead.")]
		public static LoggingOptions Logging
		{
			get
			{
				return _rootConfig.Logging.LogTo;
			}
			set
			{
				_rootConfig.Logging.LogTo = value;
			}
		}

		[Obsolete("This property is obsolete. Use LoggingConfig.LogResponses instead.")]
		public static ResponseLoggingOption ResponseLogging
		{
			get
			{
				return _rootConfig.Logging.LogResponses;
			}
			set
			{
				_rootConfig.Logging.LogResponses = value;
			}
		}

		[Obsolete("This property is obsolete. Use LoggingConfig.LogMetrics instead.")]
		public static bool LogMetrics
		{
			get
			{
				return _rootConfig.Logging.LogMetrics;
			}
			set
			{
				_rootConfig.Logging.LogMetrics = value;
			}
		}

		public static string EndpointDefinition
		{
			get
			{
				return _rootConfig.EndpointDefinition;
			}
			set
			{
				_rootConfig.EndpointDefinition = value;
			}
		}

		public static bool UseSdkCache
		{
			get
			{
				return _rootConfig.UseSdkCache;
			}
			set
			{
				_rootConfig.UseSdkCache = value;
			}
		}

		public static LoggingConfig LoggingConfig => _rootConfig.Logging;

		public static ProxyConfig ProxyConfig => _rootConfig.Proxy;

		public static RegionEndpoint RegionEndpoint
		{
			get
			{
				return _rootConfig.RegionEndpoint;
			}
			set
			{
				_rootConfig.RegionEndpoint = value;
			}
		}

		public static string ApplicationName
		{
			get
			{
				return _rootConfig.ApplicationName;
			}
			set
			{
				_rootConfig.ApplicationName = value;
			}
		}

		public static HttpClientOption HttpClient
		{
			get
			{
				return _httpClient;
			}
			set
			{
				if (value == HttpClientOption.UnityWebRequest)
				{
					if (!UnityWebRequestWrapper.IsUnityWebRequestSupported)
					{
						UnityWebRequestInitialized = false;
						throw new InvalidOperationException("UnityWebRequest is not supported in the current version of unity");
					}
					UnityWebRequestInitialized = true;
				}
				_httpClient = value;
			}
		}

		internal static event PropertyChangedEventHandler PropertyChanged
		{
			add
			{
				lock (propertyChangedLock)
				{
					mPropertyChanged = (PropertyChangedEventHandler)Delegate.Combine(mPropertyChanged, value);
				}
			}
			remove
			{
				lock (propertyChangedLock)
				{
					mPropertyChanged = (PropertyChangedEventHandler)Delegate.Remove(mPropertyChanged, value);
				}
			}
		}

		private static LoggingOptions GetLoggingSetting()
		{
			string config = GetConfig("AWSLogging");
			if (string.IsNullOrEmpty(config))
			{
				return LoggingOptions.None;
			}
			string[] array = config.Split(validSeparators, StringSplitOptions.RemoveEmptyEntries);
			if (array == null || array.Length == 0)
			{
				return LoggingOptions.None;
			}
			LoggingOptions loggingOptions = LoggingOptions.None;
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				LoggingOptions loggingOptions2 = ParseEnum<LoggingOptions>(array2[i]);
				loggingOptions |= loggingOptions2;
			}
			return loggingOptions;
		}

		internal static void OnPropertyChanged(string name)
		{
			mPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(name));
		}

		private static bool GetConfigBool(string name, bool defaultValue = false)
		{
			if (bool.TryParse(GetConfig(name), out bool result))
			{
				return result;
			}
			return defaultValue;
		}

		private static T GetConfigEnum<T>(string name)
		{
			ITypeInfo typeInfo = TypeFactory.GetTypeInfo(typeof(T));
			if (!typeInfo.IsEnum)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Type {0} must be enum", typeInfo.FullName));
			}
			string config = GetConfig(name);
			if (string.IsNullOrEmpty(config))
			{
				return default(T);
			}
			return ParseEnum<T>(config);
		}

		private static T ParseEnum<T>(string value)
		{
			if (TryParseEnum(value, out T result))
			{
				return result;
			}
			Type typeFromHandle = typeof(T);
			string format = "Unable to parse value {0} as enum of type {1}. Valid values are: {2}";
			string text = string.Join(", ", Enum.GetNames(typeFromHandle));
			throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, format, value, typeFromHandle.FullName, text));
		}

		private static bool TryParseEnum<T>(string value, out T result)
		{
			result = default(T);
			if (!string.IsNullOrEmpty(value))
			{
				try
				{
					T val = result = (T)Enum.Parse(typeof(T), value, ignoreCase: true);
					return true;
				}
				catch (ArgumentException)
				{
					return false;
				}
			}
			return false;
		}

		public static string GetConfig(string name)
		{
			return null;
		}

		internal static T GetSection<T>(string sectionName) where T : AWSSection, new()
		{
			if (!configPresent)
			{
				return new T();
			}
			if (xmlDoc == null)
			{
				lock (_lock)
				{
					if (xmlDoc == null)
					{
						xmlDoc = LoadConfigFromResource();
						configPresent = (xmlDoc != null);
					}
				}
			}
			if (configPresent)
			{
				XElement xElement = xmlDoc.Element(sectionName);
				T val = new T();
				val.Logging = GetObject<LoggingSection>(xElement, "logging");
				val.Region = ((xElement.Attribute("region") == null) ? string.Empty : xElement.Attribute("region").Value);
				val.CorrectForClockSkew = bool.Parse(xElement.Attribute("correctForClockSkew").Value);
				val.ServiceSections = GetUnresolvedElements(xElement);
				return val;
			}
			return new T();
		}

		internal static bool XmlSectionExists(string sectionName)
		{
			if (configPresent)
			{
				return xmlDoc.Element(sectionName) != null;
			}
			return false;
		}

		public static void AddTraceListener(string source, TraceListener listener)
		{
			if (string.IsNullOrEmpty(source))
			{
				throw new ArgumentException("Source cannot be null or empty", "source");
			}
			if (listener == null)
			{
				throw new ArgumentException("Listener cannot be null", "listener");
			}
			lock (_traceListeners)
			{
				if (!_traceListeners.ContainsKey(source))
				{
					_traceListeners.Add(source, new List<TraceListener>());
				}
				_traceListeners[source].Add(listener);
			}
		}

		public static void RemoveTraceListener(string source, string name)
		{
			if (string.IsNullOrEmpty(source))
			{
				throw new ArgumentException("Source cannot be null or empty", "source");
			}
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("Name cannot be null or empty", "name");
			}
			lock (_traceListeners)
			{
				if (_traceListeners.ContainsKey(source))
				{
					foreach (TraceListener item in _traceListeners[source])
					{
						if (item.Name.Equals(name, StringComparison.Ordinal))
						{
							_traceListeners[source].Remove(item);
							break;
						}
					}
				}
			}
			Logger.ClearLoggerCache();
		}

		internal static TraceListener[] TraceListeners(string source)
		{
			lock (_traceListeners)
			{
				if (_traceListeners.TryGetValue(source, out List<TraceListener> value))
				{
					return value.ToArray();
				}
				return new TraceListener[0];
			}
		}

		private static XDocument LoadConfigFromResource()
		{
			XDocument xDoc = null;
			TextAsset awsConfig = null;
			Action action = delegate
			{
				awsConfig = (Resources.Load("awsconfig") as TextAsset);
				if (awsConfig != null && awsConfig.get_bytes().Count() > 0)
				{
					using (Stream stream = new MemoryStream(awsConfig.get_bytes()))
					{
						using (StreamReader textReader = new StreamReader(stream))
						{
							xDoc = XDocument.Load(textReader);
						}
					}
				}
			};
			if (UnityInitializer.IsMainThread())
			{
				action();
			}
			else
			{
				ManualResetEvent e = new ManualResetEvent(initialState: false);
				UnityRequestQueue.Instance.ExecuteOnMainThread(delegate
				{
					action();
					e.Set();
				});
				e.WaitOne();
			}
			return xDoc;
		}

		public static T GetObject<T>(XElement rootElement, string propertyName) where T : class, new()
		{
			return (T)GetObject(rootElement, propertyName, typeof(T));
		}

		private static object GetObject(XElement rootElement, string propertyName, Type type)
		{
			object obj = Activator.CreateInstance(type);
			PropertyInfo[] properties = obj.GetType().GetProperties();
			rootElement = ((rootElement.Elements(propertyName).Count() == 0) ? rootElement : rootElement.Elements(propertyName).First());
			PropertyInfo[] array = properties;
			foreach (PropertyInfo propertyInfo in array)
			{
				if (propertyInfo.CanWrite)
				{
					Type propertyType = propertyInfo.PropertyType;
					XAttribute xAttribute = rootElement.Attributes().SingleOrDefault((XAttribute a) => a.Name.ToString().Equals(propertyInfo.Name, StringComparison.OrdinalIgnoreCase));
					if (xAttribute != null)
					{
						if (propertyType.BaseType.Equals(typeof(Enum)))
						{
							propertyInfo.SetValue(obj, Enum.Parse(propertyType, xAttribute.Value, ignoreCase: true), null);
						}
						else
						{
							Type underlyingType = Nullable.GetUnderlyingType(propertyType);
							object value = (underlyingType != null) ? Convert.ChangeType(xAttribute.Value, underlyingType) : ((propertyType != typeof(Type)) ? Convert.ChangeType(xAttribute.Value, propertyType) : Type.GetType(xAttribute.Value, throwOnError: true));
							propertyInfo.SetValue(obj, value, null);
						}
					}
					else
					{
						XElement xElement = rootElement.Elements().SingleOrDefault((XElement e) => e.Name.ToString().Equals(propertyInfo.Name, StringComparison.OrdinalIgnoreCase));
						if (typeof(IList).IsAssignableFrom(propertyType))
						{
							string propertyName2 = xElement?.Name.ToString();
							IEnumerable list = GetList(rootElement, propertyType, propertyName2);
							propertyInfo.SetValue(obj, list, null);
						}
						else if (xElement != null)
						{
							object @object = GetObject(xElement, xElement.Name.ToString(), propertyType);
							propertyInfo.SetValue(obj, @object, null);
						}
					}
				}
			}
			return obj;
		}

		private static IEnumerable GetList(XElement rootElement, Type listType, string propertyName)
		{
			IList list = (IList)Activator.CreateInstance(listType);
			string propertyName2 = (string)listType.GetProperty("ItemPropertyName").GetValue(list, null);
			Type propertyType = listType.GetProperty("Item").PropertyType;
			if (!string.IsNullOrEmpty(propertyName))
			{
				rootElement = ((rootElement.Elements(propertyName).Count() == 0) ? rootElement : rootElement.Elements(propertyName).First());
			}
			foreach (XElement item in rootElement.Elements())
			{
				object @object = GetObject(item, propertyName2, propertyType);
				list.Add(@object);
			}
			return list;
		}

		private static IDictionary<string, XElement> GetUnresolvedElements(XElement parent)
		{
			IDictionary<string, XElement> dictionary = new Dictionary<string, XElement>();
			foreach (XElement item in parent.Elements())
			{
				if (!standardConfigs.Contains(item.Name.ToString()))
				{
					dictionary.Add(item.Name.ToString(), item);
				}
			}
			return dictionary;
		}
	}
}
