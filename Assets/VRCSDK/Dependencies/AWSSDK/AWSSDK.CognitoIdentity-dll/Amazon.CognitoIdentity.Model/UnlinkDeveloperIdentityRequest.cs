namespace Amazon.CognitoIdentity.Model
{
	public class UnlinkDeveloperIdentityRequest : AmazonCognitoIdentityRequest
	{
		private string _developerProviderName;

		private string _developerUserIdentifier;

		private string _identityId;

		private string _identityPoolId;

		public string DeveloperProviderName
		{
			get
			{
				return _developerProviderName;
			}
			set
			{
				_developerProviderName = value;
			}
		}

		public string DeveloperUserIdentifier
		{
			get
			{
				return _developerUserIdentifier;
			}
			set
			{
				_developerUserIdentifier = value;
			}
		}

		public string IdentityId
		{
			get
			{
				return _identityId;
			}
			set
			{
				_identityId = value;
			}
		}

		public string IdentityPoolId
		{
			get
			{
				return _identityPoolId;
			}
			set
			{
				_identityPoolId = value;
			}
		}

		internal bool IsSetDeveloperProviderName()
		{
			return _developerProviderName != null;
		}

		internal bool IsSetDeveloperUserIdentifier()
		{
			return _developerUserIdentifier != null;
		}

		internal bool IsSetIdentityId()
		{
			return _identityId != null;
		}

		internal bool IsSetIdentityPoolId()
		{
			return _identityPoolId != null;
		}
	}
}
