using AOT;
using System;
using System.Collections.Generic;

namespace ThirdParty.iOS4Unity
{
	public static class Callbacks
	{
		private class Methods
		{
			public Action Action;

			public IntPtrHandler ActionIntPtr;

			public IntPtrHandler2 ActionIntPtrIntPtr;

			public IntPtrHandler3 ActionIntPtrIntPtrIntPtr;

			public EventHandler EventHandler;

			public EventHandler<ButtonEventArgs> EventHandlerInt;

			public readonly object Object;

			public Methods(object obj)
			{
				Object = obj;
			}
		}

		private static readonly Dictionary<string, Delegate> _delegates = new Dictionary<string, Delegate>();

		private static readonly Dictionary<IntPtr, Dictionary<IntPtr, Methods>> _callbacks = new Dictionary<IntPtr, Dictionary<IntPtr, Methods>>();

		private static Methods GetMethods(NSObject obj, string selector)
		{
			if (!_callbacks.TryGetValue(obj.Handle, out Dictionary<IntPtr, Methods> value))
			{
				value = (_callbacks[obj.Handle] = new Dictionary<IntPtr, Methods>());
			}
			IntPtr selector2 = ObjC.GetSelector(selector);
			if (!value.TryGetValue(selector2, out Methods value2))
			{
				value2 = (value[selector2] = new Methods(obj));
			}
			return value2;
		}

		public static void Subscribe(NSObject obj, string selector, IntPtrHandler callback)
		{
			Methods methods = GetMethods(obj, selector);
			methods.ActionIntPtr = (IntPtrHandler)Delegate.Combine(methods.ActionIntPtr, callback);
			if (!_delegates.ContainsKey(selector))
			{
				IntPtrHandler3 intPtrHandler = OnCallback;
				if (!ObjC.AddMethod(obj.ClassHandle, Selector.GetHandle(selector), intPtrHandler, "v@:@"))
				{
					throw new InvalidOperationException("AddMethod failed for selector " + selector);
				}
				_delegates[selector] = intPtrHandler;
			}
		}

		public static void Subscribe(NSObject obj, string selector, EventHandler callback)
		{
			Methods methods = GetMethods(obj, selector);
			methods.EventHandler = (EventHandler)Delegate.Combine(methods.EventHandler, callback);
			if (!_delegates.ContainsKey(selector))
			{
				IntPtrHandler3 intPtrHandler = OnCallback;
				if (!ObjC.AddMethod(obj.ClassHandle, Selector.GetHandle(selector), intPtrHandler, "v@:@"))
				{
					throw new InvalidOperationException("AddMethod failed for selector " + selector);
				}
				_delegates[selector] = intPtrHandler;
			}
		}

		public static void Subscribe(NSObject obj, string selector, EventHandler<ButtonEventArgs> callback)
		{
			Methods methods = GetMethods(obj, selector);
			methods.EventHandlerInt = (EventHandler<ButtonEventArgs>)Delegate.Combine(methods.EventHandlerInt, callback);
			if (!_delegates.ContainsKey(selector))
			{
				Action<IntPtr, IntPtr, IntPtr, int> action = OnCallbackInt;
				if (!ObjC.AddMethod(obj.ClassHandle, Selector.GetHandle(selector), action, "v@:@l"))
				{
					throw new InvalidOperationException("AddMethod failed for selector " + selector);
				}
				_delegates[selector] = action;
			}
		}

		public static void Subscribe(NSObject obj, string selector, IntPtrHandler2 callback)
		{
			Methods methods = GetMethods(obj, selector);
			methods.ActionIntPtrIntPtr = (IntPtrHandler2)Delegate.Combine(methods.ActionIntPtrIntPtr, callback);
			if (!_delegates.ContainsKey(selector))
			{
				IntPtrHandler4 intPtrHandler = OnCallbackIntPtrIntPtr;
				if (!ObjC.AddMethod(obj.ClassHandle, Selector.GetHandle(selector), intPtrHandler, "v@:@@"))
				{
					throw new InvalidOperationException("AddMethod failed for selector " + selector);
				}
				_delegates[selector] = intPtrHandler;
			}
		}

		public static void Subscribe(NSObject obj, string selector, IntPtrHandler3 callback)
		{
			Methods methods = GetMethods(obj, selector);
			methods.ActionIntPtrIntPtrIntPtr = (IntPtrHandler3)Delegate.Combine(methods.ActionIntPtrIntPtrIntPtr, callback);
			if (!_delegates.ContainsKey(selector))
			{
				IntPtrHandler5 intPtrHandler = OnCallbackIntPtrIntPtrIntPtr;
				if (!ObjC.AddMethod(obj.ClassHandle, Selector.GetHandle(selector), intPtrHandler, "v@:@@@"))
				{
					throw new InvalidOperationException("AddMethod failed for selector " + selector);
				}
				_delegates[selector] = intPtrHandler;
			}
		}

		public static void Unsubscribe(NSObject obj, string selector, Action callback)
		{
			Methods methods = GetMethods(obj, selector);
			methods.Action = (Action)Delegate.Remove(methods.Action, callback);
		}

		public static void Unsubscribe(NSObject obj, string selector, IntPtrHandler callback)
		{
			Methods methods = GetMethods(obj, selector);
			methods.ActionIntPtr = (IntPtrHandler)Delegate.Remove(methods.ActionIntPtr, callback);
		}

		public static void Unsubscribe(NSObject obj, string selector, IntPtrHandler2 callback)
		{
			Methods methods = GetMethods(obj, selector);
			methods.ActionIntPtrIntPtr = (IntPtrHandler2)Delegate.Remove(methods.ActionIntPtrIntPtr, callback);
		}

		public static void Unsubscribe(NSObject obj, string selector, IntPtrHandler3 callback)
		{
			Methods methods = GetMethods(obj, selector);
			methods.ActionIntPtrIntPtrIntPtr = (IntPtrHandler3)Delegate.Remove(methods.ActionIntPtrIntPtrIntPtr, callback);
		}

		public static void Unsubscribe(NSObject obj, string selector, EventHandler callback)
		{
			Methods methods = GetMethods(obj, selector);
			methods.EventHandler = (EventHandler)Delegate.Remove(methods.EventHandler, callback);
		}

		public static void Unsubscribe(NSObject obj, string selector, EventHandler<ButtonEventArgs> callback)
		{
			Methods methods = GetMethods(obj, selector);
			methods.EventHandlerInt = (EventHandler<ButtonEventArgs>)Delegate.Remove(methods.EventHandlerInt, callback);
		}

		public static void UnsubscribeAll(NSObject obj)
		{
			_callbacks.Remove(obj.Handle);
		}

		[MonoPInvokeCallback(typeof(IntPtrHandler3))]
		private static void OnCallback(IntPtr @this, IntPtr selector, IntPtr arg1)
		{
			if (_callbacks.TryGetValue(@this, out Dictionary<IntPtr, Methods> value) && value.TryGetValue(selector, out Methods value2))
			{
				value2.Action?.Invoke();
				value2.ActionIntPtr?.Invoke(arg1);
				value2.EventHandler?.Invoke(value2.Object, EventArgs.Empty);
			}
		}

		[MonoPInvokeCallback(typeof(Action<IntPtr, IntPtr, IntPtr, int>))]
		private static void OnCallbackInt(IntPtr @this, IntPtr selector, IntPtr arg1, int arg2)
		{
			if (_callbacks.TryGetValue(@this, out Dictionary<IntPtr, Methods> value) && value.TryGetValue(selector, out Methods value2))
			{
				value2.EventHandlerInt?.Invoke(value2.Object, new ButtonEventArgs
				{
					Index = arg2
				});
			}
		}

		[MonoPInvokeCallback(typeof(IntPtrHandler4))]
		private static void OnCallbackIntPtrIntPtr(IntPtr @this, IntPtr selector, IntPtr arg1, IntPtr arg2)
		{
			if (_callbacks.TryGetValue(@this, out Dictionary<IntPtr, Methods> value) && value.TryGetValue(selector, out Methods value2))
			{
				value2.ActionIntPtrIntPtr?.Invoke(arg1, arg2);
			}
		}

		[MonoPInvokeCallback(typeof(IntPtrHandler5))]
		private static void OnCallbackIntPtrIntPtrIntPtr(IntPtr @this, IntPtr selector, IntPtr arg1, IntPtr arg2, IntPtr arg3)
		{
			if (_callbacks.TryGetValue(@this, out Dictionary<IntPtr, Methods> value) && value.TryGetValue(selector, out Methods value2))
			{
				value2.ActionIntPtrIntPtrIntPtr?.Invoke(arg1, arg2, arg3);
			}
		}
	}
}
