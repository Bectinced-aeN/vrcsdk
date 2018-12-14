using System.Collections.Generic;

namespace Amazon.S3.Model
{
	public abstract class MetricsNAryOperator : MetricsFilterPredicate
	{
		private readonly List<MetricsFilterPredicate> operands;

		public List<MetricsFilterPredicate> Operands => operands;

		protected MetricsNAryOperator(List<MetricsFilterPredicate> operands)
		{
			this.operands = operands;
		}
	}
}
