namespace Amazon.Util.Internal.PlatformServices
{
	public interface IApplicationSettings
	{
		void SetValue(string key, string value, ApplicationSettingsMode mode);

		string GetValue(string key, ApplicationSettingsMode mode);

		void RemoveValue(string key, ApplicationSettingsMode mode);
	}
}
