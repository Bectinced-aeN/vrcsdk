using Amazon.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Amazon
{
	public class RegionEndpoint
	{
		public class Endpoint
		{
			public string Hostname
			{
				get;
				private set;
			}

			public string AuthRegion
			{
				get;
				private set;
			}

			public string SignatureVersionOverride
			{
				get;
				private set;
			}

			internal Endpoint(string hostname, string authregion, string signatureVersionOverride)
			{
				Hostname = hostname;
				AuthRegion = authregion;
				SignatureVersionOverride = signatureVersionOverride;
			}

			public override string ToString()
			{
				return Hostname;
			}
		}

		private static Dictionary<string, RegionEndpoint> _hashBySystemName = new Dictionary<string, RegionEndpoint>(StringComparer.OrdinalIgnoreCase);

		public static readonly RegionEndpoint USEast1 = NewEndpoint("us-east-1", "US East (Virginia)");

		public static readonly RegionEndpoint USEast2 = NewEndpoint("us-east-2", "US East (Ohio)");

		public static readonly RegionEndpoint USWest1 = NewEndpoint("us-west-1", "US West (N. California)");

		public static readonly RegionEndpoint USWest2 = NewEndpoint("us-west-2", "US West (Oregon)");

		public static readonly RegionEndpoint EUWest1 = NewEndpoint("eu-west-1", "EU West (Ireland)");

		public static readonly RegionEndpoint EUCentral1 = NewEndpoint("eu-central-1", "EU Central (Frankfurt)");

		public static readonly RegionEndpoint APNortheast1 = NewEndpoint("ap-northeast-1", "Asia Pacific (Tokyo)");

		public static readonly RegionEndpoint APNortheast2 = NewEndpoint("ap-northeast-2", "Asia Pacific (Seoul)");

		public static readonly RegionEndpoint APSouth1 = NewEndpoint("ap-south-1", "Asia Pacific (Mumbai)");

		public static readonly RegionEndpoint APSoutheast1 = NewEndpoint("ap-southeast-1", "Asia Pacific (Singapore)");

		public static readonly RegionEndpoint APSoutheast2 = NewEndpoint("ap-southeast-2", "Asia Pacific (Sydney)");

		public static readonly RegionEndpoint SAEast1 = NewEndpoint("sa-east-1", "South America (Sao Paulo)");

		public static readonly RegionEndpoint USGovCloudWest1 = NewEndpoint("us-gov-west-1", "US GovCloud West (Oregon)");

		public static readonly RegionEndpoint CNNorth1 = NewEndpoint("cn-north-1", "China (Beijing)");

		private static IRegionEndpointProvider _regionEndpointProvider;

		public static IEnumerable<RegionEndpoint> EnumerableAllRegions
		{
			get
			{
				List<RegionEndpoint> list = new List<RegionEndpoint>();
				foreach (IRegionEndpoint allRegionEndpoint in RegionEndpointProvider.AllRegionEndpoints)
				{
					if (!_hashBySystemName.TryGetValue(allRegionEndpoint.RegionName, out RegionEndpoint value))
					{
						value = NewEndpoint(allRegionEndpoint.RegionName, allRegionEndpoint.DisplayName);
					}
					list.Add(value);
				}
				return list;
			}
		}

		private static IRegionEndpointProvider RegionEndpointProvider
		{
			get
			{
				if (_regionEndpointProvider == null)
				{
					if (!string.IsNullOrEmpty(AWSConfigs.EndpointDefinition))
					{
						_regionEndpointProvider = new RegionEndpointProviderV2();
					}
					else
					{
						_regionEndpointProvider = new RegionEndpointProviderV3();
					}
				}
				return _regionEndpointProvider;
			}
		}

		public string SystemName
		{
			get;
			private set;
		}

		public string DisplayName
		{
			get;
			private set;
		}

		private IRegionEndpoint InternedRegionEndpoint => RegionEndpointProvider.GetRegionEndpoint(SystemName);

		public static RegionEndpoint GetBySystemName(string systemName)
		{
			RegionEndpoint value = null;
			if (_hashBySystemName.TryGetValue(systemName, out value))
			{
				return value;
			}
			IRegionEndpoint regionEndpoint = RegionEndpointProvider.GetRegionEndpoint(systemName);
			return NewEndpoint(systemName, regionEndpoint.DisplayName);
		}

		private static RegionEndpoint NewEndpoint(string systemName, string displayName)
		{
			RegionEndpoint regionEndpoint = new RegionEndpoint(systemName, displayName);
			_hashBySystemName.Add(regionEndpoint.SystemName, regionEndpoint);
			return regionEndpoint;
		}

		private RegionEndpoint(string systemName, string displayName)
		{
			SystemName = systemName;
			DisplayName = displayName;
		}

		public Endpoint GetEndpointForService(string serviceName)
		{
			return GetEndpointForService(serviceName, dualStack: false);
		}

		public Endpoint GetEndpointForService(string serviceName, bool dualStack)
		{
			return InternedRegionEndpoint.GetEndpointForService(serviceName, dualStack);
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0} ({1})", DisplayName, SystemName);
		}
	}
}
