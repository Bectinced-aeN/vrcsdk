using System;
using System.Collections.Generic;
using System.Globalization;

namespace Amazon.S3.Model
{
	public sealed class HeadersCollection
	{
		private readonly IDictionary<string, string> _values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		public string this[string name]
		{
			get
			{
				if (_values.TryGetValue(name, out string value))
				{
					return value;
				}
				return null;
			}
			set
			{
				if (value != null)
				{
					_values[name] = value;
				}
				else if (_values.ContainsKey(name))
				{
					_values.Remove(name);
				}
			}
		}

		public int Count => _values.Count;

		public ICollection<string> Keys => _values.Keys;

		public string CacheControl
		{
			get
			{
				return this["Cache-Control"];
			}
			set
			{
				this["Cache-Control"] = value;
			}
		}

		public string ContentDisposition
		{
			get
			{
				return this["Content-Disposition"];
			}
			set
			{
				this["Content-Disposition"] = value;
			}
		}

		public string ContentEncoding
		{
			get
			{
				return this["Content-Encoding"];
			}
			set
			{
				this["Content-Encoding"] = value;
			}
		}

		public long ContentLength
		{
			get
			{
				string text = this["Content-Length"];
				if (string.IsNullOrEmpty(text))
				{
					return -1L;
				}
				return long.Parse(text, CultureInfo.InvariantCulture);
			}
			set
			{
				this["Content-Length"] = value.ToString(CultureInfo.InvariantCulture);
			}
		}

		public string ContentMD5
		{
			get
			{
				return this["Content-MD5"];
			}
			set
			{
				this["Content-MD5"] = value;
			}
		}

		public string ContentType
		{
			get
			{
				return this["Content-Type"];
			}
			set
			{
				this["Content-Type"] = value;
			}
		}

		public DateTime? Expires
		{
			get
			{
				if (this["Expires"] == null)
				{
					return null;
				}
				return DateTime.Parse(this["Expires"], CultureInfo.InvariantCulture);
			}
			set
			{
				if (!value.HasValue)
				{
					this["Expires"] = null;
				}
				this["Expires"] = value.GetValueOrDefault().ToString("ddd, dd MMM yyyy HH:mm:ss \\G\\M\\T", CultureInfo.InvariantCulture);
			}
		}

		internal bool IsSetContentType()
		{
			return !string.IsNullOrEmpty(ContentType);
		}
	}
}
