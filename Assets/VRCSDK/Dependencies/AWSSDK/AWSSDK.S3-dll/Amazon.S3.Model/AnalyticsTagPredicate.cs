using Amazon.S3.Model.Internal;

namespace Amazon.S3.Model
{
	public sealed class AnalyticsTagPredicate : AnalyticsFilterPredicate
	{
		private readonly Tag tag;

		public Tag Tag => tag;

		public AnalyticsTagPredicate(Tag tag)
		{
			this.tag = tag;
		}

		internal override void Accept(IAnalyticsPredicateVisitor analyticsPredicateVisitor)
		{
			analyticsPredicateVisitor.visit(this);
		}
	}
}
