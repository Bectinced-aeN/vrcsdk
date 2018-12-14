namespace Amazon.CognitoIdentity.Model
{
	public class DescribeIdentityPoolRequest : AmazonCognitoIdentityRequest
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
