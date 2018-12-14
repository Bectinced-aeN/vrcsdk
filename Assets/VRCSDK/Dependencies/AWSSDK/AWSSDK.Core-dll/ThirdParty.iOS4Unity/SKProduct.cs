using System;

namespace ThirdParty.iOS4Unity
{
	public class SKProduct : NSObject
	{
		private static readonly IntPtr _classHandle;

		public override IntPtr ClassHandle => _classHandle;

		public bool Downloadable => ObjC.MessageSendBool(Handle, Selector.GetHandle("isDownloadable"));

		public string LocalizedDescription => ObjC.MessageSendString(Handle, Selector.GetHandle("localizedDescription"));

		public string LocalizedTitle => ObjC.MessageSendString(Handle, Selector.GetHandle("localizedTitle"));

		public double Price => ObjC.FromNSNumber(ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("price")));

		public NSLocale PriceLocale => Runtime.GetNSObject<NSLocale>(ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("priceLocale")));

		public string ProductIdentifier => ObjC.MessageSendString(Handle, Selector.GetHandle("productIdentifier"));

		public SKPaymentTransactionState TransactionState => (SKPaymentTransactionState)ObjC.MessageSendInt(Handle, Selector.GetHandle("transactionState"));

		static SKProduct()
		{
			_classHandle = ObjC.GetClass("SKProduct");
		}

		internal SKProduct(IntPtr handle)
			: base(handle)
		{
		}
	}
}
