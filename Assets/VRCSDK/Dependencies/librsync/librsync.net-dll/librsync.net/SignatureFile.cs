using System;
using System.Collections.Generic;

namespace librsync.net
{
	internal struct SignatureFile
	{
		public int BlockLength;

		public int StrongSumLength;

		public Func<byte[], byte[]> StrongSumMethod;

		public Dictionary<int, List<BlockSignature>> BlockLookup;
	}
}
