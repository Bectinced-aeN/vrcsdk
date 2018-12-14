namespace Amazon.SecurityToken.Model
{
	public class FederatedUser
	{
		private string _arn;

		private string _federatedUserId;

		public string Arn
		{
			get
			{
				return _arn;
			}
			set
			{
				_arn = value;
			}
		}

		public string FederatedUserId
		{
			get
			{
				return _federatedUserId;
			}
			set
			{
				_federatedUserId = value;
			}
		}

		public FederatedUser()
		{
		}

		public FederatedUser(string federatedUserId, string arn)
		{
			_federatedUserId = federatedUserId;
			_arn = arn;
		}

		internal bool IsSetArn()
		{
			return _arn != null;
		}

		internal bool IsSetFederatedUserId()
		{
			return _federatedUserId != null;
		}
	}
}
