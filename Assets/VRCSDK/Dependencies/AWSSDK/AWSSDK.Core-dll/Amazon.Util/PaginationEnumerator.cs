using System;
using System.Collections;
using System.Collections.Generic;

namespace Amazon.Util
{
	internal class PaginationEnumerator<U> : IEnumerator<U>, IDisposable, IEnumerator
	{
		private PaginatedResource<U> paginatedResource;

		private int position = -1;

		private static Marker<U> blankSpot = new Marker<U>(new List<U>(), null);

		private Marker<U> currentSpot = blankSpot;

		private bool started;

		object IEnumerator.Current
		{
			get
			{
				return Current;
			}
		}

		public U Current
		{
			get
			{
				try
				{
					return currentSpot.Data[position];
				}
				catch (IndexOutOfRangeException)
				{
					throw new InvalidOperationException();
				}
			}
		}

		internal PaginationEnumerator(PaginatedResource<U> paginatedResource)
		{
			this.paginatedResource = paginatedResource;
		}

		public bool MoveNext()
		{
			position++;
			while (position == currentSpot.Data.Count)
			{
				if (!started || !string.IsNullOrEmpty(currentSpot.NextToken))
				{
					currentSpot = paginatedResource.fetcher(currentSpot.NextToken);
					position = 0;
					started = true;
				}
				else
				{
					currentSpot = blankSpot;
					position = -1;
				}
			}
			return position != -1;
		}

		public void Reset()
		{
			position = -1;
			currentSpot = new Marker<U>(new List<U>(), null);
			started = false;
		}

		public void Dispose()
		{
		}
	}
}
