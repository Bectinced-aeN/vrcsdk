namespace Amazon.SecurityToken.Model
{
	public class GetSessionTokenRequest : AmazonSecurityTokenServiceRequest
	{
		private int? _durationSeconds;

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
