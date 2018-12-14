namespace Amazon.SecurityToken.Model
{
	public class AssumeRoleWithSAMLRequest : AmazonSecurityTokenServiceRequest
	{
		private int? _durationSeconds;

		private string _policy;

		private string _principalArn;

		private string _roleArn;

		private string _samlAssertion;

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

		public string PrincipalArn
		{
			get
			{
				return _principalArn;
			}
			set
			{
				_principalArn = value;
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

		public string SAMLAssertion
		{
			get
			{
				return _samlAssertion;
			}
			set
			{
				_samlAssertion = value;
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

		internal bool IsSetPrincipalArn()
		{
			return _principalArn != null;
		}

		internal bool IsSetRoleArn()
		{
			return _roleArn != null;
		}

		internal bool IsSetSAMLAssertion()
		{
			return _samlAssertion != null;
		}
	}
}
