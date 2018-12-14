using System.Collections.Generic;

namespace Amazon.CognitoIdentity.Model
{
	public class GetIdRequest : AmazonCognitoIdentityRequest
	{
		private string _accountId;

		private string _identityPoolId;

		private Dictionary<string, string> _logins = new Dictionary<string, string>();

		public string AccountId
		{
			get
			{
				return _accountId;
			}
			set
			{
				_accountId = value;
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

		internal bool IsSetAccountId()
		{
			return _accountId != null;
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
	}
}
