using System;
using System.Runtime.InteropServices;

namespace ThirdParty.iOS4Unity.Marshalers
{
	public class NSStringMarshaler : ICustomMarshaler
	{
		private static readonly NSStringMarshaler _instance = new NSStringMarshaler();

		public static ICustomMarshaler GetInstance(string cookie)
		{
			return _instance;
		}

		public void CleanUpManagedData(object managedObj)
		{
		}

		public virtual void CleanUpNativeData(IntPtr pNativeData)
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
			return ObjC.ToNSString(text);
		}

		public object MarshalNativeToManaged(IntPtr pNativeData)
		{
			if (pNativeData == IntPtr.Zero)
			{
				return null;
			}
			return ObjC.FromNSString(pNativeData);
		}
	}
}
