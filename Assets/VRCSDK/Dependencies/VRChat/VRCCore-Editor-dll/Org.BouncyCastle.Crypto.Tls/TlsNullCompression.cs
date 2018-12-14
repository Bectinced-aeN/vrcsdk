using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
	internal class TlsNullCompression : TlsCompression
	{
		public virtual Stream Compress(Stream output)
		{
			return output;
		}

		public virtual Stream Decompress(Stream output)
		{
			return output;
		}
	}
}
