using Amazon.Runtime;

namespace Amazon.SecurityToken.Model
{
	public class GetSessionTokenResponse : AmazonWebServiceResponse
	{
		private Credentials _credentials;

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

		internal bool IsSetCredentials()
		{
			return _credentials != null;
		}

		public GetSessionTokenResponse()
			: this()
		{
		}
	}
}
