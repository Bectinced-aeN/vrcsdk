using System;
using System.Collections.Generic;

namespace ThirdParty.iOS4Unity
{
	public class SKPaymentQueue : NSObject
	{
		private static readonly IntPtr _classHandle;

		private Dictionary<object, IntPtrHandler2> _restoreFailed;

		private Dictionary<object, IntPtrHandler2> _updatedTransactions;

		public override IntPtr ClassHandle => _classHandle;

		public static bool CanMakePayments => ObjC.MessageSendBool(_classHandle, Selector.GetHandle("canMakePayments"));

		public SKPaymentTransaction[] Transactions => ObjC.FromNSArray<SKPaymentTransaction>(ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("transactions")));

		public static SKPaymentQueue DefaultQueue => Runtime.GetNSObject<SKPaymentQueue>(ObjC.MessageSendIntPtr(_classHandle, Selector.GetHandle("defaultQueue")));

		public event EventHandler RestoreCompleted
		{
			add
			{
				Callbacks.Subscribe(this, "paymentQueueRestoreCompletedTransactionsFinished:", value);
			}
			remove
			{
				Callbacks.Unsubscribe(this, "paymentQueueRestoreCompletedTransactionsFinished:", value);
			}
		}

		public event EventHandler<NSErrorEventArgs> RestoreFailed
		{
			add
			{
				if (_restoreFailed == null)
				{
					_restoreFailed = new Dictionary<object, IntPtrHandler2>();
				}
				IntPtrHandler2 intPtrHandler = delegate(IntPtr _, IntPtr i)
				{
					value(this, new NSErrorEventArgs
					{
						Error = Runtime.GetNSObject<NSError>(i)
					});
				};
				_restoreFailed[value] = intPtrHandler;
				Callbacks.Subscribe(this, "paymentQueue:restoreCompletedTransactionsFailedWithError:", intPtrHandler);
			}
			remove
			{
				if (_restoreFailed != null && _restoreFailed.TryGetValue(value, out IntPtrHandler2 value2))
				{
					_restoreFailed.Remove(value);
					Callbacks.Unsubscribe(this, "paymentQueue:restoreCompletedTransactionsFailedWithError:", value2);
				}
			}
		}

		public event EventHandler<SKPaymentTransactionEventArgs> UpdatedTransactions
		{
			add
			{
				if (_updatedTransactions == null)
				{
					_updatedTransactions = new Dictionary<object, IntPtrHandler2>();
				}
				IntPtrHandler2 intPtrHandler = delegate(IntPtr _, IntPtr i)
				{
					value(this, new SKPaymentTransactionEventArgs
					{
						Transactions = ObjC.FromNSArray<SKPaymentTransaction>(i)
					});
				};
				_updatedTransactions[value] = intPtrHandler;
				Callbacks.Subscribe(this, "paymentQueue:updatedTransactions:", intPtrHandler);
			}
			remove
			{
				if (_updatedTransactions != null && _updatedTransactions.TryGetValue(value, out IntPtrHandler2 value2))
				{
					_updatedTransactions.Remove(value);
					Callbacks.Unsubscribe(this, "paymentQueue:updatedTransactions:", value2);
				}
			}
		}

		static SKPaymentQueue()
		{
			_classHandle = ObjC.GetClass("SKPaymentQueue");
		}

		internal SKPaymentQueue(IntPtr handle)
			: base(handle)
		{
			ObjC.MessageSend(Handle, Selector.GetHandle("addTransactionObserver:"), Handle);
		}

		public void AddPayment(SKPayment payment)
		{
			ObjC.MessageSend(Handle, Selector.GetHandle("addPayment:"), payment.Handle);
		}

		public void RestoreCompletedTransactions()
		{
			ObjC.MessageSend(Handle, Selector.GetHandle("restoreCompletedTransactions"));
		}

		public void FinishTransaction(SKPaymentTransaction transaction)
		{
			ObjC.MessageSend(Handle, Selector.GetHandle("finishTransaction:"), transaction.Handle);
		}

		public override void Dispose()
		{
			ObjC.MessageSend(Handle, Selector.GetHandle("removeTransactionObserver:"), Handle);
			base.Dispose();
		}
	}
}
