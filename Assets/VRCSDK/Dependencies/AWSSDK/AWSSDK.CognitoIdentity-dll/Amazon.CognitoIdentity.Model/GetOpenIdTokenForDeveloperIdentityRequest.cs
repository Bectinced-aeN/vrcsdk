using System.Collections.Generic;

namespace Amazon.CognitoIdentity.Model
{
	public class GetOpenIdTokenForDeveloperIdentityRequest : AmazonCognitoIdentityRequest
	{
		private string _identityId;

		private string _identityPoolId;

		private Dictionary<string, string> _logins = new Dictionary<string, string>();

		private long? _tokenDuration;

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

		public string IdentityPoolId
		{
			get
			{
				return _identityPoolId;
			}
			set
			{
				_identityPoolId = value;
			}
		}

		public Dictionary<string, string> Logins
		{
			get
			{
				return _logins;
			}
			set
			{
				_logins = value;
			}
		}

		public long TokenDuration
		{
			get
			{
				return _tokenDuration.GetValueOrDefault();
			}
			set
			{
				_tokenDuration = value;
			}
		}

		internal bool IsSetIdentityId()
		{
			return _identityId != null;
		}

		internal bool IsSetIdentityPoolId()
		{
			return _identityPoolId != null;
		}

		internal bool IsSetLogins()
		{
			if (_logins != null)
			{
				return _logins.Count > 0;
			}
			return false;
		}

		internal bool IsSetTokenDuration()
		{
			return _tokenDuration.HasValue;
		}
	}
}
