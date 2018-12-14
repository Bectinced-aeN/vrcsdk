using Amazon.Runtime;
using System;

namespace Amazon.CognitoIdentity.Model
{
	public class Credentials : AWSCredentials
	{
		private ImmutableCredentials _credentials;

		private string _accessKeyId;

		private DateTime? _expiration;

		private string _secretKey;

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

		public string SecretKey
		{
			get
			{
				return _secretKey;
			}
			set
			{
				_secretKey = value;
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
			if (_credentials == null)
			{
				_credentials = new ImmutableCredentials(AccessKeyId, SecretKey, SessionToken);
			}
			return _credentials.Copy();
		}

		internal bool IsSetAccessKeyId()
		{
			return _accessKeyId != null;
		}

		internal bool IsSetExpiration()
		{
			return _expiration.HasValue;
		}

		internal bool IsSetSecretKey()
		{
			return _secretKey != null;
		}

		internal bool IsSetSessionToken()
		{
			return _sessionToken != null;
		}
	}
}
