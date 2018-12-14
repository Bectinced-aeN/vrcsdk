using System;

namespace Org.BouncyCastle.Crypto.Parameters
{
	internal class DesEdeParameters : DesParameters
	{
		public const int DesEdeKeyLength = 24;

		public DesEdeParameters(byte[] key)
			: base(FixKey(key, 0, key.Length))
		{
		}

		public DesEdeParameters(byte[] key, int keyOff, int keyLen)
			: base(FixKey(key, keyOff, keyLen))
		{
		}

		private static byte[] FixKey(byte[] key, int keyOff, int keyLen)
		{
			byte[] array = new byte[24];
			switch (keyLen)
			{
			case 16:
				Array.Copy(key, keyOff, array, 0, 16);
				Array.Copy(key, keyOff, array, 16, 8);
				break;
			case 24:
				Array.Copy(key, keyOff, array, 0, 24);
				break;
			default:
				throw new ArgumentException("Bad length for DESede key: " + keyLen, "keyLen");
			}
			if (IsWeakKey(array))
			{
				throw new ArgumentException("attempt to create weak DESede key");
			}
			return array;
		}

		public static bool IsWeakKey(byte[] key, int offset, int length)
		{
			for (int i = offset; i < length; i += 8)
			{
				if (DesParameters.IsWeakKey(key, i))
				{
					return true;
				}
			}
			return false;
		}

		public new static bool IsWeakKey(byte[] key, int offset)
		{
			return IsWeakKey(key, offset, key.Length - offset);
		}

		public new static bool IsWeakKey(byte[] key)
		{
			return IsWeakKey(key, 0, key.Length);
		}
	}
}
