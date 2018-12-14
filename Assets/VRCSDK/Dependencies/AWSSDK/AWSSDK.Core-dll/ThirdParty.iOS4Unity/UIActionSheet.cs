using System;

namespace ThirdParty.iOS4Unity
{
	public class UIActionSheet : UIView
	{
		private static readonly IntPtr _classHandle;

		public override IntPtr ClassHandle => _classHandle;

		public int ButtonCount => ObjC.MessageSendInt(Handle, Selector.GetHandle("numberOfButtons"));

		public int CancelButtonIndex
		{
			get
			{
				return ObjC.MessageSendInt(Handle, Selector.GetHandle("cancelButtonIndex"));
			}
			set
			{
				ObjC.MessageSend(Handle, Selector.GetHandle("setCancelButtonIndex:"), value);
			}
		}

		public int FirstOtherButtonIndex => ObjC.MessageSendInt(Handle, Selector.GetHandle("firstOtherButtonIndex"));

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

		public bool Visible => ObjC.MessageSendBool(Handle, Selector.GetHandle("isVisible"));

		public event EventHandler<ButtonEventArgs> Clicked
		{
			add
			{
				Callbacks.Subscribe(this, "actionSheet:clickedButtonAtIndex:", value);
			}
			remove
			{
				Callbacks.Unsubscribe(this, "actionSheet:clickedButtonAtIndex:", value);
			}
		}

		public event EventHandler<ButtonEventArgs> Dismissed
		{
			add
			{
				Callbacks.Subscribe(this, "actionSheet:didDismissWithButtonIndex:", value);
			}
			remove
			{
				Callbacks.Unsubscribe(this, "actionSheet:didDismissWithButtonIndex:", value);
			}
		}

		public event EventHandler<ButtonEventArgs> WillDismiss
		{
			add
			{
				Callbacks.Subscribe(this, "actionSheet:willDismissWithButtonIndex:", value);
			}
			remove
			{
				Callbacks.Unsubscribe(this, "actionSheet:willDismissWithButtonIndex:", value);
			}
		}

		public event EventHandler Canceled
		{
			add
			{
				Callbacks.Subscribe(this, "actionSheetCancel:", value);
			}
			remove
			{
				Callbacks.Unsubscribe(this, "actionSheetCancel:", value);
			}
		}

		public event EventHandler Presented
		{
			add
			{
				Callbacks.Subscribe(this, "didPresentActionSheet:", value);
			}
			remove
			{
				Callbacks.Unsubscribe(this, "didPresentActionSheet:", value);
			}
		}

		public event EventHandler WillPresent
		{
			add
			{
				Callbacks.Subscribe(this, "willPresentActionSheet:", value);
			}
			remove
			{
				Callbacks.Unsubscribe(this, "willPresentActionSheet:", value);
			}
		}

		static UIActionSheet()
		{
			_classHandle = ObjC.GetClass("UIActionSheet");
		}

		public UIActionSheet()
		{
			Handle = ObjC.MessageSendIntPtr(Handle, Selector.Init);
			ObjC.MessageSend(Handle, Selector.GetHandle("setDelegate:"), Handle);
		}

		internal UIActionSheet(IntPtr handle)
			: base(handle)
		{
		}

		public int AddButton(string title)
		{
			return ObjC.MessageSendInt(Handle, Selector.GetHandle("addButtonWithTitle:"), title);
		}

		public string ButtonTitle(int index)
		{
			return ObjC.MessageSendString(Handle, Selector.GetHandle("buttonTitleAtIndex:"), index);
		}

		public void DismissWithClickedButtonIndex(int buttonIndex, bool animated)
		{
			ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("dismissWithClickedButtonIndex:animated:"), buttonIndex, animated);
		}

		public void ShowInView(UIView view)
		{
			ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("showInView:"), view.Handle);
		}
	}
}
