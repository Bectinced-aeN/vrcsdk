namespace Amazon.S3.Model.Internal
{
	internal interface IAnalyticsPredicateVisitor
	{
		void Visit(AnalyticsPrefixPredicate analyticsPrefixPredicate);

		void visit(AnalyticsTagPredicate analyticsTagPredicate);

		void visit(AnalyticsAndOperator analyticsAndOperatorPredicate);
	}
}
