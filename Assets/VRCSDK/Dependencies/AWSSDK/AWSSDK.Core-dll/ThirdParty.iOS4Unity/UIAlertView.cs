using System;

namespace ThirdParty.iOS4Unity
{
	public class UIAlertView : NSObject
	{
		private static readonly IntPtr _classHandle;

		public override IntPtr ClassHandle => _classHandle;

		public UIAlertViewStyle AlertViewStyle
		{
			get
			{
				return (UIAlertViewStyle)ObjC.MessageSendInt(Handle, Selector.GetHandle("alertViewStyle"));
			}
			set
			{
				ObjC.MessageSend(Handle, Selector.GetHandle("setAlertViewStyle:"), (int)value);
			}
		}

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

		public bool Visible => ObjC.MessageSendBool(Handle, Selector.GetHandle("isVisible"));

		public string Message
		{
			get
			{
				return ObjC.MessageSendString(Handle, Selector.GetHandle("message"));
			}
			set
			{
				ObjC.MessageSend(Handle, Selector.GetHandle("setMessage:"), value);
			}
		}

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

		public event EventHandler<ButtonEventArgs> Clicked
		{
			add
			{
				Callbacks.Subscribe(this, "alertView:clickedButtonAtIndex:", value);
			}
			remove
			{
				Callbacks.Unsubscribe(this, "alertView:clickedButtonAtIndex:", value);
			}
		}

		public event EventHandler<ButtonEventArgs> Dismissed
		{
			add
			{
				Callbacks.Subscribe(this, "alertView:didDismissWithButtonIndex:", value);
			}
			remove
			{
				Callbacks.Unsubscribe(this, "alertView:didDismissWithButtonIndex:", value);
			}
		}

		public event EventHandler<ButtonEventArgs> WillDismiss
		{
			add
			{
				Callbacks.Subscribe(this, "alertView:willDismissWithButtonIndex:", value);
			}
			remove
			{
				Callbacks.Unsubscribe(this, "alertView:willDismissWithButtonIndex:", value);
			}
		}

		public event EventHandler Canceled
		{
			add
			{
				Callbacks.Subscribe(this, "alertViewCancel:", value);
			}
			remove
			{
				Callbacks.Unsubscribe(this, "alertViewCancel:", value);
			}
		}

		public event EventHandler Presented
		{
			add
			{
				Callbacks.Subscribe(this, "didPresentAlertView:", value);
			}
			remove
			{
				Callbacks.Unsubscribe(this, "didPresentAlertView:", value);
			}
		}

		public event EventHandler WillPresent
		{
			add
			{
				Callbacks.Subscribe(this, "willPresentAlertView:", value);
			}
			remove
			{
				Callbacks.Unsubscribe(this, "willPresentAlertView:", value);
			}
		}

		static UIAlertView()
		{
			_classHandle = ObjC.GetClass("UIAlertView");
		}

		public UIAlertView()
		{
			Handle = ObjC.MessageSendIntPtr(Handle, Selector.Init);
			ObjC.MessageSend(Handle, Selector.GetHandle("setDelegate:"), Handle);
		}

		internal UIAlertView(IntPtr handle)
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

		public void Show()
		{
			ObjC.MessageSend(Handle, Selector.GetHandle("show"));
		}

		public void Dismiss(int buttonIndex, bool animated = true)
		{
			ObjC.MessageSend(Handle, Selector.GetHandle("dismissWithClickedButtonIndex:animated:"), buttonIndex, animated);
		}
	}
}
