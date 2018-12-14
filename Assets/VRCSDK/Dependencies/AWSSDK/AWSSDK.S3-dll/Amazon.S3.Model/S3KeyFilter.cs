using System.Collections.Generic;

namespace Amazon.S3.Model
{
	public class S3KeyFilter
	{
		private List<FilterRule> filterRules;

		public List<FilterRule> FilterRules
		{
			get
			{
				if (filterRules == null)
				{
					filterRules = new List<FilterRule>();
				}
				return filterRules;
			}
			set
			{
				filterRules = value;
			}
		}

		internal bool IsSetFilterRules()
		{
			if (filterRules != null)
			{
				return filterRules.Count > 0;
			}
			return false;
		}
	}
}
