using Amazon.S3.Model.Internal;

namespace Amazon.S3.Model
{
	public class LifecycleTagPredicate : LifecycleFilterPredicate
	{
		public Tag Tag
		{
			get;
			set;
		}

		internal bool IsSetTag()
		{
			return Tag != null;
		}

		internal override void Accept(ILifecyclePredicateVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}
