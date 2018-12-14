using System;

namespace ThirdParty.iOS4Unity
{
	public class NSBundle : NSObject
	{
		private static readonly IntPtr _classHandle;

		public override IntPtr ClassHandle => _classHandle;

		public string BundleIdentifier => ObjC.MessageSendString(Handle, Selector.GetHandle("bundleIdentifier"));

		public string BundlePath => ObjC.MessageSendString(Handle, Selector.GetHandle("bundlePath"));

		public static NSBundle MainBundle => Runtime.GetNSObject<NSBundle>(ObjC.MessageSendIntPtr(_classHandle, Selector.GetHandle("mainBundle")));

		public string ResourcePath => ObjC.MessageSendString(Handle, Selector.GetHandle("resourcePath"));

		public NSDictionary InfoDictionary => Runtime.GetNSObject<NSDictionary>(ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("infoDictionary")));

		static NSBundle()
		{
			_classHandle = ObjC.GetClass("NSBundle");
		}

		internal NSBundle(IntPtr handle)
			: base(handle)
		{
		}

		public static NSBundle FromIdentifier(string str)
		{
			return Runtime.GetNSObject<NSBundle>(ObjC.MessageSendIntPtr(_classHandle, Selector.GetHandle("bundleWithIdentifier:"), str));
		}

		public static NSBundle FromPath(string path)
		{
			return Runtime.GetNSObject<NSBundle>(ObjC.MessageSendIntPtr(_classHandle, Selector.GetHandle("bundleWithPath:"), path));
		}

		public string LocalizedString(string key, string value = "", string table = "")
		{
			return ObjC.MessageSendString(Handle, Selector.GetHandle("localizedStringForKey:value:table:"), key, value, table);
		}
	}
}
