using System;

namespace ThirdParty.iOS4Unity
{
	public class UIUserNotificationSettings : NSObject
	{
		private static readonly IntPtr _classHandle;

		public override IntPtr ClassHandle => _classHandle;

		public UIUserNotificationType Types => (UIUserNotificationType)ObjC.MessageSendUInt(Handle, Selector.GetHandle("types"));

		static UIUserNotificationSettings()
		{
			_classHandle = ObjC.GetClass("UIUserNotificationSettings");
		}

		public UIUserNotificationSettings()
		{
			ObjC.MessageSendIntPtr(Handle, Selector.Init);
		}

		internal UIUserNotificationSettings(IntPtr handle)
			: base(handle)
		{
		}

		public static UIUserNotificationSettings GetSettingsForTypes(UIUserNotificationType types)
		{
			return Runtime.GetNSObject<UIUserNotificationSettings>(ObjC.MessageSendIntPtr(_classHandle, Selector.GetHandle("settingsForTypes:categories:"), (uint)types, IntPtr.Zero));
		}
	}
}
