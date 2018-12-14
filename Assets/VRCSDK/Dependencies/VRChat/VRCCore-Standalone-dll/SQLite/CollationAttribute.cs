using System;

namespace SQLite
{
	[AttributeUsage(AttributeTargets.Property)]
	public class CollationAttribute : Attribute
	{
		public string Value
		{
			get;
			private set;
		}

		public CollationAttribute(string collation)
		{
			Value = collation;
		}
	}
}
