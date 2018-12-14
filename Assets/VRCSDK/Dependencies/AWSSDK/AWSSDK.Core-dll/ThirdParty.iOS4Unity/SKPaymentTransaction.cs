using System;

namespace ThirdParty.iOS4Unity
{
	public class SKPaymentTransaction : NSObject
	{
		private static readonly IntPtr _classHandle;

		public override IntPtr ClassHandle => _classHandle;

		public NSError Error
		{
			get
			{
				IntPtr intPtr = ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("error"));
				if (!(intPtr == IntPtr.Zero))
				{
					return Runtime.GetNSObject<NSError>(intPtr);
				}
				return null;
			}
		}

		public SKPaymentTransaction OriginalTransaction
		{
			get
			{
				IntPtr intPtr = ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("originalTransaction"));
				if (!(intPtr == IntPtr.Zero))
				{
					return Runtime.GetNSObject<SKPaymentTransaction>(intPtr);
				}
				return null;
			}
		}

		public DateTime TransactionDate => (DateTime)ObjC.MessageSendDate(Handle, Selector.GetHandle("transactionDate"));

		public string TransactionIdentifier => ObjC.MessageSendString(Handle, Selector.GetHandle("transactionIdentifier"));

		static SKPaymentTransaction()
		{
			_classHandle = ObjC.GetClass("SKPaymentTransaction");
		}

		internal SKPaymentTransaction(IntPtr handle)
			: base(handle)
		{
		}
	}
}
