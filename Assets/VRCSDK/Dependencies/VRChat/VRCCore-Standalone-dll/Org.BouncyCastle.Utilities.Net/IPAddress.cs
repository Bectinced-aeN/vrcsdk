using System;
using System.Globalization;

namespace Org.BouncyCastle.Utilities.Net
{
	internal static class IPAddress
	{
		public static bool IsValid(string address)
		{
			return IsValidIPv4(address) || IsValidIPv6(address);
		}

		public static bool IsValidWithNetMask(string address)
		{
			return IsValidIPv4WithNetmask(address) || IsValidIPv6WithNetmask(address);
		}

		public static bool IsValidIPv4(string address)
		{
			try
			{
				return unsafeIsValidIPv4(address);
				IL_000c:;
			}
			catch (FormatException)
			{
			}
			catch (OverflowException)
			{
			}
			return false;
		}

		private static bool unsafeIsValidIPv4(string address)
		{
			if (address.Length == 0)
			{
				return false;
			}
			int num = 0;
			string text = address + ".";
			int num2 = 0;
			int num3;
			while (num2 < text.Length && (num3 = text.IndexOf('.', num2)) > num2)
			{
				if (num == 4)
				{
					return false;
				}
				string s = text.Substring(num2, num3 - num2);
				int num4 = int.Parse(s);
				if (num4 < 0 || num4 > 255)
				{
					return false;
				}
				num2 = num3 + 1;
				num++;
			}
			return num == 4;
		}

		public static bool IsValidIPv4WithNetmask(string address)
		{
			int num = address.IndexOf("/");
			string text = address.Substring(num + 1);
			return num > 0 && IsValidIPv4(address.Substring(0, num)) && (IsValidIPv4(text) || IsMaskValue(text, 32));
		}

		public static bool IsValidIPv6WithNetmask(string address)
		{
			int num = address.IndexOf("/");
			string text = address.Substring(num + 1);
			return num > 0 && IsValidIPv6(address.Substring(0, num)) && (IsValidIPv6(text) || IsMaskValue(text, 128));
		}

		private static bool IsMaskValue(string component, int size)
		{
			int num = int.Parse(component);
			try
			{
				return num >= 0 && num <= size;
				IL_001e:;
			}
			catch (FormatException)
			{
			}
			catch (OverflowException)
			{
			}
			return false;
		}

		public static bool IsValidIPv6(string address)
		{
			try
			{
				return unsafeIsValidIPv6(address);
				IL_000c:;
			}
			catch (FormatException)
			{
			}
			catch (OverflowException)
			{
			}
			return false;
		}

		private static bool unsafeIsValidIPv6(string address)
		{
			if (address.Length == 0)
			{
				return false;
			}
			int num = 0;
			string text = address + ":";
			bool flag = false;
			int num2 = 0;
			int num3;
			while (num2 < text.Length && (num3 = text.IndexOf(':', num2)) >= num2)
			{
				if (num == 8)
				{
					return false;
				}
				if (num2 != num3)
				{
					string text2 = text.Substring(num2, num3 - num2);
					if (num3 == text.Length - 1 && text2.IndexOf('.') > 0)
					{
						if (!IsValidIPv4(text2))
						{
							return false;
						}
						num++;
					}
					else
					{
						string s = text.Substring(num2, num3 - num2);
						int num4 = int.Parse(s, NumberStyles.AllowHexSpecifier);
						if (num4 < 0 || num4 > 65535)
						{
							return false;
						}
					}
				}
				else
				{
					if (num3 != 1 && num3 != text.Length - 1 && flag)
					{
						return false;
					}
					flag = true;
				}
				num2 = num3 + 1;
				num++;
			}
			return num == 8 || flag;
		}
	}
}
