namespace Amazon.Runtime.Internal.Util
{
	public class LruListItem<TKey, TValue>
	{
		public TValue Value
		{
			get;
			set;
		}

		public TKey Key
		{
			get;
			private set;
		}

		public LruListItem<TKey, TValue> Next
		{
			get;
			set;
		}

		public LruListItem<TKey, TValue> Previous
		{
			get;
			set;
		}

		public LruListItem(TKey key, TValue value)
		{
			Key = key;
			Value = value;
		}
	}
}
