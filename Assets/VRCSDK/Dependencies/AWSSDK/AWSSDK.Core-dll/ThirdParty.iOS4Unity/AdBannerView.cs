using System;
using System.Collections.Generic;

namespace ThirdParty.iOS4Unity
{
	public class AdBannerView : UIView
	{
		private static readonly IntPtr _classHandle;

		private Dictionary<object, IntPtrHandler2> _failedToReceiveAd;

		public override IntPtr ClassHandle => _classHandle;

		public AdType AdType => (AdType)ObjC.MessageSendInt(Handle, Selector.GetHandle("adType"));

		public bool BannerLoaded => ObjC.MessageSendBool(Handle, Selector.GetHandle("isBannerLoaded"));

		public event EventHandler AdLoaded
		{
			add
			{
				Callbacks.Subscribe(this, "bannerViewDidLoadAd:", value);
			}
			remove
			{
				Callbacks.Unsubscribe(this, "bannerViewDidLoadAd:", value);
			}
		}

		public event EventHandler ActionFinished
		{
			add
			{
				Callbacks.Subscribe(this, "bannerViewActionDidFinish:", value);
			}
			remove
			{
				Callbacks.Unsubscribe(this, "bannerViewActionDidFinish:", value);
			}
		}

		public event EventHandler<NSErrorEventArgs> FailedToReceiveAd
		{
			add
			{
				if (_failedToReceiveAd == null)
				{
					_failedToReceiveAd = new Dictionary<object, IntPtrHandler2>();
				}
				IntPtrHandler2 intPtrHandler = delegate(IntPtr _, IntPtr i)
				{
					value(this, new NSErrorEventArgs
					{
						Error = Runtime.GetNSObject<NSError>(i)
					});
				};
				_failedToReceiveAd[value] = intPtrHandler;
				Callbacks.Subscribe(this, "bannerView:didFailToReceiveAdWithError:", intPtrHandler);
			}
			remove
			{
				if (_failedToReceiveAd != null && _failedToReceiveAd.TryGetValue(value, out IntPtrHandler2 value2))
				{
					_failedToReceiveAd.Remove(value);
					Callbacks.Unsubscribe(this, "bannerView:didFailToReceiveAdWithError:", value2);
				}
			}
		}

		public event EventHandler WillLoad
		{
			add
			{
				Callbacks.Subscribe(this, "bannerViewWillLoadAd:", value);
			}
			remove
			{
				Callbacks.Unsubscribe(this, "bannerViewWillLoadAd:", value);
			}
		}

		static AdBannerView()
		{
			_classHandle = ObjC.GetClass("ADBannerView");
		}

		public AdBannerView()
		{
			ObjC.MessageSend(Handle, Selector.GetHandle("setDelegate:"), Handle);
		}

		public AdBannerView(CGRect frame)
			: base(frame)
		{
			ObjC.MessageSend(Handle, Selector.GetHandle("setDelegate:"), Handle);
		}

		public AdBannerView(AdType type)
		{
			ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("initWithAdType:"), (int)type);
			ObjC.MessageSend(Handle, Selector.GetHandle("setDelegate:"), Handle);
		}

		internal AdBannerView(IntPtr handle)
			: base(handle)
		{
		}

		public void CancelBannerViewAction()
		{
			ObjC.MessageSend(Handle, Selector.GetHandle("cancelBannerViewAction"));
		}
	}
}
