using System.Collections.Generic;

namespace Amazon.CognitoIdentity.Model
{
	public class GetOpenIdTokenRequest : AmazonCognitoIdentityRequest
	{
		private string _identityId;

		private Dictionary<string, string> _logins = new Dictionary<string, string>();

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

		internal bool IsSetIdentityId()
		{
			return _identityId != null;
		}

		internal bool IsSetLogins()
		{
			if (_logins != null)
			{
				return _logins.Count > 0;
			}
			return false;
		}
	}
}
