using System;
using System.Linq;
using System.Reflection;

namespace Amazon.Util.Internal
{
	public class AndroidInterop
	{
		public static T CallStaticJavaMethod<T>(string className, string methodName, params object[] parameters)
		{
			Type typeFromUnityEngine = InternalSDKUtils.GetTypeFromUnityEngine("AndroidJavaClass");
			if (typeFromUnityEngine != null)
			{
				object obj = Activator.CreateInstance(typeFromUnityEngine, className);
				MethodInfo methodInfo = (from x in typeFromUnityEngine.GetMethods()
				where x.Name == "CallStatic"
				select x).FirstOrDefault();
				if (methodInfo != null)
				{
					return (T)methodInfo.Invoke(obj, new object[2]
					{
						methodName,
						parameters
					});
				}
			}
			return default(T);
		}

		public static object GetJavaObjectStatically(string className, string methodName)
		{
			Type typeFromUnityEngine = InternalSDKUtils.GetTypeFromUnityEngine("AndroidJavaClass");
			Type typeFromUnityEngine2 = InternalSDKUtils.GetTypeFromUnityEngine("AndroidJavaObject");
			if (typeFromUnityEngine != null)
			{
				object obj = Activator.CreateInstance(typeFromUnityEngine, className);
				return (from x in typeFromUnityEngine.GetMethods()
				where x.Name == "CallStatic"
				select x).First((MethodInfo x) => x.ContainsGenericParameters).MakeGenericMethod(typeFromUnityEngine2).Invoke(obj, new object[2]
				{
					methodName,
					new object[0]
				});
			}
			return null;
		}

		public static T CallMethod<T>(object androidJavaObject, string methodName, params object[] parameters)
		{
			return (T)(from x in androidJavaObject.GetType().GetMethods()
			where x.Name == "Call"
			select x).First((MethodInfo x) => x.ContainsGenericParameters).MakeGenericMethod(typeof(T)).Invoke(androidJavaObject, new object[2]
			{
				methodName,
				parameters
			});
		}

		public static object CallMethod(object androidJavaObject, string methodName, params object[] parameters)
		{
			Type typeFromUnityEngine = InternalSDKUtils.GetTypeFromUnityEngine("AndroidJavaObject");
			return (from x in androidJavaObject.GetType().GetMethods()
			where x.Name == "Call"
			select x).First((MethodInfo x) => x.ContainsGenericParameters).MakeGenericMethod(typeFromUnityEngine).Invoke(androidJavaObject, new object[2]
			{
				methodName,
				parameters
			});
		}

		public static T GetStaticJavaField<T>(string className, string methodName)
		{
			Type typeFromUnityEngine = InternalSDKUtils.GetTypeFromUnityEngine("AndroidJavaClass");
			if (typeFromUnityEngine != null)
			{
				object obj = Activator.CreateInstance(typeFromUnityEngine, className);
				MethodInfo methodInfo = typeFromUnityEngine.GetMethod("GetStatic").MakeGenericMethod(typeof(T));
				if (methodInfo != null)
				{
					return (T)methodInfo.Invoke(obj, new object[1]
					{
						methodName
					});
				}
			}
			return default(T);
		}

		public static object GetStaticJavaField(string className, string methodName)
		{
			Type typeFromUnityEngine = InternalSDKUtils.GetTypeFromUnityEngine("AndroidJavaClass");
			Type typeFromUnityEngine2 = InternalSDKUtils.GetTypeFromUnityEngine("AndroidJavaObject");
			if (typeFromUnityEngine != null)
			{
				object obj = Activator.CreateInstance(typeFromUnityEngine, className);
				MethodInfo methodInfo = typeFromUnityEngine.GetMethod("GetStatic").MakeGenericMethod(typeFromUnityEngine2);
				if (methodInfo != null)
				{
					return methodInfo.Invoke(obj, new object[1]
					{
						methodName
					});
				}
			}
			return null;
		}

		public static T GetJavaField<T>(object androidJavaObject, string methodName)
		{
			return (T)(from x in androidJavaObject.GetType().GetMethods()
			where x.Name == "Get"
			select x).First((MethodInfo x) => x.ContainsGenericParameters).MakeGenericMethod(typeof(T)).Invoke(androidJavaObject, new object[1]
			{
				methodName
			});
		}

		public static object GetAndroidContext()
		{
			return GetStaticJavaField("com.unity3d.player.UnityPlayer", "currentActivity");
		}
	}
}
