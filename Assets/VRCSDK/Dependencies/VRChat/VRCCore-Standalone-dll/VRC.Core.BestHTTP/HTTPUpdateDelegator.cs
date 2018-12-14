using System.Threading;
using UnityEngine;
using VRC.Core.BestHTTP.Caching;
using VRC.Core.BestHTTP.Cookies;

namespace VRC.Core.BestHTTP
{
	[ExecuteInEditMode]
	internal sealed class HTTPUpdateDelegator : MonoBehaviour
	{
		private static bool IsSetupCalled;

		public static HTTPUpdateDelegator Instance
		{
			get;
			private set;
		}

		public static bool IsCreated
		{
			get;
			private set;
		}

		public static bool IsThreaded
		{
			get;
			set;
		}

		public static bool IsThreadRunning
		{
			get;
			private set;
		}

		public static int ThreadFrequencyInMS
		{
			get;
			set;
		}

		public HTTPUpdateDelegator()
			: this()
		{
		}

		static HTTPUpdateDelegator()
		{
			ThreadFrequencyInMS = 100;
		}

		public static void CheckInstance()
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Expected O, but got Unknown
			try
			{
				if (!IsCreated)
				{
					GameObject val = GameObject.Find("HTTP Update Delegator");
					if (val != null)
					{
						Instance = val.GetComponent<HTTPUpdateDelegator>();
					}
					if (Instance == null)
					{
						val = new GameObject("HTTP Update Delegator");
						val.set_hideFlags(61);
						try
						{
							Object.DontDestroyOnLoad(val);
						}
						catch
						{
						}
						Instance = val.AddComponent<HTTPUpdateDelegator>();
					}
					IsCreated = true;
				}
			}
			catch
			{
				HTTPManager.Logger.Error("HTTPUpdateDelegator", "Please call the VRC.Core.BestHTTP.HTTPManager.Setup() from one of Unity's event(eg. awake, start) before you send any request!");
			}
		}

		private void Setup()
		{
			HTTPCacheService.SetupCacheFolder();
			CookieJar.SetupFolder();
			CookieJar.Load();
			if (IsThreaded)
			{
				new Thread(ThreadFunc).Start();
			}
			IsSetupCalled = true;
		}

		private void ThreadFunc(object obj)
		{
			IsThreadRunning = true;
			while (IsThreadRunning)
			{
				HTTPManager.OnUpdate();
				Thread.Sleep(ThreadFrequencyInMS);
			}
		}

		private void Update()
		{
			if (!IsSetupCalled)
			{
				IsSetupCalled = true;
				Setup();
			}
			if (!IsThreaded)
			{
				HTTPManager.OnUpdate();
			}
		}

		private void OnDisable()
		{
			OnApplicationQuit();
		}

		private void OnApplicationQuit()
		{
			if (IsCreated)
			{
				IsCreated = false;
				IsThreadRunning = true;
				HTTPManager.OnQuit();
			}
		}
	}
}
