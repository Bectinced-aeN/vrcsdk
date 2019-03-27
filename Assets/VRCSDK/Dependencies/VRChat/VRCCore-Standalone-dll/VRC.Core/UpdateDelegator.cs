using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace VRC.Core
{
	[ExecuteInEditMode]
	public sealed class UpdateDelegator : MonoBehaviour
	{
		private static int? spawnedThreadID;

		private static readonly object _queueLock = new object();

		private static Queue<Action> _jobQueue = new Queue<Action>();

		private static UpdateDelegator Instance;

		public static bool IsMainThread => spawnedThreadID.HasValue && Thread.CurrentThread.ManagedThreadId == spawnedThreadID;

		public UpdateDelegator()
			: this()
		{
		}

		public static void Initialize()
		{
			CheckInstance();
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
						try
						{
							Object.DontDestroyOnLoad(val);
						}
						catch
						{
						}
						val.AddComponent<UpdateDelegator>();
					}
				}
				catch (Exception ex)
				{
					Debug.LogErrorFormat("Caught {0} in UpdateDelegator.CheckInstance: {1}", new object[2]
					{
						ex.GetType().Name,
						ex.Message
					});
					throw;
					IL_0085:;
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
				Debug.LogErrorFormat("Caught {0} in UpdateDelegator Job: {1}\n{2}", new object[3]
				{
					ex.GetType().Name,
					ex.Message,
					ex.StackTrace
				});
			}
		}
	}
}
