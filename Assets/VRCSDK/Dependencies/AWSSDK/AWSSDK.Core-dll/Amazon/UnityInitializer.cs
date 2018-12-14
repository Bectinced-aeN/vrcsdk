using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Util;
using Amazon.Util.Internal;
using System;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

namespace Amazon
{
	public class UnityInitializer : MonoBehaviour
	{
		private static UnityInitializer _instance = null;

		private static object _lock = new object();

		private static Thread _mainThread;

		public static UnityInitializer Instance => _instance;

		private UnityInitializer()
			: this()
		{
		}

		public static void AttachToGameObject(GameObject gameObject)
		{
			if (gameObject != null)
			{
				gameObject.AddComponent<UnityInitializer>();
				Debug.Log((object)$"Attached unity initializer to {gameObject.get_name()}");
				return;
			}
			throw new ArgumentNullException("gameObject");
		}

		public void Awake()
		{
			lock (_lock)
			{
				if (_instance == null)
				{
					_instance = this;
					if (_mainThread == null || !_mainThread.Equals(Thread.CurrentThread))
					{
						_mainThread = Thread.CurrentThread;
					}
					AmazonHookedPlatformInfo.Instance.Init();
					Object.DontDestroyOnLoad(this);
					TraceListener listener = new UnityDebugTraceListener("UnityDebug");
					AWSConfigs.AddTraceListener("Amazon", listener);
					_instance.get_gameObject().AddComponent<UnityMainThreadDispatcher>();
				}
				else if (this != _instance)
				{
					Object.DestroyObject(this);
				}
			}
		}

		public static bool IsMainThread()
		{
			if (_mainThread == null)
			{
				throw new Exception("Main thread has not been set, is the AWSPrefab on the scene?");
			}
			return Thread.CurrentThread.Equals(_mainThread);
		}
	}
}
