using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Amazon
{
	internal class AWSSection
	{
		private IDictionary<string, XElement> _serviceSections;

		public LoggingSection Logging
		{
			get;
			set;
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

		public bool? UseSdkCache
		{
			get;
			set;
		}

		public bool? CorrectForClockSkew
		{
			get;
			set;
		}

		public ProxySection Proxy
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

		public string ApplicationName
		{
			get;
			set;
		}

		public IDictionary<string, XElement> ServiceSections
		{
			get
			{
				if (_serviceSections == null)
				{
					_serviceSections = new Dictionary<string, XElement>(StringComparer.Ordinal);
				}
				return _serviceSections;
			}
			set
			{
				_serviceSections = value;
			}
		}
	}
}
