using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Amazon.S3.Model
{
	public class Expiration
	{
		private DateTime expiryDate;

		private string ruleId;

		private static Regex expiryRegex = new Regex("expiry-date=\"(.+?)\"");

		private static Regex ruleRegex = new Regex("rule-id=\"(.+?)\"");

		public DateTime ExpiryDate
		{
			get
			{
				return expiryDate;
			}
			set
			{
				expiryDate = value;
			}
		}

		public string RuleId
		{
			get
			{
				return ruleId;
			}
			set
			{
				ruleId = value;
			}
		}

		public Expiration()
		{
			expiryDate = DateTime.MinValue;
			ruleId = string.Empty;
		}

		internal Expiration(string headerValue)
		{
			if (string.IsNullOrEmpty(headerValue))
			{
				throw new ArgumentNullException("headerValue");
			}
			Match match = expiryRegex.Match(headerValue);
			if (!match.Success || !match.Groups[1].Success)
			{
				throw new InvalidOperationException("No Expiry Date match");
			}
			DateTime dateTime = DateTime.ParseExact(match.Groups[1].Value, "ddd, dd MMM yyyy HH:mm:ss \\G\\M\\T", CultureInfo.InvariantCulture);
			Match match2 = ruleRegex.Match(headerValue);
			if (!match2.Success || !match2.Groups[1].Success)
			{
				throw new InvalidOperationException("No Rule Id match");
			}
			string text = UrlDecode(match2.Groups[1].Value);
			expiryDate = dateTime;
			ruleId = text;
		}

		private static string UrlDecode(string url)
		{
			return Uri.UnescapeDataString(url).Replace("+", " ");
		}
	}
}
