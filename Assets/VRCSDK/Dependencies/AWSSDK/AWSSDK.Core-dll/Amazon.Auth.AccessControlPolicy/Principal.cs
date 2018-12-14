using System;

namespace Amazon.Auth.AccessControlPolicy
{
	public class Principal
	{
		public static readonly Principal AllUsers = new Principal("*");

		public static readonly Principal Anonymous = new Principal("__ANONYMOUS__", "*");

		public const string AWS_PROVIDER = "AWS";

		public const string CANONICAL_USER_PROVIDER = "CanonicalUser";

		public const string FEDERATED_PROVIDER = "Federated";

		public const string SERVICE_PROVIDER = "Service";

		public const string ANONYMOUS_PROVIDER = "__ANONYMOUS__";

		private string id;

		private string provider;

		public string Provider
		{
			get
			{
				return provider;
			}
			set
			{
				provider = value;
			}
		}

		public string Id => id;

		public Principal(string accountId)
			: this("AWS", accountId)
		{
			if (accountId == null)
			{
				throw new ArgumentNullException("accountId");
			}
		}

		public Principal(string provider, string id)
			: this(provider, id, provider == "AWS")
		{
		}

		public Principal(string provider, string id, bool stripHyphen)
		{
			this.provider = provider;
			if (stripHyphen)
			{
				id = id.Replace("-", "");
			}
			this.id = id;
		}
	}
}
