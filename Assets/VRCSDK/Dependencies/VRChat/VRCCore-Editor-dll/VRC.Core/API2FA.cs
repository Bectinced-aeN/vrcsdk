using System.Collections.Generic;

namespace VRC.Core
{
	public class API2FA : ApiModel
	{
		public const string TIME_BASED_ONE_TIME_PASSWORD_AUTHENTICATION = "totp";

		public const string ONE_TIME_PASSWORD_AUTHENTICATION = "otp";

		public const string SMS_AUTHENTICATION = "sms";

		[ApiField(Required = true)]
		public List<string> requiresTwoFactorAuth
		{
			get;
			protected set;
		}

		public bool TimeBasedOneTimePasswordSupported()
		{
			return requiresTwoFactorAuth != null && requiresTwoFactorAuth.Contains("totp");
		}

		public bool OneTimePasswordSupported()
		{
			return requiresTwoFactorAuth != null && requiresTwoFactorAuth.Contains("otp");
		}

		public bool SmsSupported()
		{
			return requiresTwoFactorAuth != null && requiresTwoFactorAuth.Contains("sms");
		}

		public override string ToString()
		{
			return string.Format("requiresTwoFactorAuth: " + ((requiresTwoFactorAuth != null) ? string.Join(", ", requiresTwoFactorAuth.ToArray()) : "<null>"));
		}
	}
}
