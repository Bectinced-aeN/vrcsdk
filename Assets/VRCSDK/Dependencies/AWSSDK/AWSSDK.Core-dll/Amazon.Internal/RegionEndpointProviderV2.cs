using Amazon.Runtime.Internal.Util;
using Amazon.Util;
using Amazon.Util.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using ThirdParty.Json.LitJson;

namespace Amazon.Internal
{
	public class RegionEndpointProviderV2 : IRegionEndpointProvider
	{
		public class RegionEndpoint : IRegionEndpoint
		{
			private const string REGIONS_FILE = "Amazon.endpoints.json";

			private const string REGIONS_CUSTOMIZATIONS_FILE = "Amazon.endpoints.customizations.json";

			private const string DEFAULT_RULE = "*/*";

			private static Dictionary<string, JsonData> _documentEndpoints;

			private const int MAX_DOWNLOAD_RETRIES = 3;

			private static bool loaded = false;

			private static readonly object LOCK_OBJECT = new object();

			private static Dictionary<string, RegionEndpoint> hashBySystemName = new Dictionary<string, RegionEndpoint>(StringComparer.OrdinalIgnoreCase);

			public static IEnumerable<RegionEndpoint> EnumerableAllRegions
			{
				get
				{
					if (!loaded)
					{
						LoadEndpointDefinitions();
					}
					return hashBySystemName.Values;
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

			public string RegionName => SystemName;

			public Amazon.RegionEndpoint.Endpoint GetEndpointForService(string serviceName, bool dualStack)
			{
				if (!loaded)
				{
					LoadEndpointDefinitions();
				}
				JsonData endpointRule = GetEndpointRule(serviceName);
				string text = endpointRule["endpoint"].ToString();
				if (dualStack)
				{
					if (serviceName.Equals("s3", StringComparison.OrdinalIgnoreCase))
					{
						if (text.Equals("s3.amazonaws.com", StringComparison.OrdinalIgnoreCase))
						{
							text = "s3.dualstack.us-east-1.amazonaws.com";
						}
						else if (!text.StartsWith("s3-external-", StringComparison.OrdinalIgnoreCase))
						{
							if (text.StartsWith("s3-", StringComparison.OrdinalIgnoreCase))
							{
								text = "s3." + text.Substring(3);
							}
							if (text.StartsWith("s3.", StringComparison.OrdinalIgnoreCase))
							{
								text = text.Replace("s3.", "s3.dualstack.");
							}
						}
					}
					else
					{
						text = text.Replace("{region}", "dualstack.{region}");
					}
				}
				string text2 = text.Replace("{region}", SystemName).Replace("{service}", serviceName);
				string signatureVersionOverride = null;
				if (endpointRule["signature-version"] != null)
				{
					signatureVersionOverride = endpointRule["signature-version"].ToString();
				}
				string text3 = (endpointRule["auth-region"] == null) ? AWSSDKUtils.DetermineRegion(text2) : endpointRule["auth-region"].ToString();
				if (string.Equals(text3, SystemName, StringComparison.OrdinalIgnoreCase))
				{
					text3 = null;
				}
				return new Amazon.RegionEndpoint.Endpoint(text2, text3, signatureVersionOverride);
			}

			private JsonData GetEndpointRule(string serviceName)
			{
				JsonData value = null;
				if (_documentEndpoints.TryGetValue(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", SystemName, serviceName), out value))
				{
					return value;
				}
				if (_documentEndpoints.TryGetValue(string.Format(CultureInfo.InvariantCulture, "{0}/*", SystemName), out value))
				{
					return value;
				}
				if (_documentEndpoints.TryGetValue(string.Format(CultureInfo.InvariantCulture, "*/{0}", serviceName), out value))
				{
					return value;
				}
				return _documentEndpoints["*/*"];
			}

			private static RegionEndpoint NewEndpoint(string systemName, string displayName)
			{
				RegionEndpoint regionEndpoint = new RegionEndpoint(systemName, displayName);
				hashBySystemName.Add(regionEndpoint.SystemName, regionEndpoint);
				return regionEndpoint;
			}

			public static RegionEndpoint GetBySystemName(string systemName)
			{
				if (!loaded)
				{
					LoadEndpointDefinitions();
				}
				RegionEndpoint value = null;
				if (!hashBySystemName.TryGetValue(systemName, out value))
				{
					Logger.GetLogger(typeof(RegionEndpoint)).InfoFormat("Region system name {0} was not found in region data bundled with SDK; assuming new region.", systemName);
					if (systemName.StartsWith("cn-", StringComparison.Ordinal))
					{
						return NewEndpoint(systemName, "China (Unknown)");
					}
					return NewEndpoint(systemName, "Unknown");
				}
				return value;
			}

			private static void LoadEndpointDefinitions()
			{
				LoadEndpointDefinitions(AWSConfigs.EndpointDefinition);
			}

			public static void LoadEndpointDefinitions(string endpointsPath)
			{
				lock (LOCK_OBJECT)
				{
					if (!loaded)
					{
						_documentEndpoints = new Dictionary<string, JsonData>();
						if (string.IsNullOrEmpty(endpointsPath))
						{
							LoadEndpointDefinitionsFromEmbeddedResource();
						}
						loaded = true;
					}
				}
			}

			private static void ReadEndpointFile(Stream stream)
			{
				using (StreamReader reader = new StreamReader(stream))
				{
					JsonData jsonData = JsonMapper.ToObject(reader)["endpoints"];
					foreach (string propertyName in jsonData.PropertyNames)
					{
						_documentEndpoints[propertyName] = jsonData[propertyName];
					}
				}
			}

			private static void LoadEndpointDefinitionsFromEmbeddedResource()
			{
				using (Stream stream = TypeFactory.GetTypeInfo(typeof(RegionEndpoint)).Assembly.GetManifestResourceStream("Amazon.endpoints.json"))
				{
					ReadEndpointFile(stream);
				}
				using (Stream stream2 = TypeFactory.GetTypeInfo(typeof(RegionEndpoint)).Assembly.GetManifestResourceStream("Amazon.endpoints.customizations.json"))
				{
					ReadEndpointFile(stream2);
				}
			}

			public static void UnloadEndpointDefinitions()
			{
				lock (LOCK_OBJECT)
				{
					_documentEndpoints.Clear();
					loaded = false;
				}
			}

			private RegionEndpoint(string systemName, string displayName)
			{
				SystemName = systemName;
				DisplayName = displayName;
			}

			public override string ToString()
			{
				return string.Format(CultureInfo.InvariantCulture, "{0} ({1})", DisplayName, SystemName);
			}
		}

		public IEnumerable<IRegionEndpoint> AllRegionEndpoints => RegionEndpoint.EnumerableAllRegions as IEnumerable<IRegionEndpoint>;

		public IRegionEndpoint GetRegionEndpoint(string regionName)
		{
			return RegionEndpoint.GetBySystemName(regionName);
		}
	}
}
