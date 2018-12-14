using Amazon.Util;
using System;
using System.Globalization;
using System.Text;

namespace Amazon.Runtime.Internal.Auth
{
	public class AWS4SigningResult
	{
		private readonly string _awsAccessKeyId;

		private readonly DateTime _originalDateTime;

		private readonly string _signedHeaders;

		private readonly string _scope;

		private readonly byte[] _signingKey;

		private readonly byte[] _signature;

		public string AccessKeyId => _awsAccessKeyId;

		public string ISO8601DateTime => AWS4Signer.FormatDateTime(_originalDateTime, "yyyyMMddTHHmmssZ");

		public string ISO8601Date => AWS4Signer.FormatDateTime(_originalDateTime, "yyyyMMdd");

		public string SignedHeaders => _signedHeaders;

		public string Scope => _scope;

		public byte[] SigningKey
		{
			get
			{
				byte[] array = new byte[_signingKey.Length];
				_signingKey.CopyTo(array, 0);
				return array;
			}
		}

		public string Signature => AWSSDKUtils.ToHex(_signature, lowercase: true);

		public byte[] SignatureBytes
		{
			get
			{
				byte[] array = new byte[_signature.Length];
				_signature.CopyTo(array, 0);
				return array;
			}
		}

		public string ForAuthorizationHeader
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("AWS4-HMAC-SHA256");
				stringBuilder.AppendFormat(" {0}={1}/{2},", "Credential", AccessKeyId, Scope);
				stringBuilder.AppendFormat(" {0}={1},", "SignedHeaders", SignedHeaders);
				stringBuilder.AppendFormat(" {0}={1}", "Signature", Signature);
				return stringBuilder.ToString();
			}
		}

		public string ForQueryParameters
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("{0}={1}", "X-Amz-Algorithm", "AWS4-HMAC-SHA256");
				stringBuilder.AppendFormat("&{0}={1}", "X-Amz-Credential", string.Format(CultureInfo.InvariantCulture, "{0}/{1}", AccessKeyId, Scope));
				stringBuilder.AppendFormat("&{0}={1}", "X-Amz-Date", ISO8601DateTime);
				stringBuilder.AppendFormat("&{0}={1}", "X-Amz-SignedHeaders", SignedHeaders);
				stringBuilder.AppendFormat("&{0}={1}", "X-Amz-Signature", Signature);
				return stringBuilder.ToString();
			}
		}

		public AWS4SigningResult(string awsAccessKeyId, DateTime signedAt, string signedHeaders, string scope, byte[] signingKey, byte[] signature)
		{
			_awsAccessKeyId = awsAccessKeyId;
			_originalDateTime = signedAt;
			_signedHeaders = signedHeaders;
			_scope = scope;
			_signingKey = signingKey;
			_signature = signature;
		}
	}
}
