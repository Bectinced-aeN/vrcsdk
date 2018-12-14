namespace Amazon.CognitoIdentity.Model
{
	public class ListIdentitiesRequest : AmazonCognitoIdentityRequest
	{
		private bool? _hideDisabled;

		private string _identityPoolId;

		private int? _maxResults;

		private string _nextToken;

		public bool HideDisabled
		{
			get
			{
				return _hideDisabled.GetValueOrDefault();
			}
			set
			{
				_hideDisabled = value;
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

		internal bool IsSetHideDisabled()
		{
			return _hideDisabled.HasValue;
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
