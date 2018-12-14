namespace Amazon.CognitoIdentity.Model
{
	public class MergeDeveloperIdentitiesRequest : AmazonCognitoIdentityRequest
	{
		private string _destinationUserIdentifier;

		private string _developerProviderName;

		private string _identityPoolId;

		private string _sourceUserIdentifier;

		public string DestinationUserIdentifier
		{
			get
			{
				return _destinationUserIdentifier;
			}
			set
			{
				_destinationUserIdentifier = value;
			}
		}

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

		public string SourceUserIdentifier
		{
			get
			{
				return _sourceUserIdentifier;
			}
			set
			{
				_sourceUserIdentifier = value;
			}
		}

		internal bool IsSetDestinationUserIdentifier()
		{
			return _destinationUserIdentifier != null;
		}

		internal bool IsSetDeveloperProviderName()
		{
			return _developerProviderName != null;
		}

		internal bool IsSetIdentityPoolId()
		{
			return _identityPoolId != null;
		}

		internal bool IsSetSourceUserIdentifier()
		{
			return _sourceUserIdentifier != null;
		}
	}
}
