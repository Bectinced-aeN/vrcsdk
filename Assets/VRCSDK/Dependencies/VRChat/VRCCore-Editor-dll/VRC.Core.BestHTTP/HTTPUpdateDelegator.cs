using System;
using System.Threading;
using UnityEditor;
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

		public unsafe static void CheckInstance()
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Expected O, but got Unknown
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Expected O, but got Unknown
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Expected O, but got Unknown
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Expected O, but got Unknown
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Expected O, but got Unknown
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Expected O, but got Unknown
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Expected O, but got Unknown
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Expected O, but got Unknown
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Expected O, but got Unknown
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
					if (!EditorApplication.get_isPlaying())
					{
						EditorApplication.update = Delegate.Remove((Delegate)EditorApplication.update, (Delegate)new CallbackFunction((object)Instance, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
						EditorApplication.update = Delegate.Combine((Delegate)EditorApplication.update, (Delegate)new CallbackFunction((object)Instance, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
					}
					EditorApplication.playmodeStateChanged = Delegate.Remove((Delegate)EditorApplication.playmodeStateChanged, (Delegate)new CallbackFunction((object)Instance, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
					EditorApplication.playmodeStateChanged = Delegate.Combine((Delegate)EditorApplication.playmodeStateChanged, (Delegate)new CallbackFunction((object)Instance, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
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

		private unsafe void OnPlayModeStateChanged()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Expected O, but got Unknown
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Expected O, but got Unknown
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Expected O, but got Unknown
			if (EditorApplication.get_isPlaying())
			{
				EditorApplication.update = Delegate.Remove((Delegate)EditorApplication.update, (Delegate)new CallbackFunction((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			}
			else if (!EditorApplication.get_isPlaying())
			{
				EditorApplication.update = Delegate.Combine((Delegate)EditorApplication.update, (Delegate)new CallbackFunction((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			}
		}

		private void OnDisable()
		{
			OnApplicationQuit();
		}

		private unsafe void OnApplicationQuit()
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Expected O, but got Unknown
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Expected O, but got Unknown
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Expected O, but got Unknown
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Expected O, but got Unknown
			if (IsCreated)
			{
				IsCreated = false;
				IsThreadRunning = true;
				HTTPManager.OnQuit();
				EditorApplication.update = Delegate.Remove((Delegate)EditorApplication.update, (Delegate)new CallbackFunction((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
				EditorApplication.playmodeStateChanged = Delegate.Remove((Delegate)EditorApplication.playmodeStateChanged, (Delegate)new CallbackFunction((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			}
		}
	}
}
