using Amazon.S3.Model.Internal;

namespace Amazon.S3.Model
{
	public class LifecyclePrefixPredicate : LifecycleFilterPredicate
	{
		public string Prefix
		{
			get;
			set;
		}

		internal bool IsSetPrefix()
		{
			return Prefix != null;
		}

		internal override void Accept(ILifecyclePredicateVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}
