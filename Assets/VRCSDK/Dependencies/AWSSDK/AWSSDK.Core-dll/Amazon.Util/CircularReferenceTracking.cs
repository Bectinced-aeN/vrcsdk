using System;
using System.Collections.Generic;
using System.Globalization;

namespace Amazon.Util
{
	public class CircularReferenceTracking
	{
		private class Tracker : IDisposable
		{
			private bool disposed;

			public object Target
			{
				get;
				private set;
			}

			private CircularReferenceTracking State
			{
				get;
				set;
			}

			public Tracker(CircularReferenceTracking state, object target)
			{
				State = state;
				Target = target;
			}

			public override string ToString()
			{
				return string.Format(CultureInfo.InvariantCulture, "Tracking {0}", Target);
			}

			protected virtual void Dispose(bool disposing)
			{
				if (!disposed)
				{
					if (disposing)
					{
						State.PopTracker(this);
					}
					disposed = true;
				}
			}

			public void Dispose()
			{
				Dispose(disposing: true);
				GC.SuppressFinalize(this);
			}

			~Tracker()
			{
				Dispose(disposing: false);
			}
		}

		private object referenceTrackersLock = new object();

		private Stack<Tracker> referenceTrackers = new Stack<Tracker>();

		public IDisposable Track(object target)
		{
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}
			lock (referenceTrackersLock)
			{
				if (TrackerExists(target))
				{
					throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Circular reference detected with object [{0}] of type {1}", target, target.GetType().FullName));
				}
				Tracker tracker = new Tracker(this, target);
				referenceTrackers.Push(tracker);
				return tracker;
			}
		}

		private void PopTracker(Tracker tracker)
		{
			lock (referenceTrackersLock)
			{
				if (referenceTrackers.Peek() != tracker)
				{
					throw new InvalidOperationException("Tracker being released is not the latest one. Make sure to release child trackers before releasing parent.");
				}
				referenceTrackers.Pop();
			}
		}

		private bool TrackerExists(object target)
		{
			foreach (Tracker referenceTracker in referenceTrackers)
			{
				if (referenceTracker.Target == target)
				{
					return true;
				}
			}
			return false;
		}
	}
}
