namespace Amazon.Runtime.Internal
{
	public class RetryCapacity
	{
		private readonly int _maxCapacity;

		public int AvailableCapacity
		{
			get;
			set;
		}

		public int MaxCapacity => _maxCapacity;

		public RetryCapacity(int maxCapacity)
		{
			_maxCapacity = maxCapacity;
			AvailableCapacity = maxCapacity;
		}
	}
}
