using System;
using System.Globalization;

namespace Amazon.Util.Internal.PlatformServices
{
	public class EnvironmentInfo : IEnvironmentInfo
	{
		public string Platform
		{
			get;
			private set;
		}

		public string Model
		{
			get;
			private set;
		}

		public string Make
		{
			get;
			private set;
		}

		public string PlatformVersion
		{
			get;
			private set;
		}

		public string Locale
		{
			get;
			private set;
		}

		public string FrameworkUserAgent
		{
			get;
			private set;
		}

		public string PclPlatform
		{
			get;
			private set;
		}

		public string PlatformUserAgent
		{
			get;
			private set;
		}

		public EnvironmentInfo()
		{
			Platform = AmazonHookedPlatformInfo.Instance.Platform;
			PlatformVersion = AmazonHookedPlatformInfo.Instance.PlatformVersion;
			Model = AmazonHookedPlatformInfo.Instance.Model;
			Make = AmazonHookedPlatformInfo.Instance.Make;
			Locale = AmazonHookedPlatformInfo.Instance.Locale;
			FrameworkUserAgent = string.Format(CultureInfo.InvariantCulture, ".NET_Runtime/{0}.{1} UnityVersion/{2}", Environment.Version.Major, Environment.Version.MajorRevision, AmazonHookedPlatformInfo.Instance.UnityVersion);
			PclPlatform = "Unity";
			PlatformUserAgent = $"unity_{Platform}_{PlatformVersion}";
		}
	}
}
