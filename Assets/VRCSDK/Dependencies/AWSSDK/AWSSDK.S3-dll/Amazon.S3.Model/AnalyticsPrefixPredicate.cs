using Amazon.S3.Model.Internal;

namespace Amazon.S3.Model
{
	public sealed class AnalyticsPrefixPredicate : AnalyticsFilterPredicate
	{
		private readonly string prefix;

		public string Prefix => prefix;

		public AnalyticsPrefixPredicate(string prefix)
		{
			this.prefix = prefix;
		}

		internal override void Accept(IAnalyticsPredicateVisitor analyticsPredicateVisitor)
		{
			analyticsPredicateVisitor.Visit(this);
		}
	}
}
