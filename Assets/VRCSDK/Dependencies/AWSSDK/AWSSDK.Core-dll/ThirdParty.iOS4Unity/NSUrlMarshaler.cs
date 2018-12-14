using System;
using System.Runtime.InteropServices;

namespace ThirdParty.iOS4Unity
{
	public class NSUrlMarshaler : ICustomMarshaler
	{
		private static readonly NSUrlMarshaler _instance = new NSUrlMarshaler();

		public static ICustomMarshaler GetInstance(string cookie)
		{
			return _instance;
		}

		public void CleanUpManagedData(object managedObj)
		{
		}

		public void CleanUpNativeData(IntPtr pNativeData)
		{
		}

		public int GetNativeDataSize()
		{
			return IntPtr.Size;
		}

		public IntPtr MarshalManagedToNative(object managedObj)
		{
			string text = managedObj as string;
			if (text == null)
			{
				return IntPtr.Zero;
			}
			return ObjC.ToNSUrl(text);
		}

		public object MarshalNativeToManaged(IntPtr pNativeData)
		{
			if (pNativeData == IntPtr.Zero)
			{
				return null;
			}
			return ObjC.FromNSUrl(pNativeData);
		}
	}
}
