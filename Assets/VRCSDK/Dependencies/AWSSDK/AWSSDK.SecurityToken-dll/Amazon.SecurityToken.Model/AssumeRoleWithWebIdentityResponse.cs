using Amazon.Runtime;

namespace Amazon.SecurityToken.Model
{
	public class AssumeRoleWithWebIdentityResponse : AmazonWebServiceResponse
	{
		private AssumedRoleUser _assumedRoleUser;

		private string _audience;

		private Credentials _credentials;

		private int? _packedPolicySize;

		private string _provider;

		private string _subjectFromWebIdentityToken;

		public AssumedRoleUser AssumedRoleUser
		{
			get
			{
				return _assumedRoleUser;
			}
			set
			{
				_assumedRoleUser = value;
			}
		}

		public string Audience
		{
			get
			{
				return _audience;
			}
			set
			{
				_audience = value;
			}
		}

		public Credentials Credentials
		{
			get
			{
				return _credentials;
			}
			set
			{
				_credentials = value;
			}
		}

		public int PackedPolicySize
		{
			get
			{
				return _packedPolicySize.GetValueOrDefault();
			}
			set
			{
				_packedPolicySize = value;
			}
		}

		public string Provider
		{
			get
			{
				return _provider;
			}
			set
			{
				_provider = value;
			}
		}

		public string SubjectFromWebIdentityToken
		{
			get
			{
				return _subjectFromWebIdentityToken;
			}
			set
			{
				_subjectFromWebIdentityToken = value;
			}
		}

		internal bool IsSetAssumedRoleUser()
		{
			return _assumedRoleUser != null;
		}

		internal bool IsSetAudience()
		{
			return _audience != null;
		}

		internal bool IsSetCredentials()
		{
			return _credentials != null;
		}

		internal bool IsSetPackedPolicySize()
		{
			return _packedPolicySize.HasValue;
		}

		internal bool IsSetProvider()
		{
			return _provider != null;
		}

		internal bool IsSetSubjectFromWebIdentityToken()
		{
			return _subjectFromWebIdentityToken != null;
		}

		public AssumeRoleWithWebIdentityResponse()
			: this()
		{
		}
	}
}
