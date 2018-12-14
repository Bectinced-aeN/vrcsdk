using Amazon.Runtime;
using System;

namespace Amazon.SecurityToken.Model
{
	public class Credentials : AWSCredentials
	{
		private ImmutableCredentials _credentials;

		private string _accessKeyId;

		private DateTime? _expiration;

		private string _secretAccessKey;

		private string _sessionToken;

		public string AccessKeyId
		{
			get
			{
				return _accessKeyId;
			}
			set
			{
				_accessKeyId = value;
			}
		}

		public DateTime Expiration
		{
			get
			{
				return _expiration.GetValueOrDefault();
			}
			set
			{
				_expiration = value;
			}
		}

		public string SecretAccessKey
		{
			get
			{
				return _secretAccessKey;
			}
			set
			{
				_secretAccessKey = value;
			}
		}

		public string SessionToken
		{
			get
			{
				return _sessionToken;
			}
			set
			{
				_sessionToken = value;
			}
		}

		public override ImmutableCredentials GetCredentials()
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected O, but got Unknown
			if (_credentials == null)
			{
				_credentials = new ImmutableCredentials(AccessKeyId, SecretAccessKey, SessionToken);
			}
			return _credentials.Copy();
		}

		public Credentials()
			: this()
		{
		}

		public Credentials(string accessKeyId, string secretAccessKey, string sessionToken, DateTime expiration)
			: this()
		{
			_accessKeyId = accessKeyId;
			_secretAccessKey = secretAccessKey;
			_sessionToken = sessionToken;
			_expiration = expiration;
		}

		internal bool IsSetAccessKeyId()
		{
			return _accessKeyId != null;
		}

		internal bool IsSetExpiration()
		{
			return _expiration.HasValue;
		}

		internal bool IsSetSecretAccessKey()
		{
			return _secretAccessKey != null;
		}

		internal bool IsSetSessionToken()
		{
			return _sessionToken != null;
		}
	}
}
