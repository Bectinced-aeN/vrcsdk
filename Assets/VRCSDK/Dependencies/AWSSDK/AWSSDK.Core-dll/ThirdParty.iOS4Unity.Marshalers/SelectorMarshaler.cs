using System;
using System.Runtime.InteropServices;

namespace ThirdParty.iOS4Unity.Marshalers
{
	public class SelectorMarshaler : ICustomMarshaler
	{
		private static readonly SelectorMarshaler _instance = new SelectorMarshaler();

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
			if (string.IsNullOrEmpty(text))
			{
				return IntPtr.Zero;
			}
			return ObjC.GetSelector(text);
		}

		public object MarshalNativeToManaged(IntPtr pNativeData)
		{
			return ObjC.GetSelectorName(pNativeData);
		}
	}
}
