using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
	internal interface TlsCompression
	{
		Stream Compress(Stream output);

		Stream Decompress(Stream output);
	}
}
