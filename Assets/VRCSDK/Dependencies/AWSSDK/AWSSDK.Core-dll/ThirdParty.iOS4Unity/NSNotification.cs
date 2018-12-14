using System;

namespace ThirdParty.iOS4Unity
{
	public class NSNotification : NSObject
	{
		private static readonly IntPtr _classHandle;

		public override IntPtr ClassHandle => _classHandle;

		public string Name => ObjC.MessageSendString(Handle, Selector.GetHandle("name"));

		public NSObject Object => Runtime.GetNSObject<NSObject>(ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("object")));

		static NSNotification()
		{
			_classHandle = ObjC.GetClass("NSNotification");
		}

		public static NSNotification FromName(string name, NSObject obj = null)
		{
			return Runtime.GetNSObject<NSNotification>(ObjC.MessageSendIntPtr(_classHandle, Selector.GetHandle("notificationWithName:object:"), name, obj?.Handle ?? IntPtr.Zero));
		}

		internal NSNotification(IntPtr handle)
			: base(handle)
		{
		}
	}
}
