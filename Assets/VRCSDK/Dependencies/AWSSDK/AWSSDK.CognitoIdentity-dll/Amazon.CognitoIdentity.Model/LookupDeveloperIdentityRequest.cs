namespace Amazon.CognitoIdentity.Model
{
	public class LookupDeveloperIdentityRequest : AmazonCognitoIdentityRequest
	{
		private string _developerUserIdentifier;

		private string _identityId;

		private string _identityPoolId;

		private int? _maxResults;

		private string _nextToken;

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

		public int MaxResults
		{
			get
			{
				return _maxResults.GetValueOrDefault();
			}
			set
			{
				_maxResults = value;
			}
		}

		public string NextToken
		{
			get
			{
				return _nextToken;
			}
			set
			{
				_nextToken = value;
			}
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

		internal bool IsSetMaxResults()
		{
			return _maxResults.HasValue;
		}

		internal bool IsSetNextToken()
		{
			return _nextToken != null;
		}
	}
}
