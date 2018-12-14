using System;
using System.Security.Cryptography;

namespace ThirdParty.MD5
{
	public class MD5Managed : HashAlgorithm
	{
		private byte[] _data;

		private ABCDStruct _abcd;

		private long _totalLength;

		private int _dataSize;

		public MD5Managed()
		{
			HashSizeValue = 128;
			Initialize();
		}

		public override void Initialize()
		{
			_data = new byte[64];
			_dataSize = 0;
			_totalLength = 0L;
			_abcd = default(ABCDStruct);
			_abcd.A = 1732584193u;
			_abcd.B = 4023233417u;
			_abcd.C = 2562383102u;
			_abcd.D = 271733878u;
		}

		protected override void HashCore(byte[] array, int ibStart, int cbSize)
		{
			int num = _dataSize + cbSize;
			if (num >= 64)
			{
				Array.Copy(array, ibStart, _data, _dataSize, 64 - _dataSize);
				MD5Core.GetHashBlock(_data, ref _abcd, 0);
				int num2 = ibStart + (64 - _dataSize);
				num -= 64;
				while (num >= 64)
				{
					Array.Copy(array, num2, _data, 0, 64);
					MD5Core.GetHashBlock(array, ref _abcd, num2);
					num -= 64;
					num2 += 64;
				}
				_dataSize = num;
				Array.Copy(array, num2, _data, 0, num);
			}
			else
			{
				Array.Copy(array, ibStart, _data, _dataSize, cbSize);
				_dataSize = num;
			}
			_totalLength += cbSize;
		}

		protected override byte[] HashFinal()
		{
			HashValue = MD5Core.GetHashFinalBlock(_data, 0, _dataSize, _abcd, _totalLength * 8);
			return HashValue;
		}
	}
}
