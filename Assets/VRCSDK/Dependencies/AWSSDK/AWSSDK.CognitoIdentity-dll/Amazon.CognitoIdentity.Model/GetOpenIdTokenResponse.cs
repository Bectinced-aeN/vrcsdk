using Amazon.Runtime;

namespace Amazon.CognitoIdentity.Model
{
	public class GetOpenIdTokenResponse : AmazonWebServiceResponse
	{
		private string _identityId;

		private string _token;

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

		public string Token
		{
			get
			{
				return _token;
			}
			set
			{
				_token = value;
			}
		}

		internal bool IsSetIdentityId()
		{
			return _identityId != null;
		}

		internal bool IsSetToken()
		{
			return _token != null;
		}
	}
}
