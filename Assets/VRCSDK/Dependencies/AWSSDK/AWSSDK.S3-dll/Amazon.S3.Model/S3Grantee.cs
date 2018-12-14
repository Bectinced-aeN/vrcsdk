namespace Amazon.S3.Model
{
	public class S3Grantee
	{
		private string displayName;

		private string emailAddress;

		private string canonicalUser;

		private string uRI;

		public GranteeType Type
		{
			get
			{
				if (IsSetEmailAddress())
				{
					return GranteeType.Email;
				}
				if (IsSetURI())
				{
					return GranteeType.Group;
				}
				if (IsSetCanonicalUser())
				{
					return GranteeType.CanonicalUser;
				}
				return null;
			}
		}

		public string DisplayName
		{
			get
			{
				return displayName;
			}
			set
			{
				displayName = value;
			}
		}

		public string EmailAddress
		{
			get
			{
				return emailAddress;
			}
			set
			{
				emailAddress = value;
			}
		}

		public string CanonicalUser
		{
			get
			{
				return canonicalUser;
			}
			set
			{
				canonicalUser = value;
			}
		}

		public string URI
		{
			get
			{
				return uRI;
			}
			set
			{
				uRI = value;
			}
		}

		internal bool IsSetType()
		{
			return Type != null;
		}

		internal bool IsSetDisplayName()
		{
			return displayName != null;
		}

		internal bool IsSetEmailAddress()
		{
			return emailAddress != null;
		}

		internal bool IsSetCanonicalUser()
		{
			return canonicalUser != null;
		}

		internal bool IsSetURI()
		{
			return uRI != null;
		}
	}
}
