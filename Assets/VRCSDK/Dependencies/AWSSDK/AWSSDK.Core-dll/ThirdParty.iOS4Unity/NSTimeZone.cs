using System;

namespace ThirdParty.iOS4Unity
{
	public class NSTimeZone : NSObject
	{
		private static readonly IntPtr _classHandle;

		public override IntPtr ClassHandle => _classHandle;

		public static string[] KnownTimeZoneNames => ObjC.FromNSArray(ObjC.MessageSendIntPtr(_classHandle, Selector.GetHandle("knownTimeZoneNames")));

		public static NSDictionary Abbreviations => Runtime.GetNSObject<NSDictionary>(ObjC.MessageSendIntPtr(_classHandle, Selector.GetHandle("abbreviationDictionary")));

		public NSData Data => Runtime.GetNSObject<NSData>(ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("data")));

		public static string DataVersion => ObjC.MessageSendString(_classHandle, Selector.GetHandle("timeZoneDataVersion"));

		public static NSTimeZone DefaultTimeZone
		{
			get
			{
				return Runtime.GetNSObject<NSTimeZone>(ObjC.MessageSendIntPtr(_classHandle, Selector.GetHandle("defaultTimeZone")));
			}
			set
			{
				ObjC.MessageSend(_classHandle, Selector.GetHandle("setDefaultTimeZone:"), value);
			}
		}

		public int GetSecondsFromGMT => ObjC.MessageSendInt(Handle, Selector.GetHandle("secondsFromGMT"));

		public static NSTimeZone LocalTimeZone => Runtime.GetNSObject<NSTimeZone>(ObjC.MessageSendIntPtr(_classHandle, Selector.GetHandle("localTimeZone")));

		public string Name => ObjC.MessageSendString(Handle, Selector.GetHandle("name"));

		public static NSTimeZone SystemTimeZone => Runtime.GetNSObject<NSTimeZone>(ObjC.MessageSendIntPtr(_classHandle, Selector.GetHandle("systemTimeZone")));

		static NSTimeZone()
		{
			_classHandle = ObjC.GetClass("NSTimeZone");
		}

		public NSTimeZone()
		{
			ObjC.MessageSendIntPtr(Handle, Selector.Init);
		}

		public NSTimeZone(string name)
		{
			Handle = ObjC.MessageSendIntPtr(Handle, Selector.InitWithName, name);
		}

		internal NSTimeZone(IntPtr handle)
			: base(handle)
		{
		}

		public string Abbreviation(DateTime date)
		{
			return ObjC.MessageSendString(Handle, Selector.GetHandle("abbreviationForDate:"), date);
		}

		public virtual string Abbreviation()
		{
			return ObjC.MessageSendString(Handle, Selector.GetHandle("abbreviation"));
		}

		public static NSTimeZone FromAbbreviation(string abbreviation)
		{
			return Runtime.GetNSObject<NSTimeZone>(ObjC.MessageSendIntPtr(_classHandle, Selector.GetHandle("timeZoneWithAbbreviation:"), abbreviation));
		}

		public static NSTimeZone FromName(string name, NSData data)
		{
			return Runtime.GetNSObject<NSTimeZone>(ObjC.MessageSendIntPtr(_classHandle, Selector.GetHandle("timeZoneWithName:data:"), name, data.Handle));
		}

		public static NSTimeZone FromName(string name)
		{
			return Runtime.GetNSObject<NSTimeZone>(ObjC.MessageSendIntPtr(_classHandle, Selector.GetHandle("timeZoneWithName:"), name));
		}

		public static void ResetSystemTimeZone()
		{
			ObjC.MessageSend(_classHandle, Selector.GetHandle("resetSystemTimeZone"));
		}

		public int SecondsFromGMT(DateTime date)
		{
			return ObjC.MessageSendInt(Handle, Selector.GetHandle("secondsFromGMTForDate:"), date);
		}
	}
}
