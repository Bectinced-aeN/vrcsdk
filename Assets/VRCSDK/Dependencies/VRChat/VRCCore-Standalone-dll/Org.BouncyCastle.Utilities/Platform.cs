using System;
using System.Collections;
using System.Globalization;

namespace Org.BouncyCastle.Utilities
{
	internal abstract class Platform
	{
		internal static readonly string NewLine = GetNewLine();

		private static string GetNewLine()
		{
			return Environment.NewLine;
		}

		internal static int CompareIgnoreCase(string a, string b)
		{
			return string.Compare(a, b, ignoreCase: true);
		}

		internal static Exception CreateNotImplementedException(string message)
		{
			return new NotImplementedException(message);
		}

		internal static IList CreateArrayList()
		{
			return new ArrayList();
		}

		internal static IList CreateArrayList(int capacity)
		{
			return new ArrayList(capacity);
		}

		internal static IList CreateArrayList(ICollection collection)
		{
			return new ArrayList(collection);
		}

		internal static IList CreateArrayList(IEnumerable collection)
		{
			ArrayList arrayList = new ArrayList();
			foreach (object item in collection)
			{
				arrayList.Add(item);
			}
			return arrayList;
		}

		internal static IDictionary CreateHashtable()
		{
			return new Hashtable();
		}

		internal static IDictionary CreateHashtable(int capacity)
		{
			return new Hashtable(capacity);
		}

		internal static IDictionary CreateHashtable(IDictionary dictionary)
		{
			return new Hashtable(dictionary);
		}

		internal static string ToLowerInvariant(string s)
		{
			return s.ToLower(CultureInfo.InvariantCulture);
		}

		internal static string ToUpperInvariant(string s)
		{
			return s.ToUpper(CultureInfo.InvariantCulture);
		}
	}
}
