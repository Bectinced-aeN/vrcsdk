namespace Amazon
{
	internal class ProxySection
	{
		public const string hostSection = "host";

		public const string portSection = "port";

		public const string usernameSection = "username";

		public const string passwordSection = "password";

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
	}
}
