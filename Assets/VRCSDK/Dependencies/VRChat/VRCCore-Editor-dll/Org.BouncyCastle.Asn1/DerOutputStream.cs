using Org.BouncyCastle.Asn1.Utilities;
using System;
using System.IO;

namespace Org.BouncyCastle.Asn1
{
	internal class DerOutputStream : FilterStream
	{
		public DerOutputStream(Stream os)
			: base(os)
		{
		}

		private void WriteLength(int length)
		{
			if (length > 127)
			{
				int num = 1;
				uint num2 = (uint)length;
				while ((num2 >>= 8) != 0)
				{
					num++;
				}
				WriteByte((byte)(num | 0x80));
				for (int num3 = (num - 1) * 8; num3 >= 0; num3 -= 8)
				{
					WriteByte((byte)(length >> num3));
				}
			}
			else
			{
				WriteByte((byte)length);
			}
		}

		internal void WriteEncoded(int tag, byte[] bytes)
		{
			WriteByte((byte)tag);
			WriteLength(bytes.Length);
			Write(bytes, 0, bytes.Length);
		}

		internal void WriteEncoded(int tag, byte[] bytes, int offset, int length)
		{
			WriteByte((byte)tag);
			WriteLength(length);
			Write(bytes, offset, length);
		}

		internal void WriteTag(int flags, int tagNo)
		{
			if (tagNo < 31)
			{
				WriteByte((byte)(flags | tagNo));
			}
			else
			{
				WriteByte((byte)(flags | 0x1F));
				if (tagNo < 128)
				{
					WriteByte((byte)tagNo);
				}
				else
				{
					byte[] array = new byte[5];
					int num = array.Length;
					array[--num] = (byte)(tagNo & 0x7F);
					do
					{
						tagNo >>= 7;
						array[--num] = (byte)((tagNo & 0x7F) | 0x80);
					}
					while (tagNo > 127);
					Write(array, num, array.Length - num);
				}
			}
		}

		internal void WriteEncoded(int flags, int tagNo, byte[] bytes)
		{
			WriteTag(flags, tagNo);
			WriteLength(bytes.Length);
			Write(bytes, 0, bytes.Length);
		}

		protected void WriteNull()
		{
			WriteByte(5);
			WriteByte(0);
		}

		[Obsolete("Use version taking an Asn1Encodable arg instead")]
		public virtual void WriteObject(object obj)
		{
			if (obj == null)
			{
				WriteNull();
			}
			else if (obj is Asn1Object)
			{
				((Asn1Object)obj).Encode(this);
			}
			else
			{
				if (!(obj is Asn1Encodable))
				{
					throw new IOException("object not Asn1Object");
				}
				((Asn1Encodable)obj).ToAsn1Object().Encode(this);
			}
		}

		public virtual void WriteObject(Asn1Encodable obj)
		{
			if (obj == null)
			{
				WriteNull();
			}
			else
			{
				obj.ToAsn1Object().Encode(this);
			}
		}

		public virtual void WriteObject(Asn1Object obj)
		{
			if (obj == null)
			{
				WriteNull();
			}
			else
			{
				obj.Encode(this);
			}
		}
	}
}
