namespace Amazon.S3.Model
{
	public class RoutingRule
	{
		private RoutingRuleCondition condition;

		private RoutingRuleRedirect redirect;

		public RoutingRuleCondition Condition
		{
			get
			{
				return condition;
			}
			set
			{
				condition = value;
			}
		}

		public RoutingRuleRedirect Redirect
		{
			get
			{
				return redirect;
			}
			set
			{
				redirect = value;
			}
		}

		internal bool IsSetCondition()
		{
			return condition != null;
		}

		internal bool IsSetRedirect()
		{
			return redirect != null;
		}
	}
}
