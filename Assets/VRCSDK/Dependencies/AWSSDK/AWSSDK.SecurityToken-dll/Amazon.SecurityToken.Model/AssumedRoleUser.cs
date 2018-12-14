namespace Amazon.SecurityToken.Model
{
	public class AssumedRoleUser
	{
		private string _arn;

		private string _assumedRoleId;

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

		public string AssumedRoleId
		{
			get
			{
				return _assumedRoleId;
			}
			set
			{
				_assumedRoleId = value;
			}
		}

		internal bool IsSetArn()
		{
			return _arn != null;
		}

		internal bool IsSetAssumedRoleId()
		{
			return _assumedRoleId != null;
		}
	}
}
