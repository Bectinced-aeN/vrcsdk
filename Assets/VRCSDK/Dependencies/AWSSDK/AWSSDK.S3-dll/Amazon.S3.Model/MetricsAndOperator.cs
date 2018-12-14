using Amazon.S3.Model.Internal;
using System.Collections.Generic;

namespace Amazon.S3.Model
{
	public sealed class MetricsAndOperator : MetricsNAryOperator
	{
		public MetricsAndOperator(List<MetricsFilterPredicate> operands)
			: base(operands)
		{
		}

		internal override void Accept(IMetricsPredicateVisitor metricsPredicateVisitor)
		{
			metricsPredicateVisitor.visit(this);
		}
	}
}
