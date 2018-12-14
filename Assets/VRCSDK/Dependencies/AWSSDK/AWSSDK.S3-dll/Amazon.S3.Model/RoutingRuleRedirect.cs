namespace Amazon.S3.Model
{
	public class RoutingRuleRedirect
	{
		private string hostName;

		private string httpRedirectCode;

		private string protocol;

		private string replaceKeyPrefixWith;

		private string replaceKeyWith;

		public string HostName
		{
			get
			{
				return hostName;
			}
			set
			{
				hostName = value;
			}
		}

		public string HttpRedirectCode
		{
			get
			{
				return httpRedirectCode;
			}
			set
			{
				httpRedirectCode = value;
			}
		}

		public string Protocol
		{
			get
			{
				return protocol;
			}
			set
			{
				protocol = value;
			}
		}

		public string ReplaceKeyPrefixWith
		{
			get
			{
				return replaceKeyPrefixWith;
			}
			set
			{
				replaceKeyPrefixWith = value;
			}
		}

		public string ReplaceKeyWith
		{
			get
			{
				return replaceKeyWith;
			}
			set
			{
				replaceKeyWith = value;
			}
		}

		internal bool IsSetHostName()
		{
			return hostName != null;
		}

		internal bool IsSetHttpRedirectCode()
		{
			return httpRedirectCode != null;
		}

		internal bool IsSetProtocol()
		{
			return protocol != null;
		}

		internal bool IsSetReplaceKeyPrefixWith()
		{
			return replaceKeyPrefixWith != null;
		}

		internal bool IsSetReplaceKeyWith()
		{
			return replaceKeyWith != null;
		}
	}
}
