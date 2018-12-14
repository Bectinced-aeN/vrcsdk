using System;
using System.Collections.Generic;

namespace ThirdParty.iOS4Unity
{
	public class UIApplication : NSObject
	{
		private static readonly IntPtr _classHandle;

		private Dictionary<object, IntPtrHandler2> _failed;

		public override IntPtr ClassHandle => _classHandle;

		public static UIApplication SharedApplication => Runtime.GetNSObject<UIApplication>(ObjC.MessageSendIntPtr(_classHandle, Selector.GetHandle("sharedApplication")));

		public static string DidBecomeActiveNotification => ObjC.GetStringConstant(ObjC.Libraries.UIKit, "UIApplicationDidBecomeActiveNotification");

		public static string DidEnterBackgroundNotification => ObjC.GetStringConstant(ObjC.Libraries.UIKit, "UIApplicationDidEnterBackgroundNotification");

		public static string DidFinishLaunchingNotification => ObjC.GetStringConstant(ObjC.Libraries.UIKit, "UIApplicationDidFinishLaunchingNotification");

		public static string DidReceiveMemoryWarningNotification => ObjC.GetStringConstant(ObjC.Libraries.UIKit, "UIApplicationDidReceiveMemoryWarningNotification");

		public static string WillEnterForegroundNotification => ObjC.GetStringConstant(ObjC.Libraries.UIKit, "UIApplicationWillEnterForegroundNotification");

		public static string WillResignActiveNotification => ObjC.GetStringConstant(ObjC.Libraries.UIKit, "UIApplicationWillResignActiveNotification");

		public UIWindow KeyWindow => Runtime.GetNSObject<UIWindow>(ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("keyWindow")));

		public UIWindow[] Windows => ObjC.FromNSArray<UIWindow>(ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("windows")));

		public int ApplicationIconBadgeNumber
		{
			get
			{
				return ObjC.MessageSendInt(Handle, Selector.GetHandle("applicationIconBadgeNumber"));
			}
			set
			{
				ObjC.MessageSend(Handle, Selector.GetHandle("setApplicationIconBadgeNumber:"), value);
			}
		}

		public UIApplicationState ApplicationState => (UIApplicationState)ObjC.MessageSendInt(Handle, Selector.GetHandle("applicationState"));

		public UIStatusBarStyle StatusBarStyle
		{
			get
			{
				return (UIStatusBarStyle)ObjC.MessageSendInt(Handle, Selector.GetHandle("statusBarStyle"));
			}
			set
			{
				ObjC.MessageSend(Handle, Selector.GetHandle("setStatusBarStyle:"), (int)value);
			}
		}

		public bool StatusBarHidden
		{
			get
			{
				return ObjC.MessageSendBool(Handle, Selector.GetHandle("isStatusBarHidden"));
			}
			set
			{
				ObjC.MessageSend(Handle, Selector.GetHandle("setStatusBarHidden:"), value);
			}
		}

		public bool NetworkActivityIndicatorVisible
		{
			get
			{
				return ObjC.MessageSendBool(Handle, Selector.GetHandle("isNetworkActivityIndicatorVisible"));
			}
			set
			{
				ObjC.MessageSend(Handle, Selector.GetHandle("setNetworkActivityIndicatorVisible:"), value);
			}
		}

		public bool IdleTimerDisabled
		{
			get
			{
				return ObjC.MessageSendBool(Handle, Selector.GetHandle("isIdleTimerDisabled"));
			}
			set
			{
				ObjC.MessageSend(Handle, Selector.GetHandle("setIdleTimerDisabled:"), value);
			}
		}

		static UIApplication()
		{
			_classHandle = ObjC.GetClass("UIApplication");
		}

		internal UIApplication(IntPtr handle)
			: base(handle)
		{
		}

		public void SetStatusBarHidden(bool hidden, bool animated = true)
		{
			ObjC.MessageSend(Handle, Selector.GetHandle("setStatusBarHidden:animated:"), hidden, animated);
		}

		public bool CanOpenUrl(string url)
		{
			return ObjC.MessageSendBool_NSUrl(Handle, Selector.GetHandle("canOpenURL:"), url);
		}

		public bool OpenUrl(string url)
		{
			return ObjC.MessageSendBool_NSUrl(Handle, Selector.GetHandle("openURL:"), url);
		}

		public void RegisterForRemoteNotificationTypes(UIRemoteNotificationType types)
		{
			ObjC.MessageSend(Handle, Selector.GetHandle("registerForRemoteNotificationTypes:"), (int)types);
		}

		public void UnregisterForRemoteNotifications()
		{
			ObjC.MessageSend(Handle, Selector.GetHandle("unregisterForRemoteNotifications"));
		}

		public void RegisterUserNotificationSettings(UIUserNotificationSettings notificationSettings)
		{
			ObjC.MessageSend(Handle, Selector.GetHandle("registerUserNotificationSettings:"), notificationSettings.Handle);
		}

		public void PresentLocationNotificationNow(UILocalNotification notification)
		{
			ObjC.MessageSend(Handle, Selector.GetHandle("presentLocalNotificationNow:"), notification.Handle);
		}

		public void ScheduleLocalNotification(UILocalNotification notification)
		{
			ObjC.MessageSend(Handle, Selector.GetHandle("scheduleLocalNotification:"), notification.Handle);
		}

		public void CancelAllLocalNotifications()
		{
			ObjC.MessageSend(Handle, Selector.GetHandle("cancelAllLocalNotifications"));
		}

		public void CancelLocalNotification(UILocalNotification notification)
		{
			ObjC.MessageSend(Handle, Selector.GetHandle("cancelLocalNotification:"), notification.Handle);
		}
	}
}
