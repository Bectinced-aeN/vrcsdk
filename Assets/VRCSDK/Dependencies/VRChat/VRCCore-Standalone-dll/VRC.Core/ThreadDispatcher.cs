using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace VRC.Core
{
	[ExecuteInEditMode]
	public class ThreadDispatcher : MonoBehaviour
	{
		private static ThreadDispatcher Instance;

		private static int? spawnedThreadID;

		private readonly object _queueLock = new object();

		private Queue<Action> _jobQueue = new Queue<Action>();

		public static bool IsMainThread => spawnedThreadID.HasValue && Thread.CurrentThread.ManagedThreadId == spawnedThreadID;

		public ThreadDispatcher()
			: this()
		{
		}

		public static void Initialize()
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Expected O, but got Unknown
			if (!(Instance != null))
			{
				GameObject val = new GameObject("__ThreadDispatcher", new Type[1]
				{
					typeof(ThreadDispatcher)
				});
				val.set_hideFlags(61);
				Instance = val.GetComponent<ThreadDispatcher>();
				spawnedThreadID = Thread.CurrentThread.ManagedThreadId;
			}
		}

		public static void DispatchToMain(Action action)
		{
			if (IsMainThread)
			{
				action();
			}
			else
			{
				AddJob(delegate
				{
					action();
				});
			}
		}

		private static void AddJob(Action job)
		{
			if (Instance == null)
			{
				throw new Exception("ThreadDispatcher instance does not exist");
			}
			Instance.AddJobInternal(job);
		}

		private void Update()
		{
			try
			{
				while (true)
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
			}
			catch (Exception ex)
			{
				Debug.LogError((object)"Exception in ThreadDispatcher job:");
				Debug.LogException(ex);
			}
		}

		private void AddJobInternal(Action job)
		{
			lock (_queueLock)
			{
				_jobQueue.Enqueue(job);
			}
		}
	}
}
