using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Utilities.Encoders;
using System.IO;
using System.Text;

namespace Org.BouncyCastle.X509
{
	internal class PemParser
	{
		private readonly string _header1;

		private readonly string _header2;

		private readonly string _footer1;

		private readonly string _footer2;

		internal PemParser(string type)
		{
			_header1 = "-----BEGIN " + type + "-----";
			_header2 = "-----BEGIN X509 " + type + "-----";
			_footer1 = "-----END " + type + "-----";
			_footer2 = "-----END X509 " + type + "-----";
		}

		private string ReadLine(Stream inStream)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num;
			while (true)
			{
				if ((num = inStream.ReadByte()) != 13 && num != 10 && num >= 0)
				{
					if (num != 13)
					{
						stringBuilder.Append((char)num);
					}
				}
				else if (num < 0 || stringBuilder.Length != 0)
				{
					break;
				}
			}
			if (num < 0)
			{
				return null;
			}
			return stringBuilder.ToString();
		}

		internal Asn1Sequence ReadPemObject(Stream inStream)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string text;
			while ((text = ReadLine(inStream)) != null && !text.StartsWith(_header1) && !text.StartsWith(_header2))
			{
			}
			while ((text = ReadLine(inStream)) != null && !text.StartsWith(_footer1) && !text.StartsWith(_footer2))
			{
				stringBuilder.Append(text);
			}
			if (stringBuilder.Length != 0)
			{
				Asn1Object asn1Object = Asn1Object.FromByteArray(Base64.Decode(stringBuilder.ToString()));
				if (!(asn1Object is Asn1Sequence))
				{
					throw new IOException("malformed PEM data encountered");
				}
				return (Asn1Sequence)asn1Object;
			}
			return null;
		}
	}
}
