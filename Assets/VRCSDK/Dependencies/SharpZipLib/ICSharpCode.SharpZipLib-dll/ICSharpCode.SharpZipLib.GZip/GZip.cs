using ICSharpCode.SharpZipLib.Core;
using System;
using System.IO;

namespace ICSharpCode.SharpZipLib.GZip
{
	public static class GZip
	{
		public static void Decompress(Stream inStream, Stream outStream, bool isStreamOwner)
		{
			if (inStream == null || outStream == null)
			{
				throw new Exception("Null Stream");
			}
			try
			{
				using (GZipInputStream gZipInputStream = new GZipInputStream(inStream))
				{
					gZipInputStream.IsStreamOwner = isStreamOwner;
					StreamUtils.Copy(gZipInputStream, outStream, new byte[4096]);
				}
			}
			finally
			{
				if (isStreamOwner)
				{
					outStream.Dispose();
				}
			}
		}

		public static void Compress(Stream inStream, Stream outStream, bool isStreamOwner, int level)
		{
			if (inStream == null || outStream == null)
			{
				throw new Exception("Null Stream");
			}
			try
			{
				using (GZipOutputStream gZipOutputStream = new GZipOutputStream(outStream, level))
				{
					gZipOutputStream.IsStreamOwner = isStreamOwner;
					StreamUtils.Copy(inStream, gZipOutputStream, new byte[4096]);
				}
			}
			finally
			{
				if (isStreamOwner)
				{
					inStream.Dispose();
				}
			}
		}
	}
}
