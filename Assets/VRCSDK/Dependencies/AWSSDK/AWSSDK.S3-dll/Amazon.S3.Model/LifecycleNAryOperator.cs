using System.Collections.Generic;

namespace Amazon.S3.Model
{
	public abstract class LifecycleNAryOperator : LifecycleFilterPredicate
	{
		public List<LifecycleFilterPredicate> Operands
		{
			get;
			set;
		}

		internal bool IsSetOperands()
		{
			return Operands != null;
		}
	}
}
