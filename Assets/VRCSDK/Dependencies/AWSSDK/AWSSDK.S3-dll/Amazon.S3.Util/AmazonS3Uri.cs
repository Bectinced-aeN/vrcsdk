using Amazon.Runtime;
using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Amazon.S3.Util
{
	public class AmazonS3Uri
	{
		private const string EndpointPattern = "^(.+\\.)?s3[.-]([a-z0-9-]+)\\.";

		public bool IsPathStyle
		{
			get;
			private set;
		}

		public string Bucket
		{
			get;
			private set;
		}

		public string Key
		{
			get;
			private set;
		}

		public RegionEndpoint Region
		{
			get;
			set;
		}

		public AmazonS3Uri(string uri)
			: this(new Uri(uri))
		{
		}

		public AmazonS3Uri(Uri uri)
		{
			if (uri == null)
			{
				throw new ArgumentNullException("uri");
			}
			if (string.IsNullOrEmpty(uri.Host))
			{
				throw new ArgumentException("Invalid URI - no hostname present");
			}
			Match match = new Regex("^(.+\\.)?s3[.-]([a-z0-9-]+)\\.").Match(uri.Host);
			if (!match.Success)
			{
				throw new ArgumentException("Invalid S3 URI - hostname does not appear to be a valid S3 endpoint");
			}
			Group group = match.Groups[1];
			if (string.IsNullOrEmpty(group.Value))
			{
				IsPathStyle = true;
				string absolutePath = uri.AbsolutePath;
				if (absolutePath.Equals("/"))
				{
					Bucket = null;
					Key = null;
				}
				else
				{
					int num = absolutePath.IndexOf('/', 1);
					if (num == -1)
					{
						Bucket = Decode(absolutePath.Substring(1));
						Key = null;
					}
					else if (num == absolutePath.Length - 1)
					{
						Bucket = Decode(absolutePath.Substring(1, num)).TrimEnd('/');
						Key = null;
					}
					else
					{
						Bucket = Decode(absolutePath.Substring(1, num)).TrimEnd('/');
						Key = Decode(absolutePath.Substring(num + 1));
					}
				}
			}
			else
			{
				IsPathStyle = false;
				Bucket = group.Value.TrimEnd('.');
				Key = (uri.AbsolutePath.Equals("/") ? null : uri.AbsolutePath.Substring(1));
			}
			if (match.Groups.Count > 2)
			{
				string value = match.Groups[2].Value;
				if (value.Equals("amazonaws", StringComparison.Ordinal) || value.Equals("external-1", StringComparison.Ordinal))
				{
					Region = RegionEndpoint.USEast1;
				}
				else
				{
					try
					{
						Region = RegionEndpoint.GetBySystemName(value);
					}
					catch (AmazonClientException)
					{
						Region = null;
					}
				}
			}
		}

		public static bool TryParseAmazonS3Uri(string uri, out AmazonS3Uri amazonS3Uri)
		{
			return TryParseAmazonS3Uri(new Uri(uri), out amazonS3Uri);
		}

		public static bool TryParseAmazonS3Uri(Uri uri, out AmazonS3Uri amazonS3Uri)
		{
			if (IsAmazonS3Endpoint(uri))
			{
				amazonS3Uri = new AmazonS3Uri(uri);
				return true;
			}
			amazonS3Uri = null;
			return false;
		}

		public static bool IsAmazonS3Endpoint(string uri)
		{
			if (uri == null)
			{
				throw new ArgumentNullException("uri");
			}
			return IsAmazonS3Endpoint(new Uri(uri));
		}

		public static bool IsAmazonS3Endpoint(Uri uri)
		{
			if (uri == null)
			{
				throw new ArgumentNullException("uri");
			}
			Match match = new Regex("^(.+\\.)?s3[.-]([a-z0-9-]+)\\.").Match(uri.Host);
			if (uri.Host.EndsWith("amazonaws.com", StringComparison.OrdinalIgnoreCase) || uri.Host.EndsWith("amazonaws.com.cn", StringComparison.OrdinalIgnoreCase))
			{
				return match.Success;
			}
			return false;
		}

		private static string Decode(string s)
		{
			if (s == null)
			{
				return null;
			}
			for (int i = 0; i < s.Length; i++)
			{
				if (s[i] == '%')
				{
					return Decode(s, i);
				}
			}
			return s;
		}

		private static string Decode(string s, int firstPercent)
		{
			StringBuilder stringBuilder = new StringBuilder(s.Substring(0, firstPercent));
			AppendDecoded(stringBuilder, s, firstPercent);
			for (int i = firstPercent + 3; i < s.Length; i++)
			{
				if (s[i] == '%')
				{
					AppendDecoded(stringBuilder, s, i);
					i += 2;
				}
				else
				{
					stringBuilder.Append(s[i]);
				}
			}
			return stringBuilder.ToString();
		}

		private static void AppendDecoded(StringBuilder builder, string s, int index)
		{
			if (index > s.Length - 3)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Invalid percent-encoded string '{0}'", s));
			}
			char c = s[index + 1];
			char c2 = s[index + 2];
			char value = (char)((FromHex(c) << 4) | FromHex(c2));
			builder.Append(value);
		}

		private static int FromHex(char c)
		{
			if (c < '0')
			{
				throw new InvalidOperationException("Invalid percent-encoded string: bad character '" + c.ToString() + "' in escape sequence.");
			}
			if (c <= '9')
			{
				return c - 48;
			}
			if (c < 'A')
			{
				throw new InvalidOperationException("Invalid percent-encoded string: bad character '" + c.ToString() + "' in escape sequence.");
			}
			if (c <= 'F')
			{
				return c - 65 + 10;
			}
			if (c < 'a')
			{
				throw new InvalidOperationException("Invalid percent-encoded string: bad character '" + c.ToString() + "' in escape sequence.");
			}
			if (c <= 'f')
			{
				return c - 97 + 10;
			}
			throw new InvalidOperationException("Invalid percent-encoded string: bad character '" + c.ToString() + "' in escape sequence.");
		}
	}
}
