using System;
using System.Collections.Generic;

namespace Amazon.S3.Model
{
	public sealed class MetadataCollection
	{
		private IDictionary<string, string> values = new Dictionary<string, string>();

		public string this[string name]
		{
			get
			{
				if (!name.StartsWith("x-amz-meta-", StringComparison.OrdinalIgnoreCase))
				{
					name = "x-amz-meta-" + name;
				}
				if (values.TryGetValue(name, out string value))
				{
					return value;
				}
				return null;
			}
			set
			{
				if (!name.StartsWith("x-amz-meta-", StringComparison.OrdinalIgnoreCase))
				{
					name = "x-amz-meta-" + name;
				}
				values[name] = value;
			}
		}

		public int Count => values.Count;

		public ICollection<string> Keys => values.Keys;

		public void Add(string name, string value)
		{
			this[name] = value;
		}
	}
}
