using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ThirdParty.iOS4Unity
{
	public static class ObjC
	{
		public static class Libraries
		{
			public static readonly IntPtr Foundation = dlopen("/System/Library/Frameworks/Foundation.framework/Foundation", 0);

			public static readonly IntPtr UIKit = dlopen("/System/Library/Frameworks/UIKit.framework/UIKit", 0);
		}

		private const string timeIntervalSinceReferenceDate = "timeIntervalSinceReferenceDate";

		internal static readonly Selector TimeIntervalSinceReferenceDateSelector = new Selector("timeIntervalSinceReferenceDate");

		private const string absoluteString = "absoluteString";

		internal static readonly Selector AbsoluteStringSelector = new Selector("absoluteString");

		private const string doubleValue = "doubleValue";

		internal static readonly Selector DoubleValueSelector = new Selector("doubleValue");

		private const string count = "count";

		internal static readonly Selector CountSelector = new Selector("count");

		private const string objectAtIndex = "objectAtIndex:";

		internal static readonly Selector ObjectAtIndexSelector = new Selector("objectAtIndex:");

		private const string allObjects = "allObjects";

		internal static readonly Selector AllObjectsSelector = new Selector("allObjects");

		private const string arrayWithObjects_count = "arrayWithObjects:count:";

		internal static readonly Selector ArrayWithObjects_CountSelector = new Selector("arrayWithObjects:count:");

		private const string setWithArray = "setWithArray";

		internal static readonly Selector SetWithArraySelector = new Selector("setWithArray");

		private const string initWithCharacters_length = "initWithCharacters:length:";

		internal static readonly Selector InitWithCharacters_lengthSelector = new Selector("initWithCharacters:length:");

		private const string URLWithString = "URLWithString:";

		internal static readonly Selector URLWithStringSelector = new Selector("URLWithString:");

		private const string dateWithTimeIntervalSinceReferenceDate = "dateWithTimeIntervalSinceReferenceDate:";

		internal static readonly Selector DateWithTimeIntervalSinceReferenceDateSelector = new Selector("dateWithTimeIntervalSinceReferenceDate:");

		private const string initWithDouble = "initWithDouble:";

		internal static readonly Selector InitWithDoubleSelector = new Selector("initWithDouble:");

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "sel_registerName")]
		public static extern IntPtr GetSelector(string name);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "sel_getName")]
		public static extern string GetSelectorName(IntPtr selector);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_getClass")]
		public static extern IntPtr GetClass(string name);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_allocateClassPair")]
		public static extern IntPtr AllocateClassPair(IntPtr superclass, string name, int extraBytes);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "class_addMethod")]
		public static extern bool AddMethod(IntPtr cls, IntPtr selector, Delegate imp, string types);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern void MessageSend(IntPtr receiver, IntPtr selector, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "ThirdParty.iOS4Unity.Marshalers.NSDateReleaseMarshaler")] object arg1);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern void MessageSend(IntPtr receiver, IntPtr selector);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern void MessageSend(IntPtr receiver, IntPtr selector, bool arg1);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern void MessageSend(IntPtr receiver, IntPtr selector, CGSize arg1);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern void MessageSend(IntPtr receiver, IntPtr selector, IntPtr arg1);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern void MessageSend(IntPtr receiver, IntPtr selector, CGRect arg1);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern void MessageSend(IntPtr receiver, IntPtr selector, CGPoint arg1);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern void MessageSend(IntPtr receiver, IntPtr selector, int arg1);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern void MessageSend(IntPtr receiver, IntPtr selector, float arg1);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern void MessageSend(IntPtr receiver, IntPtr selector, [MarshalAs(UnmanagedType.LPStr)] string arg1);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern void MessageSend(IntPtr receiver, IntPtr selector, [MarshalAs(UnmanagedType.LPStr)] string arg1, IntPtr arg2);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern void MessageSend(IntPtr receiver, IntPtr selector, [MarshalAs(UnmanagedType.LPStr)] string arg1, int arg2);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern void MessageSend(IntPtr receiver, IntPtr selector, [MarshalAs(UnmanagedType.LPStr)] string arg1, int arg2, IntPtr arg3);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern void MessageSend(IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2, double arg3);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern void MessageSend(IntPtr receiver, IntPtr selector, IntPtr arg1, bool arg2, IntPtr arg3);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern void MessageSend(IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2, [MarshalAs(UnmanagedType.LPStr)] string arg3, IntPtr arg4);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern void MessageSend(IntPtr receiver, IntPtr selector, CGRect arg1, IntPtr arg2, uint arg3, bool arg4);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern void MessageSend(IntPtr receiver, IntPtr selector, IntPtr arg1, bool arg2);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern void MessageSend(IntPtr receiver, IntPtr selector, int arg1, bool arg2);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern void MessageSend(IntPtr receiver, IntPtr selector, bool arg1, bool arg2);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern void MessageSend(IntPtr receiver, IntPtr selector, bool arg1, IntPtr arg2);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		[CLSCompliant(false)]
		public static extern uint MessageSendUInt(IntPtr receiver, IntPtr selector);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern int MessageSendInt(IntPtr receiver, IntPtr selector);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern int MessageSendInt(IntPtr receiver, IntPtr selector, [MarshalAs(UnmanagedType.LPStr)] string arg1);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern int MessageSendInt(IntPtr receiver, IntPtr selector, [MarshalAs(UnmanagedType.LPStr)] object arg1);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern IntPtr MessageSendIntPtr(IntPtr receiver, IntPtr selector);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern IntPtr MessageSendIntPtr(IntPtr receiver, IntPtr selector, [MarshalAs(UnmanagedType.LPStr)] string arg1);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern IntPtr MessageSendIntPtr(IntPtr receiver, IntPtr selector, CGRect arg1);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern IntPtr MessageSendIntPtr(IntPtr receiver, IntPtr selector, CGSize arg1, bool arg2);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern IntPtr MessageSendIntPtr(IntPtr receiver, IntPtr selector, double arg1);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern IntPtr MessageSendIntPtr(IntPtr receiver, IntPtr selector, int arg1);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern IntPtr MessageSendIntPtr(IntPtr receiver, IntPtr selector, int arg1, bool arg2);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern IntPtr MessageSendIntPtr(IntPtr receiver, IntPtr selector, double arg1, int arg2);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		[CLSCompliant(false)]
		public static extern IntPtr MessageSendIntPtr(IntPtr receiver, IntPtr selector, uint arg1);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern IntPtr MessageSendIntPtr(IntPtr receiver, IntPtr selector, IntPtr arg1);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern IntPtr MessageSendIntPtr(IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		[CLSCompliant(false)]
		public static extern IntPtr MessageSendIntPtr(IntPtr receiver, IntPtr selector, uint arg1, IntPtr arg2);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern IntPtr MessageSendIntPtr(IntPtr receiver, IntPtr selector, IntPtr arg1, int arg2);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern IntPtr MessageSendIntPtr(IntPtr receiver, IntPtr selector, IntPtr arg1, float arg2);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		[CLSCompliant(false)]
		public static extern IntPtr MessageSendIntPtr(IntPtr receiver, IntPtr selector, IntPtr arg1, uint arg2);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		[CLSCompliant(false)]
		public static extern IntPtr MessageSendIntPtr(IntPtr receiver, IntPtr selector, IntPtr arg1, uint arg2, bool arg3);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern IntPtr MessageSendIntPtr(IntPtr receiver, IntPtr selector, [MarshalAs(UnmanagedType.LPStr)] string arg1, IntPtr arg2);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern IntPtr MessageSendIntPtr(IntPtr receiver, IntPtr selector, [MarshalAs(UnmanagedType.LPStr)] string arg1, uint arg2, out IntPtr arg3);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern bool MessageSendBool(IntPtr receiver, IntPtr selector);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern bool MessageSendBool(IntPtr receiver, IntPtr selector, bool arg1);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern bool MessageSendBool(IntPtr receiver, IntPtr selector, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "ThirdParty.iOS4Unity.Marshalers.NSDateReleaseMarshaler")] object arg1);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern bool MessageSendBool(IntPtr receiver, IntPtr selector, [MarshalAs(UnmanagedType.LPStr)] string arg1);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern bool MessageSendBool(IntPtr receiver, IntPtr selector, [MarshalAs(UnmanagedType.LPStr)] string arg1, bool arg2);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern double MessageSendDouble(IntPtr receiver, IntPtr selector);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern double MessageSendDouble(IntPtr receiver, IntPtr selector, [MarshalAs(UnmanagedType.LPStr)] object arg1);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern float MessageSendFloat(IntPtr receiver, IntPtr selector);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern float MessageSendFloat(IntPtr receiver, IntPtr selector, float arg1);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "ThirdParty.iOS4Unity.Marshalers.NSDateMarshaler")]
		public static extern object MessageSendDate(IntPtr receiver, IntPtr selector);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "ThirdParty.iOS4Unity.Marshalers.NSDateMarshaler")]
		public static extern object MessageSendDate(IntPtr receiver, IntPtr selector, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "ThirdParty.iOS4Unity.Marshalers.NSDateReleaseMarshaler")] object arg1);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		[return: MarshalAs(UnmanagedType.BStr)]
		public static extern string MessageSendString(IntPtr receiver, IntPtr selector, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "ThirdParty.iOS4Unity.Marshalers.NSDateReleaseMarshaler")] object arg1);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		[return: MarshalAs(UnmanagedType.BStr)]
		public static extern string MessageSendString(IntPtr receiver, IntPtr selector);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		[return: MarshalAs(UnmanagedType.BStr)]
		public static extern string MessageSendString(IntPtr receiver, IntPtr selector, IntPtr arg1);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		[return: MarshalAs(UnmanagedType.BStr)]
		public static extern string MessageSendString(IntPtr receiver, IntPtr selector, int arg1);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		[return: MarshalAs(UnmanagedType.BStr)]
		public static extern string MessageSendString(IntPtr receiver, IntPtr selector, double arg1);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		[return: MarshalAs(UnmanagedType.BStr)]
		public static extern string MessageSendString(IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		[return: MarshalAs(UnmanagedType.BStr)]
		public static extern string MessageSendString(IntPtr receiver, IntPtr selector, IntPtr arg1, int arg2);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		[return: MarshalAs(UnmanagedType.BStr)]
		public static extern string MessageSendString(IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2, IntPtr arg3);

		public static string MessageSendString(IntPtr receiver, IntPtr selector, string arg1)
		{
			return MessageSendString(receiver, selector, ToNSString(arg1));
		}

		public static string MessageSendString(IntPtr receiver, IntPtr selector, string arg1, string arg2, string arg3)
		{
			return MessageSendString(receiver, selector, ToNSString(arg1), ToNSString(arg2), ToNSString(arg3));
		}

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern IntPtr MessageSendIntPtr_NSUrl(IntPtr receiver, IntPtr selector, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "ThirdParty.iOS4Unity.NSUrlMarshaler")] string arg1);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		[CLSCompliant(false)]
		public static extern IntPtr MessageSendIntPtr_NSUrl(IntPtr receiver, IntPtr selector, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "ThirdParty.iOS4Unity.NSUrlMarshaler")] string arg1, uint arg2, out IntPtr arg3);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern bool MessageSendBool_NSUrl(IntPtr receiver, IntPtr selector, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "ThirdParty.iOS4Unity.NSUrlMarshaler")] string arg1);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend_stret")]
		public static extern CGRect MessageSendCGRect(IntPtr receiver, IntPtr selector);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend_stret")]
		[CLSCompliant(false)]
		public static extern CGPoint _MessageSendStretCGPoint(IntPtr receiver, IntPtr selector);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend_stret")]
		[CLSCompliant(false)]
		public static extern CGSize _MessageSendStretCGSize(IntPtr receiver, IntPtr selector);

		public static CGSize MessageSendCGSize(IntPtr receiver, string selector)
		{
			Selector selector2 = new Selector(selector);
			return _MessageSendStretCGSize(receiver, selector2.Handle);
		}

		public static CGPoint MessageSendCGPoint(IntPtr receiver, string selector)
		{
			Selector selector2 = new Selector(selector);
			return _MessageSendStretCGPoint(receiver, selector2.Handle);
		}

		[DllImport("/usr/lib/libSystem.dylib")]
		private static extern IntPtr dlsym(IntPtr handle, string symbol);

		[DllImport("/usr/lib/libSystem.dylib")]
		private static extern IntPtr dlopen(string path, int mode);

		public static string FromNSString(IntPtr handle)
		{
			if (handle == IntPtr.Zero)
			{
				return null;
			}
			return Marshal.PtrToStringAuto(MessageSendIntPtr(handle, Selector.UTF8StringHandle));
		}

		public static DateTime FromNSDate(IntPtr handle)
		{
			if (handle == IntPtr.Zero)
			{
				return default(DateTime);
			}
			double num = MessageSendDouble(handle, TimeIntervalSinceReferenceDateSelector.Handle);
			if (num < -63113904000.0)
			{
				return DateTime.MinValue;
			}
			if (num > 252423993599.0)
			{
				return DateTime.MaxValue;
			}
			return new DateTime((long)(num * 10000000.0 + 6.3113904E+17), DateTimeKind.Utc);
		}

		public static string FromNSUrl(IntPtr handle)
		{
			if (handle == IntPtr.Zero)
			{
				return null;
			}
			return FromNSString(MessageSendIntPtr(handle, AbsoluteStringSelector.Handle));
		}

		public static double FromNSNumber(IntPtr handle)
		{
			if (handle == IntPtr.Zero)
			{
				return 0.0;
			}
			return MessageSendDouble(handle, DoubleValueSelector.Handle);
		}

		public static string[] FromNSArray(IntPtr handle)
		{
			if (handle == IntPtr.Zero)
			{
				return null;
			}
			uint num = MessageSendUInt(handle, CountSelector.Handle);
			string[] array = new string[num];
			for (uint num2 = 0u; num2 < num; num2++)
			{
				IntPtr handle2 = MessageSendIntPtr(handle, ObjectAtIndexSelector.Handle, num2);
				array[num2] = FromNSString(handle2);
			}
			return array;
		}

		public static string[] FromNSSet(IntPtr handle)
		{
			if (handle == IntPtr.Zero)
			{
				return null;
			}
			return FromNSArray(MessageSendIntPtr(handle, AllObjectsSelector.Handle));
		}

		public static T[] FromNSArray<T>(IntPtr handle) where T : NSObject
		{
			if (handle == IntPtr.Zero)
			{
				return null;
			}
			uint num = MessageSendUInt(handle, CountSelector.Handle);
			T[] array = new T[num];
			for (uint num2 = 0u; num2 < num; num2++)
			{
				IntPtr handle2 = MessageSendIntPtr(handle, ObjectAtIndexSelector.Handle, num2);
				array[num2] = Runtime.GetNSObject<T>(handle2);
			}
			return array;
		}

		public static void ReleaseNSArrayItems(IntPtr handle)
		{
			uint num = MessageSendUInt(handle, CountSelector.Handle);
			for (uint num2 = 0u; num2 < num; num2++)
			{
				MessageSend(MessageSendIntPtr(handle, ObjectAtIndexSelector.Handle, num2), Selector.ReleaseHandle);
			}
		}

		public static T[] FromNSSet<T>(IntPtr handle) where T : NSObject
		{
			if (handle == IntPtr.Zero)
			{
				return null;
			}
			return FromNSArray<T>(MessageSendIntPtr(handle, AllObjectsSelector.Handle));
		}

		public static string GetStringConstant(IntPtr handle, string symbol)
		{
			IntPtr intPtr = dlsym(handle, symbol);
			if (intPtr == IntPtr.Zero)
			{
				return null;
			}
			intPtr = Marshal.ReadIntPtr(intPtr);
			if (intPtr == IntPtr.Zero)
			{
				return null;
			}
			return FromNSString(intPtr);
		}

		public unsafe static float GetFloatConstant(IntPtr handle, string symbol)
		{
			IntPtr intPtr = dlsym(handle, symbol);
			if (intPtr == IntPtr.Zero)
			{
				return 0f;
			}
			float* ptr = (float*)(void*)intPtr;
			return *ptr;
		}

		public static IntPtr ToNSArray(IntPtr[] items)
		{
			IntPtr intPtr = Marshal.AllocHGlobal((IntPtr)(items.Length * IntPtr.Size));
			for (int i = 0; i < items.Length; i++)
			{
				Marshal.WriteIntPtr(intPtr, i * IntPtr.Size, items[i]);
			}
			IntPtr result = MessageSendIntPtr(GetClass("NSArray"), ArrayWithObjects_CountSelector.Handle, intPtr, items.Length);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public static IntPtr ToNSArray(NSObject[] items)
		{
			return ToNSArray((from i in items
			select i.Handle).ToArray());
		}

		public static IntPtr ToNSArray(string[] items)
		{
			return ToNSArray((from s in items
			select ToNSString(s)).ToArray());
		}

		public static IntPtr ToNSSet(IntPtr[] items)
		{
			IntPtr arg = ToNSArray(items);
			return MessageSendIntPtr(GetClass("NSSet"), SetWithArraySelector.Handle, arg);
		}

		public static IntPtr ToNSSet(string[] items)
		{
			IntPtr[] array = new IntPtr[items.Length];
			for (int i = 0; i < items.Length; i++)
			{
				array[i] = ToNSString(items[i]);
			}
			IntPtr arg = ToNSArray(array);
			IntPtr result = MessageSendIntPtr(GetClass("NSSet"), SetWithArraySelector.Handle, arg);
			for (int j = 0; j < array.Length; j++)
			{
				MessageSend(array[j], Selector.ReleaseHandle);
			}
			return result;
		}

		public unsafe static IntPtr ToNSString(string str)
		{
			IntPtr receiver = MessageSendIntPtr(GetClass("NSString"), Selector.AllocHandle);
			fixed (char* value = str + ((IntPtr)(RuntimeHelpers.OffsetToStringData / 2)).ToString())
			{
				return MessageSendIntPtr(receiver, InitWithCharacters_lengthSelector.Handle, (IntPtr)(void*)value, str.Length);
			}
		}

		public static IntPtr ToNSUrl(string str)
		{
			return MessageSendIntPtr(GetClass("NSURL"), URLWithStringSelector.Handle, str);
		}

		public static IntPtr ToNSDate(DateTime date)
		{
			return MessageSendIntPtr(GetClass("NSDate"), DateWithTimeIntervalSinceReferenceDateSelector.Handle, (double)((date.Ticks - 631139040000000000L) / 10000000));
		}

		public static IntPtr ToNSNumber(double value)
		{
			return MessageSendIntPtr(MessageSendIntPtr(GetClass("NSDecimalNumber"), Selector.AllocHandle), InitWithDoubleSelector.Handle, value);
		}

		[DllImport("/System/Library/Frameworks/Foundation.framework/Foundation")]
		public static extern void NSLog(IntPtr format, IntPtr args);

		public static void NSLog(string format, params object[] args)
		{
			IntPtr format2 = ToNSString("%@");
			IntPtr args2 = ToNSString(string.Format(format, args));
			NSLog(format2, args2);
		}
	}
}
