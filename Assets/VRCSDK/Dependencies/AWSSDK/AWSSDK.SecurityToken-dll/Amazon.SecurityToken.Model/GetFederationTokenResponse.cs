using Amazon.Runtime;

namespace Amazon.SecurityToken.Model
{
	public class GetFederationTokenResponse : AmazonWebServiceResponse
	{
		private Credentials _credentials;

		private FederatedUser _federatedUser;

		private int? _packedPolicySize;

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

		public FederatedUser FederatedUser
		{
			get
			{
				return _federatedUser;
			}
			set
			{
				_federatedUser = value;
			}
		}

		public int PackedPolicySize
		{
			get
			{
				return _packedPolicySize.GetValueOrDefault();
			}
			set
			{
				_packedPolicySize = value;
			}
		}

		internal bool IsSetCredentials()
		{
			return _credentials != null;
		}

		internal bool IsSetFederatedUser()
		{
			return _federatedUser != null;
		}

		internal bool IsSetPackedPolicySize()
		{
			return _packedPolicySize.HasValue;
		}

		public GetFederationTokenResponse()
			: this()
		{
		}
	}
}
