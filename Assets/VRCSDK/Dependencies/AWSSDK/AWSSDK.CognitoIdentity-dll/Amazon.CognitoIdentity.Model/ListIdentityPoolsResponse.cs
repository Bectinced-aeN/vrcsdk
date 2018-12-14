using Amazon.Runtime;
using System.Collections.Generic;

namespace Amazon.CognitoIdentity.Model
{
	public class ListIdentityPoolsResponse : AmazonWebServiceResponse
	{
		private List<IdentityPoolShortDescription> _identityPools = new List<IdentityPoolShortDescription>();

		private string _nextToken;

		public List<IdentityPoolShortDescription> IdentityPools
		{
			get
			{
				return _identityPools;
			}
			set
			{
				_identityPools = value;
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

		internal bool IsSetIdentityPools()
		{
			if (_identityPools != null)
			{
				return _identityPools.Count > 0;
			}
			return false;
		}

		internal bool IsSetNextToken()
		{
			return _nextToken != null;
		}
	}
}
