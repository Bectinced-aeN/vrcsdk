namespace Amazon.S3.Model.Internal
{
	internal interface IMetricsPredicateVisitor
	{
		void Visit(MetricsPrefixPredicate metricsPrefixPredicate);

		void visit(MetricsTagPredicate metricsTagPredicate);

		void visit(MetricsAndOperator metricsAndOperatorPredicate);
	}
}
