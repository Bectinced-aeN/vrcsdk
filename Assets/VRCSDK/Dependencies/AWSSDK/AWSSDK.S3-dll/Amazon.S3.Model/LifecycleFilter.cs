namespace Amazon.S3.Model
{
	public class LifecycleFilter
	{
		public LifecycleFilterPredicate LifecycleFilterPredicate
		{
			get;
			set;
		}

		internal bool IsSetLifecycleFilterPredicate()
		{
			return LifecycleFilterPredicate != null;
		}
	}
}
