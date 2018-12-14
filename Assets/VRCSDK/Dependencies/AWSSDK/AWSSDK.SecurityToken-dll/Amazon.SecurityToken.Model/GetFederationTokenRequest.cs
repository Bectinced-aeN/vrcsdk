namespace Amazon.SecurityToken.Model
{
	public class GetFederationTokenRequest : AmazonSecurityTokenServiceRequest
	{
		private int? _durationSeconds;

		private string _name;

		private string _policy;

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

		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
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

		public GetFederationTokenRequest()
		{
		}

		public GetFederationTokenRequest(string name)
		{
			_name = name;
		}

		internal bool IsSetDurationSeconds()
		{
			return _durationSeconds.HasValue;
		}

		internal bool IsSetName()
		{
			return _name != null;
		}

		internal bool IsSetPolicy()
		{
			return _policy != null;
		}
	}
}
