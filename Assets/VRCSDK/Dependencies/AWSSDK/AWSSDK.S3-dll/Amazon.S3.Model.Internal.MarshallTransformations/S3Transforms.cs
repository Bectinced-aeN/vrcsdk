using Amazon.Runtime.Internal;
using Amazon.S3.Util;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public static class S3Transforms
	{
		internal static string ToURLEncodedValue(string value, bool path)
		{
			if (value == null)
			{
				return string.Empty;
			}
			return AmazonS3Util.UrlEncode(value, path);
		}

		internal static string ToURLEncodedValue(int value, bool path)
		{
			return ToStringValue(value);
		}

		internal static string ToURLEncodedValue(DateTime value, bool path)
		{
			return ToStringValue(value);
		}

		internal static string ToStringValue(string value)
		{
			return value ?? "";
		}

		internal static string ToStringValue(int value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}

		internal static string ToStringValue(DateTime value)
		{
			return value.ToString("ddd, dd MMM yyyy HH:mm:ss \\G\\M\\T", CultureInfo.InvariantCulture);
		}

		internal static string ToXmlStringValue(string value)
		{
			return ToStringValue(value);
		}

		internal static string ToXmlStringValue(DateTime value)
		{
			return value.ToString("yyyy-MM-dd\\THH:mm:ss.fff\\Z", CultureInfo.InvariantCulture);
		}

		internal static string ToXmlStringValue(int value)
		{
			return ToStringValue(value);
		}

		internal static DateTime ToDateTime(string value)
		{
			return ((DateTime)Convert.ChangeType(value, typeof(DateTime), CultureInfo.InvariantCulture)).ToUniversalTime();
		}

		internal static int ToInt(string value)
		{
			return Unmarshall<int>(value);
		}

		internal static string ToString(string value)
		{
			return value;
		}

		internal static T Unmarshall<T>(string text)
		{
			return (T)Convert.ChangeType(text, typeof(T), CultureInfo.InvariantCulture);
		}

		internal static void BuildQueryParameterMap(IRequest request, IDictionary<string, string> queryParameters, string queryString, params string[] unusedIfNullValueParams)
		{
			HashSet<string> hashSet = new HashSet<string>(unusedIfNullValueParams);
			int num = 0;
			int num2 = queryString.IndexOfAny(new char[2]
			{
				'&',
				';'
			}, 0);
			if (num2 == -1 && num < queryString.Length)
			{
				num2 = queryString.Length;
			}
			while (num2 != -1)
			{
				string text = queryString.Substring(num, num2 - num);
				if (num2 + 1 >= queryString.Length || queryString[num2 + 1] != ' ')
				{
					int num3 = text.IndexOf('=');
					string[] array = (num3 != -1) ? new string[2]
					{
						text.Substring(0, num3),
						text.Substring(num3 + 1)
					} : new string[1]
					{
						text
					};
					if (array.Length == 2 && array[1].Length > 0)
					{
						request.get_Parameters().Add(array[0], array[1]);
					}
					else if (!hashSet.Contains(array[0]))
					{
						request.get_Parameters().Add(array[0], null);
					}
					if (array.Length == 2 && array[1].Length > 0)
					{
						queryParameters.Add(array[0], array[1]);
					}
					else if (!hashSet.Contains(array[0]))
					{
						queryParameters.Add(array[0], null);
					}
					num = num2 + 1;
				}
				if (queryString.Length <= num2 + 1)
				{
					break;
				}
				num2 = queryString.IndexOfAny(new char[2]
				{
					'&',
					';'
				}, num2 + 1);
				if (num2 == -1 && num < queryString.Length)
				{
					num2 = queryString.Length;
				}
			}
		}
	}
}
