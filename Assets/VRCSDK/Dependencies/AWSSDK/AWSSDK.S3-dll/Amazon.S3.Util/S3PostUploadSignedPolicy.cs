using Amazon.Runtime;
using Amazon.Runtime.Internal.Auth;
using Amazon.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using ThirdParty.Json.LitJson;

namespace Amazon.S3.Util
{
	[XmlRoot(IsNullable = false)]
	public class S3PostUploadSignedPolicy
	{
		private static string KEY_POLICY = "policy";

		private static string KEY_SIGNATURE = "signature";

		private static string KEY_ACCESSKEY = "access_key";

		public string Policy
		{
			get;
			set;
		}

		public string Signature
		{
			get;
			set;
		}

		public string AccessKeyId
		{
			get;
			set;
		}

		public string SecurityToken
		{
			get;
			set;
		}

		public string SignatureVersion
		{
			get;
			set;
		}

		public string Algorithm
		{
			get;
			set;
		}

		public string Date
		{
			get;
			set;
		}

		public string Credential
		{
			get;
			set;
		}

		public static S3PostUploadSignedPolicy GetSignedPolicy(string policy, AWSCredentials credentials)
		{
			ImmutableCredentials credentials2 = credentials.GetCredentials();
			string text = Convert.ToBase64String(credentials2.get_UseToken() ? addTokenToPolicy(policy, credentials2.get_Token()) : Encoding.UTF8.GetBytes(policy.Trim()));
			string signature = CryptoUtilFactory.get_CryptoInstance().HMACSign(Encoding.UTF8.GetBytes(text), credentials2.get_SecretKey(), 0);
			return new S3PostUploadSignedPolicy
			{
				Policy = text,
				Signature = signature,
				AccessKeyId = credentials2.get_AccessKey(),
				SecurityToken = credentials2.get_Token(),
				SignatureVersion = "2"
			};
		}

		public static S3PostUploadSignedPolicy GetSignedPolicyV4(string policy, AWSCredentials credentials, RegionEndpoint region)
		{
			DateTime correctedUtcNow = AWSSDKUtils.get_CorrectedUtcNow();
			ImmutableCredentials credentials2 = credentials.GetCredentials();
			string text = "AWS4-HMAC-SHA256";
			string text2 = AWS4Signer.FormatDateTime(correctedUtcNow, "yyyyMMdd");
			string text3 = AWS4Signer.FormatDateTime(correctedUtcNow, "yyyyMMddTHHmmssZ");
			string text4 = string.Format(CultureInfo.InvariantCulture, "{0}/{1}/{2}/{3}/{4}/", credentials2.get_AccessKey(), text2, region.get_SystemName(), "s3", "aws4_request");
			Dictionary<string, string> dictionary = new Dictionary<string, string>
			{
				{
					S3Constants.PostFormDataXAmzCredential,
					text4
				},
				{
					S3Constants.PostFormDataXAmzAlgorithm,
					text
				},
				{
					S3Constants.PostFormDataXAmzDate,
					text3
				}
			};
			if (credentials2.get_UseToken())
			{
				dictionary[S3Constants.PostFormDataSecurityToken] = credentials2.get_Token();
			}
			string text5 = Convert.ToBase64String(addConditionsToPolicy(policy, dictionary));
			byte[] array = AWS4Signer.ComposeSigningKey(credentials2.get_SecretKey(), region.get_SystemName(), text2, "s3");
			string signature = AWSSDKUtils.ToHex(AWS4Signer.ComputeKeyedHash(1, array, text5), true);
			return new S3PostUploadSignedPolicy
			{
				Policy = text5,
				Signature = signature,
				AccessKeyId = credentials2.get_AccessKey(),
				SecurityToken = credentials2.get_Token(),
				SignatureVersion = "4",
				Algorithm = text,
				Date = text3,
				Credential = text4
			};
		}

		private static byte[] addConditionsToPolicy(string policy, Dictionary<string, string> newConditions)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Expected O, but got Unknown
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Expected O, but got Unknown
			JsonData val = JsonMapper.ToObject(new JsonReader(policy));
			JsonData val2 = val.get_Item("conditions");
			if (val2 != null && val2.get_IsArray())
			{
				foreach (KeyValuePair<string, string> newCondition in newConditions)
				{
					bool flag = false;
					for (int i = 0; i < val2.get_Count(); i++)
					{
						JsonData val3 = val2.get_Item(i);
						if (val3.get_IsObject() && val3.get_Item(newCondition.Key) != null)
						{
							val3.set_Item(newCondition.Key, JsonData.op_Implicit(newCondition.Value));
							flag = true;
						}
					}
					if (!flag)
					{
						JsonData val4 = new JsonData();
						val4.SetJsonType(1);
						val4.set_Item(newCondition.Key, JsonData.op_Implicit(newCondition.Value));
						val2.Add((object)val4);
					}
				}
			}
			return Encoding.UTF8.GetBytes(JsonMapper.ToJson((object)val).Trim());
		}

		private static byte[] addTokenToPolicy(string policy, string token)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Expected O, but got Unknown
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Expected O, but got Unknown
			JsonData val = JsonMapper.ToObject(new JsonReader(policy));
			bool flag = false;
			JsonData val2 = val.get_Item("conditions");
			if (val2 != null && val2.get_IsArray())
			{
				for (int i = 0; i < val2.get_Count(); i++)
				{
					JsonData val3 = val2.get_Item(i);
					if (val3.get_IsObject() && val3.get_Item(S3Constants.PostFormDataSecurityToken) != null)
					{
						val3.set_Item(S3Constants.PostFormDataSecurityToken, JsonData.op_Implicit(token));
						flag = true;
					}
				}
				if (!flag)
				{
					JsonData val4 = new JsonData();
					val4.SetJsonType(1);
					val4.set_Item(S3Constants.PostFormDataSecurityToken, JsonData.op_Implicit(token));
					val2.Add((object)val4);
				}
			}
			return Encoding.UTF8.GetBytes(JsonMapper.ToJson((object)val).Trim());
		}

		public string GetReadablePolicy()
		{
			return Encoding.UTF8.GetString(Convert.FromBase64String(Policy));
		}

		public string ToJson()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Expected O, but got Unknown
			JsonData val = new JsonData();
			val.set_Item(KEY_POLICY, JsonData.op_Implicit(Policy));
			val.set_Item(KEY_SIGNATURE, JsonData.op_Implicit(Signature));
			val.set_Item(KEY_ACCESSKEY, JsonData.op_Implicit(AccessKeyId));
			return JsonMapper.ToJson((object)val);
		}

		public string ToXml()
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			XmlSerializer xmlSerializer = new XmlSerializer(GetType());
			using (StringWriter textWriter = new StringWriter(stringBuilder, CultureInfo.InvariantCulture))
			{
				xmlSerializer.Serialize(textWriter, this);
			}
			return stringBuilder.ToString();
		}

		public static S3PostUploadSignedPolicy GetSignedPolicyFromJson(string policyJson)
		{
			JsonData val;
			try
			{
				val = JsonMapper.ToObject(policyJson);
			}
			catch (Exception innerException)
			{
				throw new ArgumentException("Invalid JSON document", innerException);
			}
			if (val.get_Item(KEY_POLICY) == null || !val.get_Item(KEY_POLICY).get_IsString())
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "JSON document requires '{0}' field"), KEY_POLICY);
			}
			if (val.get_Item(KEY_SIGNATURE) == null || !val.get_Item(KEY_SIGNATURE).get_IsString())
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "JSON document requires '{0}' field"), KEY_SIGNATURE);
			}
			if (val.get_Item(KEY_ACCESSKEY) == null || !val.get_Item(KEY_ACCESSKEY).get_IsString())
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "JSON document requires '{0}' field"), KEY_ACCESSKEY);
			}
			return new S3PostUploadSignedPolicy
			{
				Policy = ((object)val.get_Item(KEY_POLICY)).ToString(),
				Signature = ((object)val.get_Item(KEY_SIGNATURE)).ToString(),
				AccessKeyId = ((object)val.get_Item(KEY_ACCESSKEY)).ToString()
			};
		}

		public static S3PostUploadSignedPolicy GetSignedPolicyFromXml(string policyXml)
		{
			StringReader textReader = new StringReader(policyXml);
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(S3PostUploadSignedPolicy));
			S3PostUploadSignedPolicy s3PostUploadSignedPolicy;
			try
			{
				s3PostUploadSignedPolicy = (xmlSerializer.Deserialize(textReader) as S3PostUploadSignedPolicy);
			}
			catch (Exception innerException)
			{
				throw new ArgumentException("Could not parse XML", innerException);
			}
			if (string.IsNullOrEmpty(s3PostUploadSignedPolicy.AccessKeyId))
			{
				throw new ArgumentException("XML Document requries 'AccessKeyId' field");
			}
			if (string.IsNullOrEmpty(s3PostUploadSignedPolicy.Policy))
			{
				throw new ArgumentException("XML Document requries 'Policy' field");
			}
			if (string.IsNullOrEmpty(s3PostUploadSignedPolicy.Signature))
			{
				throw new ArgumentException("XML Document requries 'Signature' field");
			}
			return s3PostUploadSignedPolicy;
		}
	}
}
