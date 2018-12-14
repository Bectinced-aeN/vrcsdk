using Amazon.S3.Model.Internal;

namespace Amazon.S3.Model
{
	public sealed class MetricsTagPredicate : MetricsFilterPredicate
	{
		private readonly Tag tag;

		public Tag Tag => tag;

		public MetricsTagPredicate(Tag tag)
		{
			this.tag = tag;
		}

		internal override void Accept(IMetricsPredicateVisitor metricsPredicateVisitor)
		{
			metricsPredicateVisitor.visit(this);
		}
	}
}
