using System;
using System.IO;

namespace Amazon.Runtime.Internal.Util
{
	public interface IHashingWrapper : IDisposable
	{
		void Clear();

		byte[] ComputeHash(byte[] buffer);

		byte[] ComputeHash(Stream stream);

		void AppendBlock(byte[] buffer);

		void AppendBlock(byte[] buffer, int offset, int count);

		byte[] AppendLastBlock(byte[] buffer);

		byte[] AppendLastBlock(byte[] buffer, int offset, int count);
	}
}
