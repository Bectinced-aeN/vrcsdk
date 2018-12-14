using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace AmplitudeSDKWrapper
{
	public class LimitedConcurrencyLevelTaskScheduler
	{
		private readonly LinkedList<Action> _tasks = new LinkedList<Action>();

		private readonly int _maxDegreeOfParallelism;

		private int _delegatesQueuedOrRunning;

		public LimitedConcurrencyLevelTaskScheduler(int maxDegreeOfParallelism)
		{
			if (maxDegreeOfParallelism < 1)
			{
				throw new ArgumentOutOfRangeException("maxDegreeOfParallelism");
			}
			_maxDegreeOfParallelism = maxDegreeOfParallelism;
			Debug.Log((object)("LimitedConcurrencyLevelTaskScheduler created, threads: " + maxDegreeOfParallelism));
		}

		public void QueueTask(Action task)
		{
			lock (_tasks)
			{
				_tasks.AddLast(task);
				if (_delegatesQueuedOrRunning < _maxDegreeOfParallelism)
				{
					_delegatesQueuedOrRunning++;
					NotifyThreadPoolOfPendingWork();
				}
			}
		}

		private void NotifyThreadPoolOfPendingWork()
		{
			ThreadPool.QueueUserWorkItem(delegate
			{
				try
				{
					while (true)
					{
						Action value;
						lock (_tasks)
						{
							if (_tasks.Count == 0)
							{
								_delegatesQueuedOrRunning--;
								return;
							}
							value = _tasks.First.Value;
							_tasks.RemoveFirst();
						}
						try
						{
							value();
						}
						catch (Exception ex)
						{
							Debug.LogError((object)"Exception on LimitedConcurrencyLevelTaskScheduler worker thread:");
							Debug.LogException(ex);
						}
					}
				}
				finally
				{
				}
			});
		}
	}
}
