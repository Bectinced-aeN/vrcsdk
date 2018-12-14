using System;
using System.Runtime.InteropServices;

namespace ThirdParty.iOS4Unity.Marshalers
{
	public class NSDateMarshaler : ICustomMarshaler
	{
		private static readonly NSDateMarshaler _instance = new NSDateMarshaler();

		public static ICustomMarshaler GetInstance(string cookie)
		{
			return _instance;
		}

		public void CleanUpManagedData(object ManagedObj)
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
			return ObjC.ToNSDate((DateTime)managedObj);
		}

		public object MarshalNativeToManaged(IntPtr pNativeData)
		{
			if (pNativeData == IntPtr.Zero)
			{
				return default(DateTime);
			}
			return ObjC.FromNSDate(pNativeData);
		}
	}
}
