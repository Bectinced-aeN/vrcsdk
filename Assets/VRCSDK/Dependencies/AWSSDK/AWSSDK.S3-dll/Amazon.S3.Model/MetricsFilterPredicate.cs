using Amazon.S3.Model.Internal;

namespace Amazon.S3.Model
{
	public abstract class MetricsFilterPredicate
	{
		internal abstract void Accept(IMetricsPredicateVisitor metricsPredicateVisitor);
	}
}
