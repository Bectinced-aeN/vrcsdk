using Amazon.S3.Model.Internal;

namespace Amazon.S3.Model
{
	public class LifecycleAndOperator : LifecycleNAryOperator
	{
		internal override void Accept(ILifecyclePredicateVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}
