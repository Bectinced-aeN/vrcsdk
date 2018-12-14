using System;

namespace ThirdParty.iOS4Unity
{
	public class UILocalNotification : NSObject
	{
		private static readonly IntPtr _classHandle;

		public override IntPtr ClassHandle => _classHandle;

		public string AlertAction
		{
			get
			{
				return ObjC.MessageSendString(Handle, Selector.GetHandle("alertAction"));
			}
			set
			{
				ObjC.MessageSend(Handle, Selector.GetHandle("setAlertAction:"), value);
			}
		}

		public string AlertBody
		{
			get
			{
				return ObjC.MessageSendString(Handle, Selector.GetHandle("alertBody"));
			}
			set
			{
				ObjC.MessageSend(Handle, Selector.GetHandle("setAlertBody:"), value);
			}
		}

		public string AlertLaunchImage
		{
			get
			{
				return ObjC.MessageSendString(Handle, Selector.GetHandle("alertLaunchImage"));
			}
			set
			{
				ObjC.MessageSend(Handle, Selector.GetHandle("setAlertLaunchImage:"), value);
			}
		}

		public string AlertTitle
		{
			get
			{
				return ObjC.MessageSendString(Handle, Selector.GetHandle("alertTitle"));
			}
			set
			{
				ObjC.MessageSend(Handle, Selector.GetHandle("setAlertTitle:"), value);
			}
		}

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

		public string Category
		{
			get
			{
				return ObjC.MessageSendString(Handle, Selector.GetHandle("category"));
			}
			set
			{
				ObjC.MessageSend(Handle, Selector.GetHandle("setCategory:"), value);
			}
		}

		public DateTime FireDate
		{
			get
			{
				return (DateTime)ObjC.MessageSendDate(Handle, Selector.GetHandle("fireDate"));
			}
			set
			{
				ObjC.MessageSend(Handle, Selector.GetHandle("setFireDate:"), value);
			}
		}

		public bool HasAction
		{
			get
			{
				return ObjC.MessageSendBool(Handle, Selector.GetHandle("hasAction"));
			}
			set
			{
				ObjC.MessageSend(Handle, Selector.GetHandle("setHasAction:"), value);
			}
		}

		public string SoundName
		{
			get
			{
				return ObjC.MessageSendString(Handle, Selector.GetHandle("soundName"));
			}
			set
			{
				ObjC.MessageSend(Handle, Selector.GetHandle("setSoundName:"), value);
			}
		}

		public NSDictionary UserInfo
		{
			get
			{
				return Runtime.GetNSObject<NSDictionary>(ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("userInfo")));
			}
			set
			{
				ObjC.MessageSend(Handle, Selector.GetHandle("setUserInfo:"), value?.Handle ?? IntPtr.Zero);
			}
		}

		static UILocalNotification()
		{
			_classHandle = ObjC.GetClass("UILocalNotification");
		}

		public UILocalNotification()
		{
			ObjC.MessageSendIntPtr(Handle, Selector.Init);
		}

		internal UILocalNotification(IntPtr handle)
			: base(handle)
		{
		}
	}
}
