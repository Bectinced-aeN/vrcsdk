using System;

namespace ThirdParty.iOS4Unity
{
	public class UIActivityViewController : UIViewController
	{
		private static readonly IntPtr _classHandle;

		public override IntPtr ClassHandle => _classHandle;

		static UIActivityViewController()
		{
			_classHandle = ObjC.GetClass("UIActivityViewController");
		}

		public UIActivityViewController(string text)
		{
			IntPtr intPtr = ObjC.ToNSString(text);
			IntPtr arg = ObjC.ToNSArray(new IntPtr[1]
			{
				intPtr
			});
			ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("initWithActivityItems:applicationActivities:"), arg, IntPtr.Zero);
			ObjC.MessageSend(intPtr, Selector.ReleaseHandle);
		}

		public UIActivityViewController(UIImage image)
		{
			IntPtr arg = ObjC.ToNSArray(new IntPtr[1]
			{
				image.Handle
			});
			ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("initWithActivityItems:applicationActivities:"), arg, IntPtr.Zero);
		}

		public UIActivityViewController(string text, UIImage image)
		{
			IntPtr intPtr = ObjC.ToNSString(text);
			IntPtr arg = ObjC.ToNSArray(new IntPtr[2]
			{
				intPtr,
				image.Handle
			});
			ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("initWithActivityItems:applicationActivities:"), arg, IntPtr.Zero);
			ObjC.MessageSend(intPtr, Selector.ReleaseHandle);
		}

		internal UIActivityViewController(IntPtr handle)
			: base(handle)
		{
		}
	}
}
