using Amazon.Runtime;

namespace Amazon.SecurityToken.Model
{
	public class GetCallerIdentityResponse : AmazonWebServiceResponse
	{
		private string _account;

		private string _arn;

		private string _userId;

		public string Account
		{
			get
			{
				return _account;
			}
			set
			{
				_account = value;
			}
		}

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

		public string UserId
		{
			get
			{
				return _userId;
			}
			set
			{
				_userId = value;
			}
		}

		internal bool IsSetAccount()
		{
			return _account != null;
		}

		internal bool IsSetArn()
		{
			return _arn != null;
		}

		internal bool IsSetUserId()
		{
			return _userId != null;
		}

		public GetCallerIdentityResponse()
			: this()
		{
		}
	}
}
