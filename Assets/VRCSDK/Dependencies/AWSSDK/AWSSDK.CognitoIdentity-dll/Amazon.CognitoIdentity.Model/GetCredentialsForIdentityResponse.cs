using Amazon.Runtime;

namespace Amazon.CognitoIdentity.Model
{
	public class GetCredentialsForIdentityResponse : AmazonWebServiceResponse
	{
		private Credentials _credentials;

		private string _identityId;

		public Credentials Credentials
		{
			get
			{
				return _credentials;
			}
			set
			{
				_credentials = value;
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

		internal bool IsSetCredentials()
		{
			return _credentials != null;
		}

		internal bool IsSetIdentityId()
		{
			return _identityId != null;
		}
	}
}
