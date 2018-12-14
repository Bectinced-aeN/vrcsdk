namespace Amazon.S3.Model
{
	public class FilterRule
	{
		private string _name;

		private string _value;

		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		public string Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}

		public FilterRule()
			: this(null, null)
		{
		}

		public FilterRule(string name, string value)
		{
			_name = name;
			_value = value;
		}

		internal bool IsSetName()
		{
			return _name != null;
		}

		internal bool IsSetValue()
		{
			return _value != null;
		}
	}
}
