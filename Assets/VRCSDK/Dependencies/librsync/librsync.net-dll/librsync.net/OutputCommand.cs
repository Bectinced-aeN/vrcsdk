using System.Collections.Generic;

namespace librsync.net
{
	internal struct OutputCommand
	{
		public CommandKind Kind;

		public ulong Position;

		public ulong Length;

		public List<byte> Literal;
	}
}
