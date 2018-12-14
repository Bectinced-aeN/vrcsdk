namespace Amazon.CognitoIdentity.Model
{
	public class GetIdentityPoolRolesRequest : AmazonCognitoIdentityRequest
	{
		private string _identityPoolId;

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

		internal bool IsSetIdentityPoolId()
		{
			return _identityPoolId != null;
		}
	}
}
