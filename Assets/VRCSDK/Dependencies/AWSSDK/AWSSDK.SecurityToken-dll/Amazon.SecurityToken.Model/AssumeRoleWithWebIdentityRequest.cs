namespace Amazon.SecurityToken.Model
{
	public class AssumeRoleWithWebIdentityRequest : AmazonSecurityTokenServiceRequest
	{
		private int? _durationSeconds;

		private string _policy;

		private string _providerId;

		private string _roleArn;

		private string _roleSessionName;

		private string _webIdentityToken;

		public int DurationSeconds
		{
			get
			{
				return _durationSeconds.GetValueOrDefault();
			}
			set
			{
				_durationSeconds = value;
			}
		}

		public string Policy
		{
			get
			{
				return _policy;
			}
			set
			{
				_policy = value;
			}
		}

		public string ProviderId
		{
			get
			{
				return _providerId;
			}
			set
			{
				_providerId = value;
			}
		}

		public string RoleArn
		{
			get
			{
				return _roleArn;
			}
			set
			{
				_roleArn = value;
			}
		}

		public string RoleSessionName
		{
			get
			{
				return _roleSessionName;
			}
			set
			{
				_roleSessionName = value;
			}
		}

		public string WebIdentityToken
		{
			get
			{
				return _webIdentityToken;
			}
			set
			{
				_webIdentityToken = value;
			}
		}

		internal bool IsSetDurationSeconds()
		{
			return _durationSeconds.HasValue;
		}

		internal bool IsSetPolicy()
		{
			return _policy != null;
		}

		internal bool IsSetProviderId()
		{
			return _providerId != null;
		}

		internal bool IsSetRoleArn()
		{
			return _roleArn != null;
		}

		internal bool IsSetRoleSessionName()
		{
			return _roleSessionName != null;
		}

		internal bool IsSetWebIdentityToken()
		{
			return _webIdentityToken != null;
		}
	}
}
