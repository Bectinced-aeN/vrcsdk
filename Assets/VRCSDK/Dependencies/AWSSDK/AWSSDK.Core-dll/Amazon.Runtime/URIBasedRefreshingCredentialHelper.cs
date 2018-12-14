using Amazon.Util;
using System;
using System.Globalization;
using System.Net;
using ThirdParty.Json.LitJson;

namespace Amazon.Runtime
{
	public class URIBasedRefreshingCredentialHelper : RefreshingAWSCredentials
	{
		protected class SecurityBase
		{
			public string Code
			{
				get;
				set;
			}

			public string Message
			{
				get;
				set;
			}

			public DateTime LastUpdated
			{
				get;
				set;
			}
		}

		protected class SecurityInfo : SecurityBase
		{
			public string InstanceProfileArn
			{
				get;
				set;
			}

			public string InstanceProfileId
			{
				get;
				set;
			}
		}

		protected class SecurityCredentials : SecurityBase
		{
			public string Type
			{
				get;
				set;
			}

			public string AccessKeyId
			{
				get;
				set;
			}

			public string SecretAccessKey
			{
				get;
				set;
			}

			public string Token
			{
				get;
				set;
			}

			public DateTime Expiration
			{
				get;
				set;
			}

			public string RoleArn
			{
				get;
				set;
			}
		}

		private static string SuccessCode = "Success";

		protected static string GetContents(Uri uri)
		{
			try
			{
				return AWSSDKUtils.DownloadStringContent(uri);
			}
			catch (WebException)
			{
				throw new AmazonServiceException("Unable to reach credentials server");
			}
		}

		protected static T GetObjectFromResponse<T>(Uri uri)
		{
			return JsonMapper.ToObject<T>(GetContents(uri));
		}

		protected static void ValidateResponse(SecurityBase response)
		{
			if (!string.Equals(response.Code, SuccessCode, StringComparison.OrdinalIgnoreCase))
			{
				throw new AmazonServiceException(string.Format(CultureInfo.InvariantCulture, "Unable to retrieve credentials. Code = \"{0}\". Message = \"{1}\".", response.Code, response.Message));
			}
		}
	}
}
