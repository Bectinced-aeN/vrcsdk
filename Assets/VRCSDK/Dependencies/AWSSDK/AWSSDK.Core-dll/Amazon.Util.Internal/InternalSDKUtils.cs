using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Util;
using Amazon.Util.Internal.PlatformServices;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading;
using UnityEngine;

namespace Amazon.Util.Internal
{
	public static class InternalSDKUtils
	{
		internal const string UnknownVersion = "Unknown";

		internal const string UnknownNetFrameworkVersion = ".NET_Runtime/Unknown .NET_Framework/Unknown";

		private static string _versionNumber;

		private static string _customSdkUserAgent;

		private static string _customData;

		internal const string CoreVersionNumber = "3.3.5.0";

		private static string _userAgentBaseName = "aws-sdk-unity";

		private const string UnknownMonoVersion = "Mono/Unknown";

		private static Logger _logger = Logger.GetLogger(typeof(InternalSDKUtils));

		public static bool IsAndroid => (int)Application.get_platform() == 11;

		public static bool IsiOS => (int)Application.get_platform() == 8;

		public static void SetUserAgent(string productName, string versionNumber)
		{
			SetUserAgent(productName, versionNumber, null);
		}

		public static void SetUserAgent(string productName, string versionNumber, string customData)
		{
			_userAgentBaseName = productName;
			_versionNumber = versionNumber;
			_customData = customData;
			BuildCustomUserAgentString();
		}

		private static void BuildCustomUserAgentString()
		{
			if (_versionNumber == null)
			{
				_versionNumber = "3.3.5.0";
			}
			IEnvironmentInfo service = ServiceFactory.Instance.GetService<IEnvironmentInfo>();
			_customSdkUserAgent = string.Format(CultureInfo.InvariantCulture, "{0}/{1} {2} OS/{3} {4}", _userAgentBaseName, _versionNumber, service.FrameworkUserAgent, service.PlatformUserAgent, _customData).Trim();
		}

		public static string BuildUserAgentString(string serviceSdkVersion)
		{
			if (!string.IsNullOrEmpty(_customSdkUserAgent))
			{
				return _customSdkUserAgent;
			}
			IEnvironmentInfo service = ServiceFactory.Instance.GetService<IEnvironmentInfo>();
			return string.Format(CultureInfo.InvariantCulture, "{0}/{1} aws-sdk-core/{2} {3} OS/{4} {5}", _userAgentBaseName, serviceSdkVersion, "3.3.5.0", service.FrameworkUserAgent, service.PlatformUserAgent, _customData).Trim();
		}

		public static void ApplyValues(object target, IDictionary<string, object> propertyValues)
		{
			if (propertyValues != null && propertyValues.Count != 0)
			{
				ITypeInfo typeInfo = TypeFactory.GetTypeInfo(target.GetType());
				foreach (KeyValuePair<string, object> propertyValue in propertyValues)
				{
					PropertyInfo property = typeInfo.GetProperty(propertyValue.Key);
					if (property == null)
					{
						throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Unable to find property {0} on type {1}.", propertyValue.Key, typeInfo.FullName));
					}
					try
					{
						if (TypeFactory.GetTypeInfo(property.PropertyType).IsEnum)
						{
							object value = Enum.Parse(property.PropertyType, propertyValue.Value.ToString(), ignoreCase: true);
							property.SetValue(target, value, null);
						}
						else
						{
							property.SetValue(target, propertyValue.Value, null);
						}
					}
					catch (Exception ex)
					{
						throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Unable to set property {0} on type {1}: {2}", propertyValue.Key, typeInfo.FullName, ex.Message));
					}
				}
			}
		}

		public static void AddToDictionary<TKey, TValue>(Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
		{
			if (dictionary.ContainsKey(key))
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Dictionary already contains item with key {0}", key));
			}
			dictionary[key] = value;
		}

		public static void FillDictionary<T, TKey, TValue>(IEnumerable<T> items, Func<T, TKey> keyGenerator, Func<T, TValue> valueGenerator, Dictionary<TKey, TValue> targetDictionary)
		{
			foreach (T item in items)
			{
				TKey key = keyGenerator(item);
				TValue value = valueGenerator(item);
				AddToDictionary(targetDictionary, key, value);
			}
		}

		public static Dictionary<TKey, TValue> ToDictionary<T, TKey, TValue>(IEnumerable<T> items, Func<T, TKey> keyGenerator, Func<T, TValue> valueGenerator)
		{
			return ToDictionary(items, keyGenerator, valueGenerator, null);
		}

		public static Dictionary<TKey, TValue> ToDictionary<T, TKey, TValue>(IEnumerable<T> items, Func<T, TKey> keyGenerator, Func<T, TValue> valueGenerator, IEqualityComparer<TKey> comparer)
		{
			Dictionary<TKey, TValue> dictionary = (comparer != null) ? new Dictionary<TKey, TValue>(comparer) : new Dictionary<TKey, TValue>();
			FillDictionary(items, keyGenerator, valueGenerator, dictionary);
			return dictionary;
		}

		public static bool TryFindByValue<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TValue value, IEqualityComparer<TValue> valueComparer, out TKey key)
		{
			foreach (KeyValuePair<TKey, TValue> item in dictionary)
			{
				TValue value2 = item.Value;
				if (valueComparer.Equals(value, value2))
				{
					key = item.Key;
					return true;
				}
			}
			key = default(TKey);
			return false;
		}

		public static void SetIsSet<T>(bool isSet, ref T? field) where T : struct
		{
			if (isSet)
			{
				field = default(T);
			}
			else
			{
				field = null;
			}
		}

		public static void SetIsSet<T>(bool isSet, ref List<T> field)
		{
			if (isSet)
			{
				field = new AlwaysSendList<T>(field);
			}
			else
			{
				field = new List<T>();
			}
		}

		public static void SetIsSet<TKey, TValue>(bool isSet, ref Dictionary<TKey, TValue> field)
		{
			if (isSet)
			{
				field = new AlwaysSendDictionary<TKey, TValue>(field);
			}
			else
			{
				field = new Dictionary<TKey, TValue>();
			}
		}

		public static bool GetIsSet<T>(T? field) where T : struct
		{
			return field.HasValue;
		}

		public static bool GetIsSet<T>(List<T> field)
		{
			if (field == null)
			{
				return false;
			}
			if (field.Count > 0)
			{
				return true;
			}
			if (field is AlwaysSendList<T>)
			{
				return true;
			}
			return false;
		}

		public static bool GetIsSet<TKey, TVvalue>(Dictionary<TKey, TVvalue> field)
		{
			if (field == null)
			{
				return false;
			}
			if (field.Count > 0)
			{
				return true;
			}
			if (field is AlwaysSendDictionary<TKey, TVvalue>)
			{
				return true;
			}
			return false;
		}

		public static string GetMonoRuntimeVersion()
		{
			Type type = Type.GetType("Mono.Runtime");
			if (type != null)
			{
				MethodInfo method = type.GetMethod("GetDisplayName");
				if (method != null)
				{
					string text = (string)method.Invoke(null, null);
					text = text.Replace("/", ":").Replace(" ", string.Empty);
					return "Mono/" + text;
				}
			}
			return "Mono/Unknown";
		}

		internal static Type GetTypeFromUnityEngine(string typeName)
		{
			return Type.GetType($"UnityEngine.{typeName}, UnityEngine");
		}

		public static void AsyncExecutor(Action action, AsyncOptions options)
		{
			if (options.ExecuteCallbackOnMainThread)
			{
				if (UnityInitializer.IsMainThread())
				{
					SafeExecute(action);
				}
				else
				{
					UnityRequestQueue.Instance.ExecuteOnMainThread(action);
				}
			}
			else if (!UnityInitializer.IsMainThread())
			{
				SafeExecute(action);
			}
			else
			{
				ThreadPool.QueueUserWorkItem(delegate
				{
					SafeExecute(action);
				});
			}
		}

		public static void SafeExecute(Action action)
		{
			try
			{
				action();
			}
			catch (Exception exception)
			{
				_logger.Error(exception, "An unhandled exception was thrown from the callback method {0}.", action.Method.Name);
			}
		}
	}
}
