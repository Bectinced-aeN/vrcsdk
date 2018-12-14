using Amazon.Runtime;
using System.Collections.Generic;

namespace Amazon.CognitoIdentity.Model
{
	public class LookupDeveloperIdentityResponse : AmazonWebServiceResponse
	{
		private List<string> _developerUserIdentifierList = new List<string>();

		private string _identityId;

		private string _nextToken;

		public List<string> DeveloperUserIdentifierList
		{
			get
			{
				return _developerUserIdentifierList;
			}
			set
			{
				_developerUserIdentifierList = value;
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

		internal bool IsSetDeveloperUserIdentifierList()
		{
			if (_developerUserIdentifierList != null)
			{
				return _developerUserIdentifierList.Count > 0;
			}
			return false;
		}

		internal bool IsSetIdentityId()
		{
			return _identityId != null;
		}

		internal bool IsSetNextToken()
		{
			return _nextToken != null;
		}
	}
}
