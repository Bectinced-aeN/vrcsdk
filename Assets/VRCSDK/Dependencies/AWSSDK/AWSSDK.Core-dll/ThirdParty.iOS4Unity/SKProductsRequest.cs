using System;
using System.Collections.Generic;

namespace ThirdParty.iOS4Unity
{
	public class SKProductsRequest : NSObject
	{
		private static readonly IntPtr _classHandle;

		private Dictionary<object, IntPtrHandler2> _receivedResponse;

		private Dictionary<object, IntPtrHandler2> _failed;

		public override IntPtr ClassHandle => _classHandle;

		public event EventHandler<SKProductsResponseEventArgs> ReceivedResponse
		{
			add
			{
				if (_receivedResponse == null)
				{
					_receivedResponse = new Dictionary<object, IntPtrHandler2>();
				}
				IntPtrHandler2 intPtrHandler = delegate(IntPtr _, IntPtr i)
				{
					value(this, new SKProductsResponseEventArgs
					{
						Response = Runtime.GetNSObject<SKProductsResponse>(i)
					});
				};
				_receivedResponse[value] = intPtrHandler;
				Callbacks.Subscribe(this, "productsRequest:didReceiveResponse:", intPtrHandler);
			}
			remove
			{
				if (_receivedResponse != null && _receivedResponse.TryGetValue(value, out IntPtrHandler2 value2))
				{
					_receivedResponse.Remove(value);
					Callbacks.Unsubscribe(this, "productsRequest:didReceiveResponse:", value2);
				}
			}
		}

		public event EventHandler<NSErrorEventArgs> Failed
		{
			add
			{
				if (_failed == null)
				{
					_failed = new Dictionary<object, IntPtrHandler2>();
				}
				IntPtrHandler2 intPtrHandler = delegate(IntPtr _, IntPtr i)
				{
					value(this, new NSErrorEventArgs
					{
						Error = Runtime.GetNSObject<NSError>(i)
					});
				};
				_failed[value] = intPtrHandler;
				Callbacks.Subscribe(this, "request:didFailWithError:", intPtrHandler);
			}
			remove
			{
				if (_failed != null && _failed.TryGetValue(value, out IntPtrHandler2 value2))
				{
					_failed.Remove(value);
					Callbacks.Unsubscribe(this, "request:didFailWithError:", value2);
				}
			}
		}

		public event EventHandler Finished
		{
			add
			{
				Callbacks.Subscribe(this, "requestDidFinish:", value);
			}
			remove
			{
				Callbacks.Unsubscribe(this, "requestDidFinish:", value);
			}
		}

		static SKProductsRequest()
		{
			_classHandle = ObjC.GetClass("SKProductsRequest");
		}

		public SKProductsRequest(params string[] productIds)
		{
			ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("initWithProductIdentifiers:"), ObjC.ToNSSet(productIds));
			ObjC.MessageSend(Handle, Selector.GetHandle("setDelegate:"), Handle);
		}

		internal SKProductsRequest(IntPtr handle)
			: base(handle)
		{
		}

		public void Cancel()
		{
			ObjC.MessageSend(Handle, Selector.GetHandle("cancel"));
		}

		public void Start()
		{
			ObjC.MessageSend(Handle, Selector.GetHandle("start"));
		}
	}
}
