using Amazon.Runtime;

namespace Amazon.SecurityToken.Model
{
	public class AssumeRoleWithSAMLResponse : AmazonWebServiceResponse
	{
		private AssumedRoleUser _assumedRoleUser;

		private string _audience;

		private Credentials _credentials;

		private string _issuer;

		private string _nameQualifier;

		private int? _packedPolicySize;

		private string _subject;

		private string _subjectType;

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

		public string Issuer
		{
			get
			{
				return _issuer;
			}
			set
			{
				_issuer = value;
			}
		}

		public string NameQualifier
		{
			get
			{
				return _nameQualifier;
			}
			set
			{
				_nameQualifier = value;
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

		public string Subject
		{
			get
			{
				return _subject;
			}
			set
			{
				_subject = value;
			}
		}

		public string SubjectType
		{
			get
			{
				return _subjectType;
			}
			set
			{
				_subjectType = value;
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

		internal bool IsSetIssuer()
		{
			return _issuer != null;
		}

		internal bool IsSetNameQualifier()
		{
			return _nameQualifier != null;
		}

		internal bool IsSetPackedPolicySize()
		{
			return _packedPolicySize.HasValue;
		}

		internal bool IsSetSubject()
		{
			return _subject != null;
		}

		internal bool IsSetSubjectType()
		{
			return _subjectType != null;
		}

		public AssumeRoleWithSAMLResponse()
			: this()
		{
		}
	}
}
