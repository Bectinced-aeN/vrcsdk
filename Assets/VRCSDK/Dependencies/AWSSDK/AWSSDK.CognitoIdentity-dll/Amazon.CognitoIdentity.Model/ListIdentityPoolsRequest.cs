namespace Amazon.CognitoIdentity.Model
{
	public class ListIdentityPoolsRequest : AmazonCognitoIdentityRequest
	{
		private int? _maxResults;

		private string _nextToken;

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
