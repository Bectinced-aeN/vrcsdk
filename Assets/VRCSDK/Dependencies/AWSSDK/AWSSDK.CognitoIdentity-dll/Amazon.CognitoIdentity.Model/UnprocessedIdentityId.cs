namespace Amazon.CognitoIdentity.Model
{
	public class UnprocessedIdentityId
	{
		private ErrorCode _errorCode;

		private string _identityId;

		public ErrorCode ErrorCode
		{
			get
			{
				return _errorCode;
			}
			set
			{
				_errorCode = value;
			}
		}

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

		internal bool IsSetErrorCode()
		{
			return _errorCode != null;
		}

		internal bool IsSetIdentityId()
		{
			return _identityId != null;
		}
	}
}
