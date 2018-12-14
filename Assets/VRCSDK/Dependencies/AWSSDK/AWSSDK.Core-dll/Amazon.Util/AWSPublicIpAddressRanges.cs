using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using ThirdParty.Json.LitJson;

namespace Amazon.Util
{
	public class AWSPublicIpAddressRanges
	{
		public const string AmazonServiceKey = "AMAZON";

		public const string EC2ServiceKey = "EC2";

		public const string CloudFrontServiceKey = "CLOUDFRONT";

		public const string Route53ServiceKey = "ROUTE53";

		public const string Route53HealthChecksServiceKey = "ROUTE53_HEALTHCHECKS";

		public const string GlobalRegionIdentifier = "GLOBAL";

		private const string createDateKey = "createDate";

		private const string ipv4PrefixesKey = "prefixes";

		private const string ipv4PrefixKey = "ip_prefix";

		private const string ipv6PrefixesKey = "ipv6_prefixes";

		private const string ipv6PrefixKey = "ipv6_prefix";

		private const string regionKey = "region";

		private const string serviceKey = "service";

		private const string createDateFormatString = "yyyy-MM-dd-HH-mm-ss";

		private static readonly Uri IpAddressRangeEndpoint = new Uri("https://ip-ranges.amazonaws.com/ip-ranges.json");

		public IEnumerable<string> ServiceKeys
		{
			get
			{
				HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
				foreach (AWSPublicIpAddressRange allAddressRange in AllAddressRanges)
				{
					hashSet.Add(allAddressRange.Service);
				}
				return hashSet;
			}
		}

		public DateTime CreateDate
		{
			get;
			private set;
		}

		public IEnumerable<AWSPublicIpAddressRange> AllAddressRanges
		{
			get;
			private set;
		}

		public IEnumerable<AWSPublicIpAddressRange> AddressRangesByServiceKey(string serviceKey)
		{
			if (!AllAddressRanges.Any())
			{
				throw new InvalidOperationException("No address range data has been loaded and parsed.");
			}
			return (from ar in AllAddressRanges
			where ar.Service.Equals(serviceKey, StringComparison.OrdinalIgnoreCase)
			select ar).ToList();
		}

		public IEnumerable<AWSPublicIpAddressRange> AddressRangesByRegion(string regionIdentifier)
		{
			if (!AllAddressRanges.Any())
			{
				throw new InvalidOperationException("No address range data has been loaded and parsed.");
			}
			return (from ar in AllAddressRanges
			where ar.Region.Equals(regionIdentifier, StringComparison.OrdinalIgnoreCase)
			select ar).ToList();
		}

		public static AWSPublicIpAddressRanges Load()
		{
			int num = 0;
			while (num < 3)
			{
				try
				{
					return Parse(AWSSDKUtils.DownloadStringContent(IpAddressRangeEndpoint));
				}
				catch (Exception innerException)
				{
					num++;
					if (num == 3)
					{
						throw new AmazonServiceException(string.Format(CultureInfo.InvariantCulture, "Error downloading public IP address ranges file from {0}.", IpAddressRangeEndpoint), innerException);
					}
				}
				AWSSDKUtils.Sleep(Math.Min((int)(Math.Pow(4.0, (double)num) * 100.0), 30000));
			}
			return null;
		}

		private static AWSPublicIpAddressRanges Parse(string fileContent)
		{
			try
			{
				AWSPublicIpAddressRanges aWSPublicIpAddressRanges = new AWSPublicIpAddressRanges();
				JsonData jsonData = JsonMapper.ToObject(new JsonReader(fileContent));
				DateTime? dateTime = null;
				try
				{
					string s = (string)jsonData["createDate"];
					dateTime = DateTime.ParseExact(s, "yyyy-MM-dd-HH-mm-ss", null);
				}
				catch (FormatException)
				{
				}
				catch (ArgumentNullException)
				{
				}
				aWSPublicIpAddressRanges.CreateDate = dateTime.GetValueOrDefault(DateTime.Now.ToUniversalTime());
				List<AWSPublicIpAddressRange> list = new List<AWSPublicIpAddressRange>();
				JsonData jsonData2 = jsonData["prefixes"];
				JsonData jsonData3 = jsonData["ipv6_prefixes"];
				if (!jsonData2.IsArray || !jsonData3.IsArray)
				{
					throw new InvalidDataException("Expected array content for ip_prefixes and/or ipv6_prefixes keys.");
				}
				list.AddRange(ParseRange(jsonData2, AWSPublicIpAddressRange.AddressFormat.Ipv4));
				list.AddRange(ParseRange(jsonData3, AWSPublicIpAddressRange.AddressFormat.Ipv6));
				aWSPublicIpAddressRanges.AllAddressRanges = list;
				return aWSPublicIpAddressRanges;
			}
			catch (Exception innerException)
			{
				throw new InvalidDataException("IP address ranges content in unexpected/invalid format.", innerException);
			}
		}

		private static IEnumerable<AWSPublicIpAddressRange> ParseRange(JsonData ranges, AWSPublicIpAddressRange.AddressFormat addressFormat)
		{
			string prefixKey = (addressFormat == AWSPublicIpAddressRange.AddressFormat.Ipv4) ? "ip_prefix" : "ipv6_prefix";
			List<AWSPublicIpAddressRange> list = new List<AWSPublicIpAddressRange>();
			list.AddRange(from JsonData range in ranges
			select new AWSPublicIpAddressRange
			{
				IpAddressFormat = addressFormat,
				IpPrefix = (string)range[prefixKey],
				Region = (string)range["region"],
				Service = (string)range["service"]
			});
			return list;
		}

		private AWSPublicIpAddressRanges()
		{
		}
	}
}
