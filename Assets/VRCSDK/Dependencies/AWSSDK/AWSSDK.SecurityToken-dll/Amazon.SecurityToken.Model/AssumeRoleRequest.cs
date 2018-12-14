namespace Amazon.SecurityToken.Model
{
	public class AssumeRoleRequest : AmazonSecurityTokenServiceRequest
	{
		private int? _durationSeconds;

		private string _externalId;

		private string _policy;

		private string _roleArn;

		private string _roleSessionName;

		private string _serialNumber;

		private string _tokenCode;

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

		public string ExternalId
		{
			get
			{
				return _externalId;
			}
			set
			{
				_externalId = value;
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

		public string SerialNumber
		{
			get
			{
				return _serialNumber;
			}
			set
			{
				_serialNumber = value;
			}
		}

		public string TokenCode
		{
			get
			{
				return _tokenCode;
			}
			set
			{
				_tokenCode = value;
			}
		}

		internal bool IsSetDurationSeconds()
		{
			return _durationSeconds.HasValue;
		}

		internal bool IsSetExternalId()
		{
			return _externalId != null;
		}

		internal bool IsSetPolicy()
		{
			return _policy != null;
		}

		internal bool IsSetRoleArn()
		{
			return _roleArn != null;
		}

		internal bool IsSetRoleSessionName()
		{
			return _roleSessionName != null;
		}

		internal bool IsSetSerialNumber()
		{
			return _serialNumber != null;
		}

		internal bool IsSetTokenCode()
		{
			return _tokenCode != null;
		}
	}
}
