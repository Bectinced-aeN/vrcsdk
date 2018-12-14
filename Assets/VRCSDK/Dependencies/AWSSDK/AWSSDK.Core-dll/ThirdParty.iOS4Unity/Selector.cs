using System;
using System.Runtime.InteropServices;

namespace ThirdParty.iOS4Unity
{
	public class Selector
	{
		public static readonly IntPtr Init = GetHandle("init");

		public static readonly IntPtr InitWithCoder = GetHandle("initWithCoder:");

		public static readonly IntPtr InitWithName = GetHandle("initWithName:");

		public static readonly IntPtr InitWithFrame = GetHandle("initWithFrame:");

		private static IntPtr MethodSignatureForSelector = GetHandle("methodSignatureForSelector:");

		private static IntPtr FrameLength = GetHandle("frameLength");

		internal static IntPtr RetainCount = GetHandle("retainCount");

		internal const string Alloc = "alloc";

		internal const string Release = "release";

		internal const string Retain = "retain";

		internal const string Autorelease = "autorelease";

		internal const string DoesNotRecognizeSelector = "doesNotRecognizeSelector:";

		internal const string PerformSelectorOnMainThreadWithObjectWaitUntilDone = "performSelectorOnMainThread:withObject:waitUntilDone:";

		internal const string PerformSelectorWithObjectAfterDelay = "performSelector:withObject:afterDelay:";

		internal const string UTF8String = "UTF8String";

		internal static IntPtr AllocHandle = GetHandle("alloc");

		internal static IntPtr ReleaseHandle = GetHandle("release");

		internal static IntPtr RetainHandle = GetHandle("retain");

		internal static IntPtr AutoreleaseHandle = GetHandle("autorelease");

		internal static IntPtr DoesNotRecognizeSelectorHandle = GetHandle("doesNotRecognizeSelector:");

		internal static IntPtr PerformSelectorOnMainThreadWithObjectWaitUntilDoneHandle = GetHandle("performSelectorOnMainThread:withObject:waitUntilDone:");

		internal static IntPtr PerformSelectorWithObjectAfterDelayHandle = GetHandle("performSelector:withObject:afterDelay:");

		internal static IntPtr UTF8StringHandle = GetHandle("UTF8String");

		internal IntPtr handle;

		public IntPtr Handle => handle;

		public string Name => Marshal.PtrToStringAuto(sel_getName(handle));

		public Selector(IntPtr sel)
			: this(sel, check: true)
		{
		}

		internal Selector(IntPtr sel, bool check)
		{
			if (check && !sel_isMapped(sel))
			{
				throw new ArgumentException("sel is not a selector handle.");
			}
			handle = sel;
		}

		public Selector(string name, bool alloc)
		{
			handle = GetHandle(name);
		}

		public Selector(string name)
			: this(name, alloc: false)
		{
		}

		public static Selector Register(IntPtr handle)
		{
			return new Selector(handle);
		}

		public static Selector FromHandle(IntPtr sel)
		{
			if (!sel_isMapped(sel))
			{
				return null;
			}
			return new Selector(sel, check: false);
		}

		[DllImport("/usr/lib/libobjc.dylib")]
		private static extern IntPtr sel_getName(IntPtr sel);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "sel_registerName")]
		public static extern IntPtr GetHandle(string name);

		[DllImport("/usr/lib/libobjc.dylib")]
		private static extern bool sel_isMapped(IntPtr sel);
	}
}
