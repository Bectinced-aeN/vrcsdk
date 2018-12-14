using System;

namespace ThirdParty.iOS4Unity
{
	public class UIViewController : NSObject
	{
		private static readonly IntPtr _classHandle;

		public override IntPtr ClassHandle => _classHandle;

		public string Title
		{
			get
			{
				return ObjC.MessageSendString(Handle, Selector.GetHandle("title"));
			}
			set
			{
				ObjC.MessageSend(Handle, Selector.GetHandle("setTitle:"), value);
			}
		}

		public UIView View
		{
			get
			{
				return Runtime.GetNSObject<UIView>(ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("view")));
			}
			set
			{
				ObjC.MessageSend(Handle, Selector.GetHandle("setView:"), value.Handle);
			}
		}

		public bool IsViewLoaded => ObjC.MessageSendBool(Handle, Selector.GetHandle("isViewLoaded"));

		public UIViewController ParentViewController => Runtime.GetNSObject<UIViewController>(ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("parentViewController")));

		public UIViewController PresentedViewController => Runtime.GetNSObject<UIViewController>(ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("presentedViewController")));

		public UIViewController PresentingViewController => Runtime.GetNSObject<UIViewController>(ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("presentingViewController")));

		static UIViewController()
		{
			_classHandle = ObjC.GetClass("UIViewController");
		}

		public UIViewController()
		{
		}

		internal UIViewController(IntPtr handle)
			: base(handle)
		{
		}

		public void LoadView()
		{
			ObjC.MessageSend(Handle, Selector.GetHandle("loadView"));
		}

		public void PresentViewController(UIViewController controller, bool animated = true)
		{
			ObjC.MessageSend(Handle, Selector.GetHandle("presentViewController:animated:completion:"), controller.Handle, animated, IntPtr.Zero);
		}

		public void DismissViewController(bool animated = true)
		{
			ObjC.MessageSend(Handle, Selector.GetHandle("dismissViewControllerAnimated:completion:"), animated, IntPtr.Zero);
		}
	}
}
