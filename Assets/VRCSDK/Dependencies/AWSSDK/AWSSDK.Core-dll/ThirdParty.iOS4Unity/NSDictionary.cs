using System;

namespace ThirdParty.iOS4Unity
{
	public class NSDictionary : NSObject
	{
		private static readonly IntPtr _classHandle;

		public override IntPtr ClassHandle => _classHandle;

		public virtual NSObject this[string key]
		{
			get
			{
				return ObjectForKey(key);
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public uint Count => ObjC.MessageSendUInt(Handle, Selector.GetHandle("count"));

		public string[] Keys => ObjC.FromNSArray(Runtime.GetNSObject<NSObject>(ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("allKeys"))).Handle);

		public NSObject[] Values => ObjC.FromNSArray<NSObject>(Runtime.GetNSObject<NSObject>(ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("allValues"))).Handle);

		static NSDictionary()
		{
			_classHandle = ObjC.GetClass("NSDictionary");
		}

		internal NSDictionary(IntPtr handle)
			: base(handle)
		{
		}

		public NSDictionary()
		{
			Handle = ObjC.MessageSendIntPtr(Handle, Selector.Init);
		}

		public static NSDictionary FromDictionary(NSDictionary dictionary)
		{
			return FromObjectsAndKeys(dictionary.Values, dictionary.Keys);
		}

		public static NSDictionary FromObjectAndKey(NSObject obj, string key)
		{
			IntPtr intPtr = ObjC.ToNSString(key);
			NSDictionary nSObject = Runtime.GetNSObject<NSDictionary>(ObjC.MessageSendIntPtr(_classHandle, Selector.GetHandle("dictionaryWithObject:forKey:"), obj.Handle, intPtr));
			ObjC.MessageSend(intPtr, Selector.ReleaseHandle);
			return nSObject;
		}

		public static NSDictionary FromObjectsAndKeys(NSObject[] objs, string[] keys)
		{
			IntPtr arg = ObjC.ToNSArray(objs);
			IntPtr intPtr = ObjC.ToNSArray(keys);
			NSDictionary nSObject = Runtime.GetNSObject<NSDictionary>(ObjC.MessageSendIntPtr(_classHandle, Selector.GetHandle("dictionaryWithObjects:forKeys:"), arg, intPtr));
			ObjC.ReleaseNSArrayItems(intPtr);
			return nSObject;
		}

		public NSObject ObjectForKey(string key)
		{
			IntPtr intPtr = ObjC.ToNSString(key);
			NSObject nSObject = Runtime.GetNSObject<NSObject>(ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("objectForKey:"), intPtr));
			ObjC.MessageSend(intPtr, Selector.ReleaseHandle);
			return nSObject;
		}

		public NSObject[] ObjectsForKeys(string[] keys)
		{
			IntPtr intPtr = ObjC.ToNSArray(keys);
			NSObject[] result = ObjC.FromNSArray<NSObject>(Runtime.GetNSObject<NSObject>(ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("objectsForKeys:notFoundMarker:"), intPtr, new NSObject().Handle)).Handle);
			ObjC.ReleaseNSArrayItems(intPtr);
			return result;
		}

		public string[] KeysForObject(NSObject obj)
		{
			return ObjC.FromNSArray(Runtime.GetNSObject<NSObject>(ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("allKeysForObject:"), obj.Handle)).Handle);
		}

		public static NSDictionary FromFile(string path)
		{
			return Runtime.GetNSObject<NSDictionary>(ObjC.MessageSendIntPtr(_classHandle, Selector.GetHandle("dictionaryWithContentsOfFile:"), path));
		}

		public bool ContainsKey(string key)
		{
			return ObjectForKey(key) != null;
		}
	}
}
