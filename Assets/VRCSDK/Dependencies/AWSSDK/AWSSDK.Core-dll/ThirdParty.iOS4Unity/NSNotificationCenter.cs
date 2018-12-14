using System;

namespace ThirdParty.iOS4Unity
{
	public sealed class NSNotificationCenter : NSObject
	{
		private class Observer : NSObject
		{
			private static readonly IntPtr _classHandle;

			public readonly Action<NSNotification> Action;

			public override IntPtr ClassHandle => _classHandle;

			static Observer()
			{
				_classHandle = ObjC.AllocateClassPair(ObjC.GetClass("NSObject"), "__Observer", 0);
			}

			public Observer(Action<NSNotification> action)
			{
				Action = action;
			}

			public override void Dispose()
			{
				DefaultCenter.RemoveObserver(this);
				base.Dispose();
			}
		}

		private const string SelectorName = "__onNotification:";

		private static readonly IntPtr _classHandle;

		public override IntPtr ClassHandle => _classHandle;

		public static NSNotificationCenter DefaultCenter => Runtime.GetNSObject<NSNotificationCenter>(ObjC.MessageSendIntPtr(_classHandle, Selector.GetHandle("defaultCenter")));

		static NSNotificationCenter()
		{
			_classHandle = ObjC.GetClass("NSNotificationCenter");
		}

		internal NSNotificationCenter(IntPtr handle)
			: base(handle)
		{
		}

		public NSObject AddObserver(string name, Action<NSNotification> action, NSObject fromObject = null)
		{
			Observer observer = new Observer(action);
			Callbacks.Subscribe(observer, "__onNotification:", delegate(IntPtr n)
			{
				action(Runtime.GetNSObject<NSNotification>(n));
			});
			ObjC.MessageSend(Handle, Selector.GetHandle("addObserver:selector:name:object:"), observer.Handle, Selector.GetHandle("__onNotification:"), name, fromObject?.Handle ?? IntPtr.Zero);
			return observer;
		}

		public void PostNotificationName(string name, NSObject obj = null)
		{
			ObjC.MessageSend(Handle, Selector.GetHandle("postNotificationName:object:"), name, obj?.Handle ?? IntPtr.Zero);
		}

		public void RemoveObserver(NSObject observer)
		{
			ObjC.MessageSend(Handle, Selector.GetHandle("removeObserver:"), observer.Handle);
		}
	}
}
