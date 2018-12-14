using System.Globalization;

namespace Amazon.S3.Model
{
	public class MfaCodes
	{
		public string SerialNumber
		{
			get;
			set;
		}

		public string AuthenticationValue
		{
			get;
			set;
		}

		public string FormattedMfaCodes => string.Format(CultureInfo.InvariantCulture, "{0} {1}", SerialNumber, AuthenticationValue);
	}
}
