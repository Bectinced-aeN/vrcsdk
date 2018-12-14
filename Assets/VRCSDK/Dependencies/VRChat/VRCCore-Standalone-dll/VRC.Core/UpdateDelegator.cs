using System;
using System.Collections.Generic;
using System.Threading;
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

		public static void CheckInstance()
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Expected O, but got Unknown
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
						try
						{
							Object.DontDestroyOnLoad(val);
						}
						catch
						{
						}
						Instance = val.AddComponent<UpdateDelegator>();
					}
					spawnedThreadID = Thread.CurrentThread.ManagedThreadId;
					IsCreated = true;
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

		private void OnDisable()
		{
			OnApplicationQuit();
		}

		private void OnApplicationQuit()
		{
			if (IsCreated)
			{
				IsCreated = false;
			}
		}
	}
}
