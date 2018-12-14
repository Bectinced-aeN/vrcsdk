using System;
using System.Globalization;

namespace ThirdParty.iOS4Unity
{
	public sealed class UIDevice : NSObject
	{
		private static readonly IntPtr _classHandle;

		private static int _majorVersion;

		private static int _minorVersion;

		public override IntPtr ClassHandle => _classHandle;

		public static string BatteryLevelDidChangeNotification => ObjC.GetStringConstant(ObjC.Libraries.UIKit, "UIDeviceBatteryLevelDidChangeNotification");

		public static string BatteryStateDidChangeNotification => ObjC.GetStringConstant(ObjC.Libraries.UIKit, "UIDeviceBatteryStateDidChangeNotification");

		public static string OrientationDidChangeNotification => ObjC.GetStringConstant(ObjC.Libraries.UIKit, "UIDeviceOrientationDidChangeNotification");

		public static string ProximityStateDidChangeNotification => ObjC.GetStringConstant(ObjC.Libraries.UIKit, "UIDeviceProximityStateDidChangeNotification");

		public static UIDevice CurrentDevice => Runtime.GetNSObject<UIDevice>(ObjC.MessageSendIntPtr(_classHandle, Selector.GetHandle("currentDevice")));

		public float BatteryLevel => ObjC.MessageSendFloat(Handle, Selector.GetHandle("batteryLevel"));

		public bool BatteryMonitoringEnabled
		{
			get
			{
				return ObjC.MessageSendBool(Handle, Selector.GetHandle("isBatteryMonitoringEnabled"));
			}
			set
			{
				ObjC.MessageSend(Handle, Selector.GetHandle("setBatteryMonitoringEnabled:"), value);
			}
		}

		public UIDeviceBatteryState BatteryState => (UIDeviceBatteryState)ObjC.MessageSendInt(Handle, Selector.GetHandle("batteryState"));

		public bool GeneratesDeviceOrientationNotifications => ObjC.MessageSendBool(Handle, Selector.GetHandle("isGeneratingDeviceOrientationNotifications"));

		public string LocalizedModel => ObjC.MessageSendString(Handle, Selector.GetHandle("localizedModel"));

		public string Model => ObjC.FromNSString(ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("model")));

		public string Name => ObjC.FromNSString(ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("name")));

		public UIDeviceOrientation Orientation => (UIDeviceOrientation)ObjC.MessageSendInt(Handle, Selector.GetHandle("orientation"));

		public bool ProximityMonitoringEnabled
		{
			get
			{
				return ObjC.MessageSendBool(Handle, Selector.GetHandle("isProximityMonitoringEnabled"));
			}
			set
			{
				ObjC.MessageSend(Handle, Selector.GetHandle("setProximityMonitoringEnabled:"), value);
			}
		}

		public bool ProximityState => ObjC.MessageSendBool(Handle, Selector.GetHandle("proximityState"));

		public string SystemName => ObjC.FromNSString(ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("systemName")));

		public string SystemVersion => ObjC.FromNSString(ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("systemVersion")));

		public UIUserInterfaceIdiom UserInterfaceIdiom => (UIUserInterfaceIdiom)ObjC.MessageSendInt(Handle, Selector.GetHandle("userInterfaceIdiom"));

		static UIDevice()
		{
			_majorVersion = -1;
			_minorVersion = -1;
			_classHandle = ObjC.GetClass("UIDevice");
		}

		internal UIDevice(IntPtr handle)
			: base(handle)
		{
		}

		public bool CheckSystemVersion(int major, int minor)
		{
			if (_majorVersion == -1)
			{
				string[] array = SystemVersion.Split('.');
				if (array.Length < 1 || !int.TryParse(array[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out _majorVersion))
				{
					_majorVersion = 2;
				}
				if (array.Length < 2 || !int.TryParse(array[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out _minorVersion))
				{
					_minorVersion = 0;
				}
			}
			if (_majorVersion <= major)
			{
				if (_majorVersion == major)
				{
					return _minorVersion >= minor;
				}
				return false;
			}
			return true;
		}

		public void PlayInputClick()
		{
			ObjC.MessageSend(Handle, Selector.GetHandle("playInputClick"));
		}
	}
}
