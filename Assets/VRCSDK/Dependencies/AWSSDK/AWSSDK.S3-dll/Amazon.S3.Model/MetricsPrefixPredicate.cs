using Amazon.S3.Model.Internal;

namespace Amazon.S3.Model
{
	public sealed class MetricsPrefixPredicate : MetricsFilterPredicate
	{
		private readonly string prefix;

		public string Prefix => prefix;

		public MetricsPrefixPredicate(string prefix)
		{
			this.prefix = prefix;
		}

		internal override void Accept(IMetricsPredicateVisitor metricsPredicateVisitor)
		{
			metricsPredicateVisitor.Visit(this);
		}
	}
}
