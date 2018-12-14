using System.Collections.Generic;

namespace Amazon.S3.Model
{
	public class LifecycleConfiguration
	{
		private List<LifecycleRule> rules = new List<LifecycleRule>();

		public List<LifecycleRule> Rules
		{
			get
			{
				return rules;
			}
			set
			{
				rules = value;
			}
		}

		internal bool IsSetRules()
		{
			return rules.Count > 0;
		}
	}
}
