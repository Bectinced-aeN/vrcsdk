using Blake2Sharp;
using System;
using System.Collections.Generic;
using System.IO;

namespace librsync.net
{
	internal static class SignatureHelpers
	{
		public const int DefaultBlockLength = 2048;

		public const int DefaultStrongSumLength = 32;

		public static void WriteHeader(BinaryWriter s, SignatureJobSettings settings)
		{
			StreamHelpers.WriteBigEndian(s, (uint)settings.MagicNumber);
			StreamHelpers.WriteBigEndian(s, (uint)settings.BlockLength);
			StreamHelpers.WriteBigEndian(s, (uint)settings.StrongSumLength);
		}

		public static void WriteBlock(BinaryWriter s, byte[] block, SignatureJobSettings settings)
		{
			int num = CalculateWeakSum(block);
			if (settings.MagicNumber != MagicNumber.Blake2Signature)
			{
				throw new NotImplementedException("Non-blake2 hashes aren't supported");
			}
			byte[] buffer = CalculateBlake2StrongSum(block);
			StreamHelpers.WriteBigEndian(s, (ulong)num);
			s.Write(buffer, 0, settings.StrongSumLength);
		}

		private static int CalculateWeakSum(byte[] buf)
		{
			Rollsum rollsum = new Rollsum();
			rollsum.Update(buf);
			return rollsum.Digest;
		}

		private static byte[] CalculateBlake2StrongSum(byte[] block)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected O, but got Unknown
			Blake2BConfig val = new Blake2BConfig();
			val.set_OutputSizeInBytes(32);
			return Blake2B.ComputeHash(block, val);
		}

		public static SignatureFile ParseSignatureFile(Stream s)
		{
			SignatureFile signatureFile = default(SignatureFile);
			BinaryReader binaryReader = new BinaryReader(s);
			uint num = StreamHelpers.ReadBigEndianUint32(binaryReader);
			if (num != 1920139575)
			{
				throw new InvalidDataException($"Unknown magic number {num}");
			}
			signatureFile.StrongSumMethod = CalculateBlake2StrongSum;
			signatureFile.BlockLength = (int)StreamHelpers.ReadBigEndianUint32(binaryReader);
			signatureFile.StrongSumLength = (int)StreamHelpers.ReadBigEndianUint32(binaryReader);
			Dictionary<int, List<BlockSignature>> dictionary = new Dictionary<int, List<BlockSignature>>();
			ulong num2 = 0uL;
			while (true)
			{
				byte[] array = binaryReader.ReadBytes(4);
				if (array.Length == 0)
				{
					break;
				}
				int num3 = (int)StreamHelpers.ConvertFromBigEndian(array);
				byte[] strongSum = binaryReader.ReadBytes(signatureFile.StrongSumLength);
				List<BlockSignature> value = null;
				if (!dictionary.TryGetValue(num3, out value))
				{
					value = new List<BlockSignature>();
					dictionary.Add(num3, value);
				}
				value.Add(new BlockSignature
				{
					StartPos = (ulong)((long)signatureFile.BlockLength * (long)num2),
					WeakSum = num3,
					StrongSum = strongSum
				});
				num2++;
			}
			signatureFile.BlockLookup = dictionary;
			return signatureFile;
		}

		public unsafe static bool UnsafeEquals(byte[] strA, byte[] strB)
		{
			int num = strA.Length;
			if (num != strB.Length)
			{
				return false;
			}
			fixed (byte* ptr = strA)
			{
				fixed (byte* ptr3 = strB)
				{
					byte* ptr2 = ptr;
					byte* ptr4 = ptr3;
					while (num >= 4)
					{
						if (*(int*)ptr2 != *(int*)ptr4)
						{
							return false;
						}
						ptr2 += 4;
						ptr4 += 4;
						num -= 4;
					}
					while (num > 0)
					{
						if (*ptr2 != *ptr4)
						{
							return false;
						}
						ptr2++;
						ptr4++;
						num--;
					}
				}
			}
			return true;
		}
	}
}
