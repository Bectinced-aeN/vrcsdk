using Amazon.Runtime;
using System.Collections.Generic;

namespace Amazon.CognitoIdentity.Model
{
	public class DeleteIdentitiesResponse : AmazonWebServiceResponse
	{
		private List<UnprocessedIdentityId> _unprocessedIdentityIds = new List<UnprocessedIdentityId>();

		public List<UnprocessedIdentityId> UnprocessedIdentityIds
		{
			get
			{
				return _unprocessedIdentityIds;
			}
			set
			{
				_unprocessedIdentityIds = value;
			}
		}

		internal bool IsSetUnprocessedIdentityIds()
		{
			if (_unprocessedIdentityIds != null)
			{
				return _unprocessedIdentityIds.Count > 0;
			}
			return false;
		}
	}
}
