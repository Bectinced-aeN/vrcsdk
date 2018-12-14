namespace Amazon.Auth.AccessControlPolicy
{
	public class Condition
	{
		private string type;

		private string conditionKey;

		private string[] values;

		public string Type
		{
			get
			{
				return type;
			}
			set
			{
				type = value;
			}
		}

		public string ConditionKey
		{
			get
			{
				return conditionKey;
			}
			set
			{
				conditionKey = value;
			}
		}

		public string[] Values
		{
			get
			{
				return values;
			}
			set
			{
				values = value;
			}
		}

		public Condition()
		{
		}

		public Condition(string type, string conditionKey, params string[] values)
		{
			this.type = type;
			this.conditionKey = conditionKey;
			this.values = values;
		}
	}
}
