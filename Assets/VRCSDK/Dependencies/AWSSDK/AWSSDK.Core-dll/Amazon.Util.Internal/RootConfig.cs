using System.Collections.Generic;
using System.Xml.Linq;

namespace Amazon.Util.Internal
{
	public class RootConfig
	{
		private const string _rootAwsSectionName = "aws";

		public LoggingConfig Logging
		{
			get;
			private set;
		}

		public ProxyConfig Proxy
		{
			get;
			private set;
		}

		public string EndpointDefinition
		{
			get;
			set;
		}

		public string Region
		{
			get;
			set;
		}

		public string ProfileName
		{
			get;
			set;
		}

		public string ProfilesLocation
		{
			get;
			set;
		}

		public RegionEndpoint RegionEndpoint
		{
			get
			{
				if (string.IsNullOrEmpty(Region))
				{
					return null;
				}
				return RegionEndpoint.GetBySystemName(Region);
			}
			set
			{
				if (value == null)
				{
					Region = null;
				}
				else
				{
					Region = value.SystemName;
				}
			}
		}

		public bool UseSdkCache
		{
			get;
			set;
		}

		public bool CorrectForClockSkew
		{
			get;
			set;
		}

		public string ApplicationName
		{
			get;
			set;
		}

		private IDictionary<string, XElement> ServiceSections
		{
			get;
			set;
		}

		public RootConfig()
		{
			Logging = new LoggingConfig();
			Proxy = new ProxyConfig();
			EndpointDefinition = AWSConfigs._endpointDefinition;
			Region = AWSConfigs._awsRegion;
			ProfileName = AWSConfigs._awsProfileName;
			ProfilesLocation = AWSConfigs._awsAccountsLocation;
			UseSdkCache = AWSConfigs._useSdkCache;
			CorrectForClockSkew = true;
			AWSSection section = AWSConfigs.GetSection<AWSSection>("aws");
			Logging.Configure(section.Logging);
			Proxy.Configure(section.Proxy);
			ServiceSections = section.ServiceSections;
			if (section.UseSdkCache.HasValue)
			{
				UseSdkCache = section.UseSdkCache.Value;
			}
			EndpointDefinition = Choose(EndpointDefinition, section.EndpointDefinition);
			Region = Choose(Region, section.Region);
			ProfileName = Choose(ProfileName, section.ProfileName);
			ProfilesLocation = Choose(ProfilesLocation, section.ProfilesLocation);
			ApplicationName = Choose(ApplicationName, section.ApplicationName);
			if (section.CorrectForClockSkew.HasValue)
			{
				CorrectForClockSkew = section.CorrectForClockSkew.Value;
			}
		}

		private static string Choose(string a, string b)
		{
			if (!string.IsNullOrEmpty(a))
			{
				return a;
			}
			return b;
		}

		public XElement GetServiceSection(string service)
		{
			if (ServiceSections.TryGetValue(service, out XElement value))
			{
				return value;
			}
			return null;
		}
	}
}
