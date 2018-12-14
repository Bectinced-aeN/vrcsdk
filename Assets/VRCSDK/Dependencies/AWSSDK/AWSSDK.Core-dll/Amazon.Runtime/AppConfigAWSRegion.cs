using Amazon.Runtime.Internal.Util;
using System;
using System.Globalization;

namespace Amazon.Runtime
{
	public class AppConfigAWSRegion : AWSRegion
	{
		public AppConfigAWSRegion()
		{
			RegionEndpoint regionEndpoint = AWSConfigs.RegionEndpoint;
			if (regionEndpoint != null)
			{
				base.Region = regionEndpoint;
				Logger.GetLogger(typeof(AppConfigAWSRegion)).InfoFormat("Region {0} found using {1} setting in application configuration file.", regionEndpoint.SystemName, "AWSRegion");
				return;
			}
			throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "The app.config/web.config files for the application did not contain region information"));
		}
	}
}
