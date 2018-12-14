using System;

namespace ThirdParty.iOS4Unity
{
	public class NSObject : IDisposable
	{
		private static readonly IntPtr _classHandle;

		public IntPtr Handle;

		private readonly bool _shouldRelease;

		public virtual IntPtr ClassHandle => _classHandle;

		public string Description => ObjC.FromNSString(ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("description")));

		static NSObject()
		{
			_classHandle = ObjC.GetClass("NSObject");
		}

		~NSObject()
		{
			Dispose();
		}

		public NSObject(IntPtr handle)
		{
			Handle = handle;
			Runtime.RegisterNSObject(this);
		}

		public NSObject()
		{
			Handle = ObjC.MessageSendIntPtr(ClassHandle, Selector.AllocHandle);
			Runtime.RegisterNSObject(this);
			_shouldRelease = true;
		}

		public override string ToString()
		{
			return Description;
		}

		public virtual void Dispose()
		{
			GC.SuppressFinalize(this);
			if (Handle != IntPtr.Zero)
			{
				Runtime.UnregisterNSObject(Handle);
				Callbacks.UnsubscribeAll(this);
				if (_shouldRelease)
				{
					ObjC.MessageSend(Handle, Selector.ReleaseHandle);
				}
			}
		}
	}
}
