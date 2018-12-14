using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using ThirdParty.Json.LitJson;

namespace Amazon.Internal
{
	public class RegionEndpointV3 : IRegionEndpoint
	{
		private class ServiceMap
		{
			private Dictionary<string, RegionEndpoint.Endpoint> _serviceMap = new Dictionary<string, RegionEndpoint.Endpoint>();

			private Dictionary<string, RegionEndpoint.Endpoint> _dualServiceMap = new Dictionary<string, RegionEndpoint.Endpoint>();

			private Dictionary<string, RegionEndpoint.Endpoint> GetMap(bool dualStack)
			{
				if (!dualStack)
				{
					return _serviceMap;
				}
				return _dualServiceMap;
			}

			public bool ContainsKey(string servicName)
			{
				return _serviceMap.ContainsKey(servicName);
			}

			public void Add(string serviceName, bool dualStack, RegionEndpoint.Endpoint endpoint)
			{
				if (!dualStack)
				{
					Dictionary<string, RegionEndpoint.Endpoint> serviceMap = _serviceMap;
				}
				else
				{
					Dictionary<string, RegionEndpoint.Endpoint> dualServiceMap = _dualServiceMap;
				}
				GetMap(dualStack).Add(serviceName, endpoint);
			}

			public bool TryGetEndpoint(string serviceName, bool dualStack, out RegionEndpoint.Endpoint endpoint)
			{
				return GetMap(dualStack).TryGetValue(serviceName, out endpoint);
			}
		}

		private ServiceMap _serviceMap = new ServiceMap();

		private JsonData _partitionJsonData;

		private JsonData _servicesJsonData;

		private bool _servicesLoaded;

		public string RegionName
		{
			get;
			private set;
		}

		public string DisplayName
		{
			get;
			private set;
		}

		public string PartitionName => (string)_partitionJsonData["partition"];

		public RegionEndpointV3(string regionName, string displayName, JsonData partition, JsonData services)
		{
			RegionName = regionName;
			DisplayName = displayName;
			_partitionJsonData = partition;
			_servicesJsonData = services;
		}

		public RegionEndpoint.Endpoint GetEndpointForService(string serviceName, bool dualStack)
		{
			RegionEndpoint.Endpoint endpoint = null;
			lock (_serviceMap)
			{
				if (!_servicesLoaded)
				{
					ParseAllServices();
					_servicesLoaded = true;
				}
				if (!_serviceMap.TryGetEndpoint(serviceName, dualStack, out endpoint))
				{
					return CreateUnknownEndpoint(serviceName, dualStack);
				}
				return endpoint;
			}
		}

		private RegionEndpoint.Endpoint CreateUnknownEndpoint(string serviceName, bool dualStack)
		{
			string text = (string)_partitionJsonData["defaults"]["hostname"];
			if (dualStack)
			{
				text = text.Replace("{region}", "dualstack.{region}");
			}
			return new RegionEndpoint.Endpoint(text.Replace("{service}", serviceName).Replace("{region}", RegionName).Replace("{dnsSuffix}", (string)_partitionJsonData["dnsSuffix"]), null, null);
		}

		private void ParseAllServices()
		{
			foreach (string propertyName in _servicesJsonData.PropertyNames)
			{
				AddServiceToMap(_servicesJsonData[propertyName], propertyName);
			}
		}

		private void AddServiceToMap(JsonData service, string serviceName)
		{
			string text = (service["partitionEndpoint"] != null) ? ((string)service["partitionEndpoint"]) : "";
			bool num = service["isRegionalized"] == null || (bool)service["isRegionalized"];
			string prop_name = RegionName;
			if (!num && !string.IsNullOrEmpty(text))
			{
				prop_name = text;
			}
			JsonData jsonData = service["endpoints"][prop_name];
			if (jsonData != null)
			{
				JsonData jsonData2 = new JsonData();
				MergeJsonData(jsonData2, jsonData);
				MergeJsonData(jsonData2, service["defaults"]);
				MergeJsonData(jsonData2, _partitionJsonData["defaults"]);
				CreateEndpointAndAddToServiceMap(jsonData2, RegionName, serviceName);
			}
		}

		private static void MergeJsonData(JsonData target, JsonData source)
		{
			if (source != null && target != null)
			{
				foreach (string propertyName in source.PropertyNames)
				{
					if (target[propertyName] == null)
					{
						target[propertyName] = source[propertyName];
					}
				}
			}
		}

		private void CreateEndpointAndAddToServiceMap(JsonData result, string regionName, string serviceName)
		{
			CreateEndpointAndAddToServiceMap(result, regionName, serviceName, dualStack: false);
			CreateEndpointAndAddToServiceMap(result, regionName, serviceName, dualStack: true);
		}

		private void CreateEndpointAndAddToServiceMap(JsonData result, string regionName, string serviceName, bool dualStack)
		{
			string text = ((string)result["hostname"]).Replace("{service}", serviceName).Replace("{region}", regionName).Replace("{dnsSuffix}", (string)_partitionJsonData["dnsSuffix"]);
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
					text = string.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}", serviceName, "dualstack." + regionName, (string)_partitionJsonData["dnsSuffix"]);
				}
			}
			string authregion = null;
			string text2 = null;
			JsonData jsonData = result["credentialScope"];
			if (jsonData != null)
			{
				authregion = DetermineAuthRegion(jsonData);
				if (jsonData["service"] != null && string.Compare((string)jsonData["service"], serviceName, StringComparison.OrdinalIgnoreCase) != 0)
				{
					text2 = (string)jsonData["service"];
				}
			}
			string signatureVersionOverride = DetermineSignatureOverride(result, serviceName);
			RegionEndpoint.Endpoint endpoint = new RegionEndpoint.Endpoint(text, authregion, signatureVersionOverride);
			_serviceMap.Add(serviceName, dualStack, endpoint);
			if (!string.IsNullOrEmpty(text2) && !_serviceMap.ContainsKey(text2))
			{
				_serviceMap.Add(text2, dualStack, endpoint);
			}
		}

		private static string DetermineSignatureOverride(JsonData defaults, string serviceName)
		{
			if (string.Equals(serviceName, "s3", StringComparison.OrdinalIgnoreCase))
			{
				bool flag = false;
				foreach (JsonData item in (IEnumerable)defaults["signatureVersions"])
				{
					if (string.Equals((string)item, "s3", StringComparison.OrdinalIgnoreCase))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return "4";
				}
				return "2";
			}
			return null;
		}

		private static string DetermineAuthRegion(JsonData credentialScope)
		{
			string result = null;
			if (credentialScope["region"] != null)
			{
				result = (string)credentialScope["region"];
			}
			return result;
		}
	}
}
