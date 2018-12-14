using System.Collections.Generic;

namespace Amazon.S3.Model
{
	public class CORSConfiguration
	{
		private List<CORSRule> rules = new List<CORSRule>();

		public List<CORSRule> Rules
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
