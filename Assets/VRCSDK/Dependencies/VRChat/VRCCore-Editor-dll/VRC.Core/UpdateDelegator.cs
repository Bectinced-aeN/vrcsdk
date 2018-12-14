using System;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace VRC.Core
{
	[ExecuteInEditMode]
	internal sealed class UpdateDelegator : MonoBehaviour
	{
		private static int? spawnedThreadID;

		private readonly object _queueLock = new object();

		private Queue<Action> _jobQueue = new Queue<Action>();

		public static UpdateDelegator Instance
		{
			get;
			private set;
		}

		public static bool IsCreated
		{
			get;
			private set;
		}

		public static bool IsMainThread => spawnedThreadID.HasValue && Thread.CurrentThread.ManagedThreadId == spawnedThreadID;

		public UpdateDelegator()
			: this()
		{
		}

		public unsafe static void CheckInstance()
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Expected O, but got Unknown
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Expected O, but got Unknown
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Expected O, but got Unknown
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Expected O, but got Unknown
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Expected O, but got Unknown
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Expected O, but got Unknown
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Expected O, but got Unknown
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Expected O, but got Unknown
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Expected O, but got Unknown
			try
			{
				if (!IsCreated)
				{
					GameObject val = GameObject.Find("Update Delegator");
					if (val != null)
					{
						Instance = val.GetComponent<UpdateDelegator>();
					}
					if (Instance == null)
					{
						val = new GameObject("Update Delegator");
						val.set_hideFlags(61);
						Instance = val.AddComponent<UpdateDelegator>();
					}
					spawnedThreadID = Thread.CurrentThread.ManagedThreadId;
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
			catch (Exception ex)
			{
				Debug.LogError((object)"UpdateDelegator: CheckInstance: Exception:");
				Debug.LogException(ex);
			}
		}

		public static void Dispatch(Action job)
		{
			CheckInstance();
			Instance.QueueJobInternal(job);
		}

		private void QueueJobInternal(Action job)
		{
			if (job != null)
			{
				lock (_queueLock)
				{
					_jobQueue.Enqueue(job);
				}
			}
		}

		private void OnUpdate()
		{
			try
			{
				Action action = null;
				lock (_queueLock)
				{
					if (_jobQueue.Count == 0)
					{
						return;
					}
					action = _jobQueue.Dequeue();
				}
				action();
			}
			catch (Exception ex)
			{
				Debug.LogError((object)"Exception in UpdateDelegator job:");
				Debug.LogException(ex);
			}
		}

		private void Update()
		{
			OnUpdate();
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
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Expected O, but got Unknown
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Expected O, but got Unknown
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Expected O, but got Unknown
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Expected O, but got Unknown
			if (IsCreated)
			{
				IsCreated = false;
				EditorApplication.update = Delegate.Remove((Delegate)EditorApplication.update, (Delegate)new CallbackFunction((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
				EditorApplication.playmodeStateChanged = Delegate.Remove((Delegate)EditorApplication.playmodeStateChanged, (Delegate)new CallbackFunction((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			}
		}
	}
}
