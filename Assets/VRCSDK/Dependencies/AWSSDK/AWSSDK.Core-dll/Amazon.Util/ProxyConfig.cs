using System.Collections.Generic;

namespace Amazon.Util
{
	public class ProxyConfig
	{
		public string Host
		{
			get;
			set;
		}

		public int? Port
		{
			get;
			set;
		}

		public string Username
		{
			get;
			set;
		}

		public string Password
		{
			get;
			set;
		}

		public List<string> BypassList
		{
			get;
			set;
		}

		public bool BypassOnLocal
		{
			get;
			set;
		}

		internal ProxyConfig()
		{
		}

		internal void Configure(ProxySection section)
		{
		}
	}
}
