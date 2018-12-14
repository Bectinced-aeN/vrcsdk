using Org.BouncyCastle.Utilities;
using System;
using System.Text;

namespace Org.BouncyCastle.Asn1
{
	internal class DerBitString : DerStringBase
	{
		private static readonly char[] table = new char[16]
		{
			'0',
			'1',
			'2',
			'3',
			'4',
			'5',
			'6',
			'7',
			'8',
			'9',
			'A',
			'B',
			'C',
			'D',
			'E',
			'F'
		};

		private readonly byte[] data;

		private readonly int padBits;

		public int PadBits => padBits;

		public int IntValue
		{
			get
			{
				int num = 0;
				for (int i = 0; i != data.Length && i != 4; i++)
				{
					num |= (data[i] & 0xFF) << 8 * i;
				}
				return num;
			}
		}

		internal DerBitString(byte data, int padBits)
		{
			this.data = new byte[1]
			{
				data
			};
			this.padBits = padBits;
		}

		public DerBitString(byte[] data, int padBits)
		{
			this.data = data;
			this.padBits = padBits;
		}

		public DerBitString(byte[] data)
		{
			this.data = data;
		}

		public DerBitString(Asn1Encodable obj)
		{
			data = obj.GetDerEncoded();
		}

		internal static int GetPadBits(int bitString)
		{
			int num = 0;
			for (int num2 = 3; num2 >= 0; num2--)
			{
				if (num2 != 0)
				{
					if (bitString >> num2 * 8 != 0)
					{
						num = ((bitString >> num2 * 8) & 0xFF);
						break;
					}
				}
				else if (bitString != 0)
				{
					num = (bitString & 0xFF);
					break;
				}
			}
			if (num == 0)
			{
				return 7;
			}
			int num3 = 1;
			while (((num <<= 1) & 0xFF) != 0)
			{
				num3++;
			}
			return 8 - num3;
		}

		internal static byte[] GetBytes(int bitString)
		{
			int num = 4;
			int num2 = 3;
			while (num2 >= 1 && (bitString & (255 << num2 * 8)) == 0)
			{
				num--;
				num2--;
			}
			byte[] array = new byte[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = (byte)((bitString >> i * 8) & 0xFF);
			}
			return array;
		}

		public static DerBitString GetInstance(object obj)
		{
			if (obj == null || obj is DerBitString)
			{
				return (DerBitString)obj;
			}
			throw new ArgumentException("illegal object in GetInstance: " + obj.GetType().Name);
		}

		public static DerBitString GetInstance(Asn1TaggedObject obj, bool isExplicit)
		{
			Asn1Object @object = obj.GetObject();
			if (isExplicit || @object is DerBitString)
			{
				return GetInstance(@object);
			}
			return FromAsn1Octets(((Asn1OctetString)@object).GetOctets());
		}

		public byte[] GetBytes()
		{
			return data;
		}

		internal override void Encode(DerOutputStream derOut)
		{
			byte[] array = new byte[GetBytes().Length + 1];
			array[0] = (byte)PadBits;
			Array.Copy(GetBytes(), 0, array, 1, array.Length - 1);
			derOut.WriteEncoded(3, array);
		}

		protected override int Asn1GetHashCode()
		{
			return padBits.GetHashCode() ^ Arrays.GetHashCode(data);
		}

		protected override bool Asn1Equals(Asn1Object asn1Object)
		{
			DerBitString derBitString = asn1Object as DerBitString;
			if (derBitString == null)
			{
				return false;
			}
			return padBits == derBitString.padBits && Arrays.AreEqual(data, derBitString.data);
		}

		public override string GetString()
		{
			StringBuilder stringBuilder = new StringBuilder("#");
			byte[] derEncoded = GetDerEncoded();
			for (int i = 0; i != derEncoded.Length; i++)
			{
				uint num = derEncoded[i];
				stringBuilder.Append(table[(num >> 4) & 0xF]);
				stringBuilder.Append(table[derEncoded[i] & 0xF]);
			}
			return stringBuilder.ToString();
		}

		internal static DerBitString FromAsn1Octets(byte[] octets)
		{
			if (octets.Length < 1)
			{
				throw new ArgumentException("truncated BIT STRING detected");
			}
			int num = octets[0];
			byte[] array = new byte[octets.Length - 1];
			Array.Copy(octets, 1, array, 0, array.Length);
			return new DerBitString(array, num);
		}
	}
}
