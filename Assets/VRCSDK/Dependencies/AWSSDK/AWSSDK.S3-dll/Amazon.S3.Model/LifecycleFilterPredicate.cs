using Amazon.S3.Model.Internal;

namespace Amazon.S3.Model
{
	public abstract class LifecycleFilterPredicate
	{
		internal abstract void Accept(ILifecyclePredicateVisitor visitor);
	}
}
