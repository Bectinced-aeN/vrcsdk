namespace Amazon.Util.Internal.PlatformServices
{
	public class ApplicationInfo : IApplicationInfo
	{
		public string AppTitle => AmazonHookedPlatformInfo.Instance.Title;

		public string AppVersionName => AmazonHookedPlatformInfo.Instance.VersionName;

		public string AppVersionCode => AmazonHookedPlatformInfo.Instance.VersionCode;

		public string PackageName => AmazonHookedPlatformInfo.Instance.PackageName;
	}
}
