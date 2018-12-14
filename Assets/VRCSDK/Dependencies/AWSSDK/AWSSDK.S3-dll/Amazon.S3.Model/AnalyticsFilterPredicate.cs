using Amazon.S3.Model.Internal;

namespace Amazon.S3.Model
{
	public abstract class AnalyticsFilterPredicate
	{
		internal abstract void Accept(IAnalyticsPredicateVisitor analyticsPredicateVisitor);
	}
}
