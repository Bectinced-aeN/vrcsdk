using System;
using System.Collections.Generic;

namespace ThirdParty.iOS4Unity
{
	public static class Runtime
	{
		private static object _contructorLock = new object();

		private static object _objectLock = new object();

		private static readonly Dictionary<Type, Func<IntPtr, object>> _constructors = new Dictionary<Type, Func<IntPtr, object>>
		{
			{
				typeof(AdBannerView),
				(IntPtr h) => new AdBannerView(h)
			},
			{
				typeof(NSBundle),
				(IntPtr h) => new NSBundle(h)
			},
			{
				typeof(NSData),
				(IntPtr h) => new NSData(h)
			},
			{
				typeof(NSDictionary),
				(IntPtr h) => new NSDictionary(h)
			},
			{
				typeof(NSMutableDictionary),
				(IntPtr h) => new NSMutableDictionary(h)
			},
			{
				typeof(NSError),
				(IntPtr h) => new NSError(h)
			},
			{
				typeof(NSLocale),
				(IntPtr h) => new NSLocale(h)
			},
			{
				typeof(NSNotification),
				(IntPtr h) => new NSNotification(h)
			},
			{
				typeof(NSNotificationCenter),
				(IntPtr h) => new NSNotificationCenter(h)
			},
			{
				typeof(NSNumberFormatter),
				(IntPtr h) => new NSNumberFormatter(h)
			},
			{
				typeof(NSObject),
				(IntPtr h) => new NSObject(h)
			},
			{
				typeof(NSTimeZone),
				(IntPtr h) => new NSTimeZone(h)
			},
			{
				typeof(SKPayment),
				(IntPtr h) => new SKPayment(h)
			},
			{
				typeof(SKPaymentQueue),
				(IntPtr h) => new SKPaymentQueue(h)
			},
			{
				typeof(SKPaymentTransaction),
				(IntPtr h) => new SKPaymentTransaction(h)
			},
			{
				typeof(SKProduct),
				(IntPtr h) => new SKProduct(h)
			},
			{
				typeof(SKProductsRequest),
				(IntPtr h) => new SKProductsRequest(h)
			},
			{
				typeof(SKProductsResponse),
				(IntPtr h) => new SKProductsResponse(h)
			},
			{
				typeof(UIActionSheet),
				(IntPtr h) => new UIActionSheet(h)
			},
			{
				typeof(UIActivityViewController),
				(IntPtr h) => new UIActivityViewController(h)
			},
			{
				typeof(UIAlertView),
				(IntPtr h) => new UIAlertView(h)
			},
			{
				typeof(UIApplication),
				(IntPtr h) => new UIApplication(h)
			},
			{
				typeof(UIDevice),
				(IntPtr h) => new UIDevice(h)
			},
			{
				typeof(UIImage),
				(IntPtr h) => new UIImage(h)
			},
			{
				typeof(UILocalNotification),
				(IntPtr h) => new UILocalNotification(h)
			},
			{
				typeof(UIScreen),
				(IntPtr h) => new UIScreen(h)
			},
			{
				typeof(UIScreenMode),
				(IntPtr h) => new UIScreenMode(h)
			},
			{
				typeof(UIUserNotificationSettings),
				(IntPtr h) => new UIUserNotificationSettings(h)
			},
			{
				typeof(UIView),
				(IntPtr h) => new UIView(h)
			},
			{
				typeof(UIViewController),
				(IntPtr h) => new UIViewController(h)
			},
			{
				typeof(UIWindow),
				(IntPtr h) => new UIWindow(h)
			}
		};

		private static readonly Dictionary<IntPtr, object> _objects = new Dictionary<IntPtr, object>();

		public static void RegisterType<T>(Func<IntPtr, object> constructor) where T : NSObject
		{
			lock (_contructorLock)
			{
				_constructors[typeof(T)] = constructor;
			}
		}

		public static T GetNSObject<T>(IntPtr handle) where T : NSObject
		{
			if (handle == IntPtr.Zero)
			{
				return null;
			}
			lock (_objectLock)
			{
				if (_objects.TryGetValue(handle, out object value))
				{
					return (T)value;
				}
			}
			return ConstructNSObject<T>(handle);
		}

		private static T ConstructNSObject<T>(IntPtr handle) where T : NSObject
		{
			Func<IntPtr, object> value;
			lock (_contructorLock)
			{
				if (!_constructors.TryGetValue(typeof(T), out value))
				{
					throw new NotImplementedException("Type's constructor not registered: " + typeof(T));
				}
			}
			return (T)value(handle);
		}

		public static void RegisterNSObject(NSObject obj)
		{
			if (obj != null && !(obj.Handle == IntPtr.Zero))
			{
				lock (_objectLock)
				{
					_objects[obj.Handle] = obj;
				}
			}
		}

		public static void UnregisterNSObject(IntPtr handle)
		{
			if (!(handle == IntPtr.Zero))
			{
				lock (_objectLock)
				{
					_objects.Remove(handle);
				}
			}
		}
	}
}
