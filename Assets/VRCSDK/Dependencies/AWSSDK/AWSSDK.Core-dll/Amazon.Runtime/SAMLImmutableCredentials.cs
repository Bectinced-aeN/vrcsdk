using Amazon.Runtime.Internal.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using ThirdParty.Json.LitJson;

namespace Amazon.Runtime
{
	public class SAMLImmutableCredentials : ImmutableCredentials
	{
		private const string AccessKeyProperty = "AccessKey";

		private const string SecretKeyProperty = "SecretKey";

		private const string TokenProperty = "Token";

		private const string ExpiresProperty = "Expires";

		private const string SubjectProperty = "Subject";

		public DateTime Expires
		{
			get;
			private set;
		}

		public string Subject
		{
			get;
			private set;
		}

		public SAMLImmutableCredentials(string awsAccessKeyId, string awsSecretAccessKey, string token, DateTime expires, string subject)
			: base(awsAccessKeyId, awsSecretAccessKey, token)
		{
			Expires = expires;
			Subject = subject;
		}

		public SAMLImmutableCredentials(ImmutableCredentials credentials, DateTime expires, string subject)
			: base(credentials.AccessKey, credentials.SecretKey, credentials.Token)
		{
			Expires = expires;
			Subject = subject;
		}

		public override int GetHashCode()
		{
			return Hashing.Hash(base.AccessKey, base.SecretKey, base.Token, Subject);
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			SAMLImmutableCredentials sAMLImmutableCredentials = obj as SAMLImmutableCredentials;
			if (sAMLImmutableCredentials == null)
			{
				return false;
			}
			if (base.Equals(obj))
			{
				return string.Equals(Subject, sAMLImmutableCredentials.Subject, StringComparison.Ordinal);
			}
			return false;
		}

		internal string ToJson()
		{
			return JsonMapper.ToJson(new Dictionary<string, string>
			{
				{
					"AccessKey",
					base.AccessKey
				},
				{
					"SecretKey",
					base.SecretKey
				},
				{
					"Token",
					base.Token
				},
				{
					"Expires",
					Expires.ToString("u", CultureInfo.InvariantCulture)
				},
				{
					"Subject",
					Subject
				}
			});
		}

		internal static SAMLImmutableCredentials FromJson(string json)
		{
			try
			{
				JsonData jsonData = JsonMapper.ToObject(json);
				DateTime dateTime = DateTime.Parse((string)jsonData["Expires"], CultureInfo.InvariantCulture).ToUniversalTime();
				if (dateTime <= DateTime.UtcNow)
				{
					Logger.GetLogger(typeof(SAMLImmutableCredentials)).InfoFormat("Skipping serialized credentials due to expiry.");
					return null;
				}
				string awsAccessKeyId = (string)jsonData["AccessKey"];
				string awsSecretAccessKey = (string)jsonData["SecretKey"];
				string token = (string)jsonData["Token"];
				string subject = (string)jsonData["Subject"];
				return new SAMLImmutableCredentials(awsAccessKeyId, awsSecretAccessKey, token, dateTime, subject);
			}
			catch (Exception exception)
			{
				Logger.GetLogger(typeof(SAMLImmutableCredentials)).Error(exception, "Error during deserialization");
			}
			return null;
		}
	}
}
