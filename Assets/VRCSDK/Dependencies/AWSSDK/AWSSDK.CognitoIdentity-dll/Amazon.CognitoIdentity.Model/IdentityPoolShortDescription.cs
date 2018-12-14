namespace Amazon.CognitoIdentity.Model
{
	public class IdentityPoolShortDescription
	{
		private string _identityPoolId;

		private string _identityPoolName;

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

		public string IdentityPoolName
		{
			get
			{
				return _identityPoolName;
			}
			set
			{
				_identityPoolName = value;
			}
		}

		internal bool IsSetIdentityPoolId()
		{
			return _identityPoolId != null;
		}

		internal bool IsSetIdentityPoolName()
		{
			return _identityPoolName != null;
		}
	}
}
