namespace Amazon.CognitoIdentity.Model
{
	public class CognitoIdentityProviderInfo
	{
		private string _clientId;

		private string _providerName;

		public string ClientId
		{
			get
			{
				return _clientId;
			}
			set
			{
				_clientId = value;
			}
		}

		public string ProviderName
		{
			get
			{
				return _providerName;
			}
			set
			{
				_providerName = value;
			}
		}

		internal bool IsSetClientId()
		{
			return _clientId != null;
		}

		internal bool IsSetProviderName()
		{
			return _providerName != null;
		}
	}
}
