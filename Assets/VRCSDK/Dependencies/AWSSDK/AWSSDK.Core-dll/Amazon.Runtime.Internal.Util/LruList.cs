namespace Amazon.Runtime.Internal.Util
{
	public class LruList<TKey, TValue>
	{
		public LruListItem<TKey, TValue> Head
		{
			get;
			private set;
		}

		public LruListItem<TKey, TValue> Tail
		{
			get;
			private set;
		}

		public void Add(LruListItem<TKey, TValue> item)
		{
			if (Head == null)
			{
				Head = item;
				Tail = item;
				item.Previous = null;
				item.Next = null;
			}
			else
			{
				Head.Previous = item;
				item.Next = Head;
				item.Previous = null;
				Head = item;
			}
		}

		public void Remove(LruListItem<TKey, TValue> item)
		{
			if (Head == item || Tail == item)
			{
				if (Head == item)
				{
					Head = item.Next;
					if (Head != null)
					{
						Head.Previous = null;
					}
				}
				if (Tail == item)
				{
					Tail = item.Previous;
					if (Tail != null)
					{
						Tail.Next = null;
					}
				}
			}
			else
			{
				item.Previous.Next = item.Next;
				item.Next.Previous = item.Previous;
			}
			item.Previous = null;
			item.Next = null;
		}

		public void Touch(LruListItem<TKey, TValue> item)
		{
			Remove(item);
			Add(item);
		}

		public TKey EvictOldest()
		{
			TKey result = default(TKey);
			if (Tail != null)
			{
				result = Tail.Key;
				Remove(Tail);
			}
			return result;
		}

		internal void Clear()
		{
			Head = null;
			Tail = null;
		}
	}
}
