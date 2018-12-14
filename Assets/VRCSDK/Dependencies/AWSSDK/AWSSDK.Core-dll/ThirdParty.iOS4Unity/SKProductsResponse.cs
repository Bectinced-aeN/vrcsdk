using System;

namespace ThirdParty.iOS4Unity
{
	public sealed class SKProductsResponse : NSObject
	{
		private static readonly IntPtr _classHandle;

		public override IntPtr ClassHandle => _classHandle;

		public string[] InvalidProducts => ObjC.FromNSArray(ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("invalidProductIdentifiers")));

		public SKProduct[] Products => ObjC.FromNSArray<SKProduct>(ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("products")));

		static SKProductsResponse()
		{
			_classHandle = ObjC.GetClass("SKProductsResponse");
		}

		internal SKProductsResponse(IntPtr handle)
			: base(handle)
		{
		}
	}
}
