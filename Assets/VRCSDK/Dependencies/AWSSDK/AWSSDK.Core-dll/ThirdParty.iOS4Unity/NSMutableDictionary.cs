using System;

namespace ThirdParty.iOS4Unity
{
	public class NSMutableDictionary : NSDictionary
	{
		private static readonly IntPtr _classHandle;

		public override IntPtr ClassHandle => _classHandle;

		public override NSObject this[string key]
		{
			get
			{
				return ObjectForKey(key);
			}
			set
			{
				SetObjectForKey(value, key);
			}
		}

		static NSMutableDictionary()
		{
			_classHandle = ObjC.GetClass("NSMutableDictionary");
		}

		internal NSMutableDictionary(IntPtr handle)
			: base(handle)
		{
		}

		public NSMutableDictionary()
		{
			Handle = ObjC.MessageSendIntPtr(Handle, Selector.Init);
		}

		public new static NSMutableDictionary FromDictionary(NSDictionary dictionary)
		{
			return FromObjectsAndKeys(dictionary.Values, dictionary.Keys);
		}

		public new static NSMutableDictionary FromObjectAndKey(NSObject obj, string key)
		{
			IntPtr intPtr = ObjC.ToNSString(key);
			NSMutableDictionary nSObject = Runtime.GetNSObject<NSMutableDictionary>(ObjC.MessageSendIntPtr(_classHandle, Selector.GetHandle("dictionaryWithObject:forKey:"), obj.Handle, intPtr));
			ObjC.MessageSend(intPtr, Selector.ReleaseHandle);
			return nSObject;
		}

		public new static NSMutableDictionary FromObjectsAndKeys(NSObject[] objects, string[] keys)
		{
			IntPtr arg = ObjC.ToNSArray(objects);
			IntPtr intPtr = ObjC.ToNSArray(keys);
			NSMutableDictionary nSObject = Runtime.GetNSObject<NSMutableDictionary>(ObjC.MessageSendIntPtr(_classHandle, Selector.GetHandle("dictionaryWithObjects:forKeys:"), arg, intPtr));
			ObjC.ReleaseNSArrayItems(intPtr);
			return nSObject;
		}

		public void SetObjectForKey(NSObject obj, string key)
		{
			IntPtr intPtr = ObjC.ToNSString(key);
			ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("setObject:forKey:"), obj.Handle, intPtr);
			ObjC.MessageSend(intPtr, Selector.ReleaseHandle);
		}

		public void RemoveObjectForKey(string key)
		{
			IntPtr intPtr = ObjC.ToNSString(key);
			ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("removeObjectForKey:"), intPtr);
			ObjC.MessageSend(intPtr, Selector.ReleaseHandle);
		}

		public new static NSMutableDictionary FromFile(string path)
		{
			return Runtime.GetNSObject<NSMutableDictionary>(ObjC.MessageSendIntPtr(_classHandle, Selector.GetHandle("dictionaryWithContentsOfFile:"), path));
		}

		public void Clear()
		{
			ObjC.MessageSend(Handle, Selector.GetHandle("removeAllObjects"));
		}
	}
}
