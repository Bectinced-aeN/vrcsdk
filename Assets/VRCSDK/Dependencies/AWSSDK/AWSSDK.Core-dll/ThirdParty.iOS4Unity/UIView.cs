using System;

namespace ThirdParty.iOS4Unity
{
	public class UIView : NSObject
	{
		private static readonly IntPtr _classHandle;

		public override IntPtr ClassHandle => _classHandle;

		public CGRect Frame
		{
			get
			{
				return ObjC.MessageSendCGRect(Handle, Selector.GetHandle("frame"));
			}
			set
			{
				ObjC.MessageSend(Handle, Selector.GetHandle("setFrame:"), value);
			}
		}

		public CGRect Bounds
		{
			get
			{
				return ObjC.MessageSendCGRect(Handle, Selector.GetHandle("bounds"));
			}
			set
			{
				ObjC.MessageSend(Handle, Selector.GetHandle("setBounds:"), value);
			}
		}

		public CGPoint Center
		{
			get
			{
				return ObjC.MessageSendCGPoint(Handle, "center");
			}
			set
			{
				ObjC.MessageSend(Handle, Selector.GetHandle("setCenter:"), value);
			}
		}

		public UIView[] Subviews => ObjC.FromNSArray<UIView>(ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("subviews")));

		public UIView Superview => Runtime.GetNSObject<UIView>(ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("superview")));

		public bool Hidden
		{
			get
			{
				return ObjC.MessageSendBool(Handle, Selector.GetHandle("isHidden"));
			}
			set
			{
				ObjC.MessageSend(Handle, Selector.GetHandle("setHidden:"), value);
			}
		}

		public float Alpha
		{
			get
			{
				return ObjC.MessageSendFloat(Handle, Selector.GetHandle("alpha"));
			}
			set
			{
				ObjC.MessageSend(Handle, Selector.GetHandle("setAlpha:"), value);
			}
		}

		public UIWindow Window => Runtime.GetNSObject<UIWindow>(ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("window")));

		static UIView()
		{
			_classHandle = ObjC.GetClass("UIView");
		}

		public UIView()
		{
			ObjC.MessageSendIntPtr(Handle, Selector.Init);
		}

		public UIView(CGRect frame)
		{
			Handle = ObjC.MessageSendIntPtr(Handle, Selector.InitWithFrame, frame);
		}

		internal UIView(IntPtr handle)
			: base(handle)
		{
		}

		public void AddSubview(UIView view)
		{
			ObjC.MessageSend(Handle, Selector.GetHandle("addSubview:"), view.Handle);
		}

		public void BringSubviewToFront(UIView view)
		{
			ObjC.MessageSend(Handle, Selector.GetHandle("bringSubviewToFront:"), view.Handle);
		}

		public void SendSubviewToBack(UIView view)
		{
			ObjC.MessageSend(Handle, Selector.GetHandle("sendSubviewToBack:"), view.Handle);
		}

		public void RemoveFromSuperview()
		{
			ObjC.MessageSend(Handle, Selector.GetHandle("removeFromSuperview"));
		}
	}
}
