using System;
using System.Runtime.InteropServices;

namespace ThirdParty.iOS4Unity.Marshalers
{
	public class NSStringReleaseMarshaler : NSStringMarshaler
	{
		private static readonly NSStringReleaseMarshaler _instance = new NSStringReleaseMarshaler();

		public new static ICustomMarshaler GetInstance(string cookie)
		{
			return _instance;
		}

		public override void CleanUpNativeData(IntPtr pNativeData)
		{
			if (pNativeData != IntPtr.Zero)
			{
				ObjC.MessageSend(pNativeData, Selector.ReleaseHandle);
			}
		}
	}
}
