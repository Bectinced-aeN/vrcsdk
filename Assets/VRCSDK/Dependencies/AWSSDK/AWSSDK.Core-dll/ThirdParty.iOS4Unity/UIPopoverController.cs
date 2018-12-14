using System;

namespace ThirdParty.iOS4Unity
{
	public class UIPopoverController : NSObject
	{
		private static readonly IntPtr _classHandle;

		public override IntPtr ClassHandle => _classHandle;

		public UIViewController ContentViewController
		{
			get
			{
				return Runtime.GetNSObject<UIViewController>(ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("contentViewController")));
			}
			set
			{
				ObjC.MessageSend(Handle, Selector.GetHandle("setContentViewController:"), value.Handle);
			}
		}

		public UIPopoverArrowDirection PopoverArrowDirection => (UIPopoverArrowDirection)ObjC.MessageSendUInt(Handle, Selector.GetHandle("popoverArrowDirection"));

		public CGSize PopoverContentSize
		{
			get
			{
				return ObjC.MessageSendCGSize(Handle, "popoverContentSize");
			}
			set
			{
				ObjC.MessageSend(Handle, Selector.GetHandle("setPopoverContentSize:"), value);
			}
		}

		public bool PopoverVisible => ObjC.MessageSendBool(Handle, Selector.GetHandle("isPopoverVisible"));

		public event EventHandler Dismissed
		{
			add
			{
				Callbacks.Subscribe(this, "popoverControllerDidDismissPopover:", value);
			}
			remove
			{
				Callbacks.Unsubscribe(this, "popoverControllerDidDismissPopover:", value);
			}
		}

		static UIPopoverController()
		{
			_classHandle = ObjC.GetClass("UIPopoverController");
		}

		public UIPopoverController(UIViewController controller)
		{
			Handle = ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("initWithContentViewController:"), controller.Handle);
			ObjC.MessageSend(Handle, Selector.GetHandle("setDelegate:"), Handle);
		}

		internal UIPopoverController(IntPtr handle)
			: base(handle)
		{
		}

		public void PresentFromRect(CGRect rect, UIView view, UIPopoverArrowDirection arrowDirections, bool animated)
		{
			ObjC.MessageSend(Handle, Selector.GetHandle("presentPopoverFromRect:inView:permittedArrowDirections:animated:"), rect, view.Handle, (uint)arrowDirections, animated);
		}

		public void SetContentViewController(UIViewController viewController, bool animated)
		{
			ObjC.MessageSend(Handle, Selector.GetHandle("setContentViewController:animated:"), viewController.Handle, animated);
		}

		public void SetPopoverContentSize(CGSize size, bool animated)
		{
			ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("setPopoverContentSize:animated:"), size, animated);
		}

		public void Dismiss(bool animated)
		{
			ObjC.MessageSend(Handle, Selector.GetHandle("dismissPopoverAnimated:"), animated);
		}
	}
}
