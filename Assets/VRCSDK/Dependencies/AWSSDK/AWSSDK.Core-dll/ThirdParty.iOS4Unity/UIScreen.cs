using System;

namespace ThirdParty.iOS4Unity
{
	public class UIScreen : NSObject
	{
		private static readonly IntPtr _classHandle;

		public override IntPtr ClassHandle => _classHandle;

		public CGRect ApplicationFrame => ObjC.MessageSendCGRect(Handle, Selector.GetHandle("applicationFrame"));

		public UIScreenMode[] AvailableModes => ObjC.FromNSArray<UIScreenMode>(ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("availableModes")));

		public CGRect Bounds => ObjC.MessageSendCGRect(Handle, Selector.GetHandle("bounds"));

		public float Brightness
		{
			get
			{
				return ObjC.MessageSendFloat(Handle, Selector.GetHandle("brightness"));
			}
			set
			{
				ObjC.MessageSend(Handle, Selector.GetHandle("setBrightness"), value);
			}
		}

		public static string BrightnessDidChangeNotification => ObjC.GetStringConstant(ObjC.Libraries.UIKit, "UIScreenBrightnessDidChangeNotification");

		public UIScreenMode CurrentMode
		{
			get
			{
				return Runtime.GetNSObject<UIScreenMode>(ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("currentMode")));
			}
			set
			{
				ObjC.MessageSend(Handle, Selector.GetHandle("setCurrentMode"), value.Handle);
			}
		}

		public static string DidConnectNotification => ObjC.GetStringConstant(ObjC.Libraries.UIKit, "UIScreenDidConnectNotification");

		public static string DidDisconnectNotification => ObjC.GetStringConstant(ObjC.Libraries.UIKit, "UIScreenDidDisconnectNotification");

		public static UIScreen MainScreen => Runtime.GetNSObject<UIScreen>(ObjC.MessageSendIntPtr(_classHandle, Selector.GetHandle("mainScreen")));

		public UIScreen MirroredScreen => Runtime.GetNSObject<UIScreen>(ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("mirroredScreen")));

		public static string ModeDidChangeNotification => ObjC.GetStringConstant(ObjC.Libraries.UIKit, "UIScreenModeDidChangeNotification");

		public CGRect NativeBounds => ObjC.MessageSendCGRect(Handle, Selector.GetHandle("nativeBounds"));

		public float NativeScale => ObjC.MessageSendFloat(Handle, Selector.GetHandle("nativeScale"));

		public UIScreenMode PreferredMode => Runtime.GetNSObject<UIScreenMode>(ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("preferredMode")));

		public float Scale => ObjC.MessageSendFloat(Handle, Selector.GetHandle("scale"));

		public static UIScreen[] Screens => ObjC.FromNSArray<UIScreen>(ObjC.MessageSendIntPtr(_classHandle, Selector.GetHandle("screens")));

		public bool WantsSoftwareDimming
		{
			get
			{
				return ObjC.MessageSendBool(Handle, Selector.GetHandle("wantsSoftwareDimming"));
			}
			set
			{
				ObjC.MessageSend(Handle, Selector.GetHandle("setWantsSoftwareDimming"), value);
			}
		}

		static UIScreen()
		{
			_classHandle = ObjC.GetClass("UIScreen");
		}

		internal UIScreen(IntPtr handle)
			: base(handle)
		{
		}
	}
}
