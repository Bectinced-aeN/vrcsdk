using Amazon.Runtime;

namespace Amazon.CognitoIdentity.Model
{
	public class GetIdResponse : AmazonWebServiceResponse
	{
		private string _identityId;

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

		internal bool IsSetIdentityId()
		{
			return _identityId != null;
		}
	}
}
