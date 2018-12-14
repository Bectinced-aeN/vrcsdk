using System;
using UnityEngine;

namespace AmplitudeSDKWrapper
{
	public class DeviceInfo
	{
		private string _osName;

		private string _deviceModel;

		private string _deviceName;

		private string _systemLanguage;

		private string _platform;

		public DeviceInfo()
		{
			CacheValues();
		}

		public string GetOsName()
		{
			return _osName;
		}

		public string GetOsVersion()
		{
			return string.Empty;
		}

		public string GetModel()
		{
			return _deviceModel;
		}

		public string GetDeviceName()
		{
			return _deviceName;
		}

		public string GetLanguage()
		{
			return _systemLanguage;
		}

		public string GetPlatform()
		{
			return _platform;
		}

		private void CacheValues()
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			_osName = SystemInfo.get_operatingSystem();
			_deviceModel = SystemInfo.get_deviceModel();
			_deviceName = SystemInfo.get_deviceName();
			_systemLanguage = ((Enum)Application.get_systemLanguage()).ToString();
			_platform = ((Enum)Application.get_platform()).ToString();
		}
	}
}
