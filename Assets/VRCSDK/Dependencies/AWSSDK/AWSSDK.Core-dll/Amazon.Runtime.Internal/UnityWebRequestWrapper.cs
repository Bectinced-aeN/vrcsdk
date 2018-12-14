using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using UnityEngine;

namespace Amazon.Runtime.Internal
{
	public class UnityWebRequestWrapper : IDisposable
	{
		private static Type unityWebRequestType;

		private static PropertyInfo[] unityWebRequestProperties;

		private static MethodInfo[] unityWebRequestMethods;

		private static MethodInfo setRequestHeaderMethod;

		private static MethodInfo sendMethod;

		private static MethodInfo getResponseHeadersMethod;

		private static MethodInfo isDoneGetMethod;

		private static MethodInfo downloadProgressGetMethod;

		private static MethodInfo uploadProgressGetMethod;

		private static MethodInfo isErrorGetMethod;

		private static MethodInfo downloadedBytesGetMethod;

		private static MethodInfo responseCodeGetMethod;

		private static MethodInfo downloadHandlerSetMethod;

		private static MethodInfo uploadHandlerSetMethod;

		private static MethodInfo errorGetMethod;

		private object unityWebRequestInstance;

		private DownloadHandlerBufferWrapper downloadHandler;

		private UploadHandlerRawWrapper uploadHandler;

		private bool disposedValue;

		internal static bool IsUnityWebRequestSupported => unityWebRequestType != null;

		public DownloadHandlerBufferWrapper DownloadHandler
		{
			get
			{
				return downloadHandler;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				downloadHandlerSetMethod.Invoke(unityWebRequestInstance, new object[1]
				{
					value.Instance
				});
				downloadHandler = value;
			}
		}

		public UploadHandlerRawWrapper UploadHandler
		{
			get
			{
				return uploadHandler;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				uploadHandlerSetMethod.Invoke(unityWebRequestInstance, new object[1]
				{
					value.Instance
				});
				uploadHandler = value;
			}
		}

		public bool IsDone => (bool)isDoneGetMethod.Invoke(unityWebRequestInstance, null);

		public float DownloadProgress => (float)downloadProgressGetMethod.Invoke(unityWebRequestInstance, null);

		public float UploadProgress => (float)uploadProgressGetMethod.Invoke(unityWebRequestInstance, null);

		public ulong DownloadedBytes => (ulong)downloadedBytesGetMethod.Invoke(unityWebRequestInstance, null);

		public Dictionary<string, string> ResponseHeaders => (Dictionary<string, string>)getResponseHeadersMethod.Invoke(unityWebRequestInstance, null);

		public HttpStatusCode? StatusCode
		{
			get
			{
				long num = (long)responseCodeGetMethod.Invoke(unityWebRequestInstance, null);
				if (num == -1)
				{
					return null;
				}
				return (HttpStatusCode)num;
			}
		}

		public bool IsError => (bool)isErrorGetMethod.Invoke(unityWebRequestInstance, null);

		public string Error => (string)errorGetMethod.Invoke(unityWebRequestInstance, null);

		static UnityWebRequestWrapper()
		{
			unityWebRequestType = Type.GetType("UnityEngine.Networking.UnityWebRequest, UnityEngine");
			if (unityWebRequestType == null)
			{
				unityWebRequestType = Type.GetType("UnityEngine.Experimental.Networking.UnityWebRequest, UnityEngine");
			}
			unityWebRequestMethods = unityWebRequestType.GetMethods();
			unityWebRequestProperties = unityWebRequestType.GetProperties();
			PropertyInfo property = unityWebRequestType.GetProperty("isDone");
			PropertyInfo property2 = unityWebRequestType.GetProperty("downloadProgress");
			PropertyInfo property3 = unityWebRequestType.GetProperty("uploadProgress");
			PropertyInfo property4 = unityWebRequestType.GetProperty("isError");
			PropertyInfo property5 = unityWebRequestType.GetProperty("downloadedBytes");
			PropertyInfo property6 = unityWebRequestType.GetProperty("responseCode");
			PropertyInfo property7 = unityWebRequestType.GetProperty("downloadHandler");
			PropertyInfo property8 = unityWebRequestType.GetProperty("uploadHandler");
			PropertyInfo property9 = unityWebRequestType.GetProperty("error");
			setRequestHeaderMethod = unityWebRequestType.GetMethod("SetRequestHeader");
			sendMethod = unityWebRequestType.GetMethod("Send");
			getResponseHeadersMethod = unityWebRequestType.GetMethod("GetResponseHeaders");
			isDoneGetMethod = property.GetGetMethod();
			isErrorGetMethod = property4.GetGetMethod();
			uploadProgressGetMethod = property3.GetGetMethod();
			downloadProgressGetMethod = property2.GetGetMethod();
			downloadedBytesGetMethod = property5.GetGetMethod();
			responseCodeGetMethod = property6.GetGetMethod();
			downloadHandlerSetMethod = property7.GetSetMethod();
			uploadHandlerSetMethod = property8.GetSetMethod();
			errorGetMethod = property9.GetGetMethod();
		}

		public UnityWebRequestWrapper()
		{
			if (!AWSConfigs.UnityWebRequestInitialized)
			{
				throw new InvalidOperationException("UnityWebRequest is not supported in the current version of unity");
			}
			unityWebRequestInstance = Activator.CreateInstance(unityWebRequestType);
		}

		public UnityWebRequestWrapper(string url, string method)
		{
			unityWebRequestInstance = Activator.CreateInstance(unityWebRequestType, url, method);
		}

		public UnityWebRequestWrapper(string url, string method, DownloadHandlerBufferWrapper downloadHandler, UploadHandlerRawWrapper uploadHandler)
		{
			if (downloadHandler == null)
			{
				throw new ArgumentNullException("downloadHandler");
			}
			if (uploadHandler == null)
			{
				throw new ArgumentNullException("uploadHandler");
			}
			unityWebRequestInstance = Activator.CreateInstance(unityWebRequestType, url, method, downloadHandler.Instance, uploadHandler.Instance);
		}

		public void SetRequestHeader(string key, string value)
		{
			setRequestHeaderMethod.Invoke(unityWebRequestInstance, new object[2]
			{
				key,
				value
			});
		}

		public AsyncOperation Send()
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Expected O, but got Unknown
			return sendMethod.Invoke(unityWebRequestInstance, null);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					(unityWebRequestInstance as IDisposable)?.Dispose();
				}
				unityWebRequestInstance = null;
				disposedValue = true;
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
		}
	}
}
