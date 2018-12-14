namespace Amazon.CognitoIdentity.Model
{
	public class DeleteIdentityPoolRequest : AmazonCognitoIdentityRequest
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
