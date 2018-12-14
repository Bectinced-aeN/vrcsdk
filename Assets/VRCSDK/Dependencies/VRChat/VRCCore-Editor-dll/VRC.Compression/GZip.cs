using System.IO;
using System.IO.Compression;
using UnityEngine;

namespace VRC.Compression
{
	public class GZip
	{
		public static bool IsValid(byte[] gzip)
		{
			return gzip.Length >= 2 && gzip[0] == 31 && gzip[1] == 139;
		}

		public static bool IsValid(string compressedPath)
		{
			byte[] gzip = File.ReadAllBytes(compressedPath);
			return IsValid(gzip);
		}

		public static byte[] Compress(byte[] raw)
		{
			Debug.Log((object)"GZip.Compress(bytes)");
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, leaveOpen: true))
				{
					gZipStream.Write(raw, 0, raw.Length);
				}
				return memoryStream.ToArray();
				IL_0042:
				byte[] result;
				return result;
			}
		}

		public static void CompressToFile(string uncompressedPath, string compressedPath)
		{
			Debug.Log((object)("CompressToFile(" + uncompressedPath + ", " + compressedPath + ")"));
			byte[] raw = File.ReadAllBytes(uncompressedPath);
			CompressToFile(raw, compressedPath);
		}

		public static void CompressToFile(byte[] raw, string path)
		{
			Debug.Log((object)("CompressToFile(lots of bytes," + path + ")"));
			byte[] bytes = Compress(raw);
			FileInfo fileInfo = new FileInfo(path);
			fileInfo.Directory.Create();
			File.WriteAllBytes(fileInfo.FullName, bytes);
		}

		public static byte[] Decompress(byte[] gzip)
		{
			using (GZipStream gZipStream = new GZipStream(new MemoryStream(gzip), CompressionMode.Decompress))
			{
				byte[] array = new byte[4096];
				using (MemoryStream memoryStream = new MemoryStream())
				{
					int num = 0;
					do
					{
						num = gZipStream.Read(array, 0, 4096);
						if (num > 0)
						{
							memoryStream.Write(array, 0, num);
						}
					}
					while (num > 0);
					return memoryStream.ToArray();
					IL_0057:
					byte[] result;
					return result;
				}
			}
		}

		public static void DecompressToFile(string compressedPath, string uncompressedPath)
		{
			byte[] gzip = File.ReadAllBytes(compressedPath);
			DecompressToFile(gzip, uncompressedPath);
		}

		public static void DecompressToFile(byte[] gzip, string path)
		{
			byte[] bytes = Decompress(gzip);
			FileInfo fileInfo = new FileInfo(path);
			fileInfo.Directory.Create();
			File.WriteAllBytes(fileInfo.FullName, bytes);
		}
	}
}
