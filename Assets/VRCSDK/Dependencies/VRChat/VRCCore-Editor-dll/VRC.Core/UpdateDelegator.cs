using System;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace VRC.Core
{
	[InitializeOnLoad]
	[ExecuteInEditMode]
	public sealed class UpdateDelegator : MonoBehaviour
	{
		private static int? spawnedThreadID;

		private static readonly object _queueLock;

		private static Queue<Action> _jobQueue;

		private static UpdateDelegator Instance;

		public static bool IsMainThread => spawnedThreadID.HasValue && Thread.CurrentThread.ManagedThreadId == spawnedThreadID;

		public UpdateDelegator()
			: this()
		{
		}

		static UpdateDelegator()
		{
			_queueLock = new object();
			_jobQueue = new Queue<Action>();
			SetupCallbacks();
		}

		public static void Initialize()
		{
			CheckInstance();
		}

		[DidReloadScripts(int.MaxValue)]
		private static void DidReloadScripts()
		{
			SetupCallbacks();
		}

		private unsafe static void SetupCallbacks()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Expected O, but got Unknown
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Expected O, but got Unknown
			EditorApplication.update = Delegate.Remove((Delegate)EditorApplication.update, (Delegate)new CallbackFunction((object)null, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			EditorApplication.update = Delegate.Combine((Delegate)EditorApplication.update, (Delegate)new CallbackFunction((object)null, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private static void EditorUpdate()
		{
			CheckInstance();
			if (Instance != null && !EditorApplication.get_isPlaying())
			{
				Instance.Update();
			}
		}

		private void Awake()
		{
			spawnedThreadID = Thread.CurrentThread.ManagedThreadId;
			OnEnable();
		}

		private void OnEnable()
		{
			if (!(Instance == this))
			{
				if (Instance != null)
				{
					Debug.LogError((object)"Two delegators in existence.");
					Object.Destroy(this);
				}
				else
				{
					spawnedThreadID = Thread.CurrentThread.ManagedThreadId;
					Instance = this;
				}
			}
		}

		private static void CheckInstance()
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Expected O, but got Unknown
			if (!(Instance != null))
			{
				try
				{
					Instance = Object.FindObjectOfType<UpdateDelegator>();
					if (Instance == null)
					{
						GameObject val = new GameObject("Update Delegator");
						val.set_hideFlags(61);
						val.AddComponent<UpdateDelegator>();
					}
				}
				catch (Exception ex)
				{
					Debug.LogError((object)("Exception in UpdateDelegator.CheckInstance: " + ex.ToString()));
					throw;
					IL_0062:;
				}
			}
		}

		public static void Dispatch(Action job)
		{
			if (job == null)
			{
				Debug.LogError((object)"Ignoring NULL job");
			}
			else
			{
				CheckInstance();
				lock (_queueLock)
				{
					_jobQueue.Enqueue(job);
				}
			}
		}

		private void Update()
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
				action?.Invoke();
			}
			catch (Exception ex)
			{
				Debug.LogError((object)"Exception in UpdateDelegator job:");
				Debug.LogException(ex);
			}
		}
	}
}
