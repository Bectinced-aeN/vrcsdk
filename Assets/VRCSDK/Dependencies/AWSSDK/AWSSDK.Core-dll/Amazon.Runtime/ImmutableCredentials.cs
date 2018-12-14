using Amazon.Runtime.Internal.Util;
using Amazon.Util;
using System;

namespace Amazon.Runtime
{
	public class ImmutableCredentials
	{
		public string AccessKey
		{
			get;
			private set;
		}

		public string SecretKey
		{
			get;
			private set;
		}

		public string Token
		{
			get;
			private set;
		}

		public bool UseToken => !string.IsNullOrEmpty(Token);

		public ImmutableCredentials(string awsAccessKeyId, string awsSecretAccessKey, string token)
		{
			if (string.IsNullOrEmpty(awsAccessKeyId))
			{
				throw new ArgumentNullException("awsAccessKeyId");
			}
			if (string.IsNullOrEmpty(awsSecretAccessKey))
			{
				throw new ArgumentNullException("awsSecretAccessKey");
			}
			AccessKey = awsAccessKeyId;
			SecretKey = awsSecretAccessKey;
			Token = (token ?? string.Empty);
		}

		private ImmutableCredentials()
		{
		}

		public ImmutableCredentials Copy()
		{
			return new ImmutableCredentials
			{
				AccessKey = AccessKey,
				SecretKey = SecretKey,
				Token = Token
			};
		}

		public override int GetHashCode()
		{
			return Hashing.Hash(AccessKey, SecretKey, Token);
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			ImmutableCredentials immutableCredentials = obj as ImmutableCredentials;
			if (immutableCredentials == null)
			{
				return false;
			}
			return AWSSDKUtils.AreEqual(new object[3]
			{
				AccessKey,
				SecretKey,
				Token
			}, new object[3]
			{
				immutableCredentials.AccessKey,
				immutableCredentials.SecretKey,
				immutableCredentials.Token
			});
		}
	}
}
