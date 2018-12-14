using System;

namespace ThirdParty.iOS4Unity
{
	public class NSLocale : NSObject
	{
		private static readonly IntPtr _classHandle;

		public override IntPtr ClassHandle => _classHandle;

		public string AlternateQuotationBeginDelimiter => ObjectForKey(ObjC.GetStringConstant(ObjC.Libraries.Foundation, "NSLocaleAlternateQuotationBeginDelimiterKey"));

		public string AlternateQuotationEndDelimiter => ObjectForKey(ObjC.GetStringConstant(ObjC.Libraries.Foundation, "NSLocaleAlternateQuotationEndDelimiterKey"));

		public static NSLocale AutoUpdatingCurrentLocale => Runtime.GetNSObject<NSLocale>(ObjC.MessageSendIntPtr(_classHandle, Selector.GetHandle("autoupdatingCurrentLocale")));

		public static string[] AvailableLocaleIdentifiers => ObjC.FromNSArray(ObjC.MessageSendIntPtr(_classHandle, Selector.GetHandle("availableLocaleIdentifiers")));

		public string CollationIdentifier => ObjectForKey(ObjC.GetStringConstant(ObjC.Libraries.Foundation, "NSLocaleCollationIdentifier"));

		public string CollatorIdentifier => ObjectForKey(ObjC.GetStringConstant(ObjC.Libraries.Foundation, "NSLocaleCollatorIdentifier"));

		public static string[] CommonISOCurrencyCodes => ObjC.FromNSArray(ObjC.MessageSendIntPtr(_classHandle, Selector.GetHandle("commonISOCurrencyCodes")));

		public string CountryCode => ObjectForKey(ObjC.GetStringConstant(ObjC.Libraries.Foundation, "NSLocaleCountryCode"));

		public string CurrencyCode => ObjectForKey(ObjC.GetStringConstant(ObjC.Libraries.Foundation, "NSLocaleCurrencyCode"));

		public string CurrencySymbol => ObjectForKey(ObjC.GetStringConstant(ObjC.Libraries.Foundation, "NSLocaleCurrencySymbol"));

		public static NSLocale CurrentLocale => Runtime.GetNSObject<NSLocale>(ObjC.MessageSendIntPtr(_classHandle, Selector.GetHandle("currentLocale")));

		public static string CurrentLocaleDidChangeNotification => ObjC.GetStringConstant(ObjC.Libraries.Foundation, "NSCurrentLocaleDidChangeNotification");

		public string DecimalSeparator => ObjectForKey(ObjC.GetStringConstant(ObjC.Libraries.Foundation, "NSLocaleDecimalSeparator"));

		public string GroupingSeparator => ObjectForKey(ObjC.GetStringConstant(ObjC.Libraries.Foundation, "NSLocaleGroupingSeparator"));

		public string Identifier => ObjectForKey(ObjC.GetStringConstant(ObjC.Libraries.Foundation, "NSLocaleIdentifier"));

		public static string[] ISOCountryCodes => ObjC.FromNSArray(ObjC.MessageSendIntPtr(_classHandle, Selector.GetHandle("ISOCountryCodes")));

		public static string[] ISOCurrencyCodes => ObjC.FromNSArray(ObjC.MessageSendIntPtr(_classHandle, Selector.GetHandle("ISOCurrencyCodes")));

		public static string[] ISOLanguageCodes => ObjC.FromNSArray(ObjC.MessageSendIntPtr(_classHandle, Selector.GetHandle("ISOLanguageCodes")));

		public string LanguageCode => ObjectForKey(ObjC.GetStringConstant(ObjC.Libraries.Foundation, "NSLocaleLanguageCode"));

		public string LocaleIdentifier => ObjC.GetStringConstant(Handle, "localeIdentifier");

		public string MeasurementSystem => ObjectForKey(ObjC.GetStringConstant(ObjC.Libraries.Foundation, "NSLocaleMeasurementSystem"));

		public static string[] PreferredLanguages => ObjC.FromNSArray(ObjC.MessageSendIntPtr(_classHandle, Selector.GetHandle("preferredLanguages")));

		public string ScriptCode => ObjectForKey(ObjC.GetStringConstant(ObjC.Libraries.Foundation, "NSLocaleScriptCode"));

		public static NSLocale SystemLocale => Runtime.GetNSObject<NSLocale>(ObjC.MessageSendIntPtr(_classHandle, Selector.GetHandle("systemLocale")));

		public bool UsesMetricSystem => ObjectForKeyBool(ObjC.GetStringConstant(ObjC.Libraries.Foundation, "NSLocaleUsesMetricSystem"));

		static NSLocale()
		{
			_classHandle = ObjC.GetClass("NSLocale");
		}

		internal NSLocale(IntPtr handle)
			: base(handle)
		{
		}

		private string ObjectForKey(string key)
		{
			return ObjC.FromNSString(ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("objectForKey:"), ObjC.ToNSString(key)));
		}

		private bool ObjectForKeyBool(string key)
		{
			return ObjC.MessageSendBool(Handle, Selector.GetHandle("objectForKey:"), ObjC.ToNSString(key));
		}
	}
}
