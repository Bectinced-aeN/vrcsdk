using System.IO;

namespace Amazon.Runtime.Internal.Util
{
	public class NonDisposingWrapperStream : WrapperStream
	{
		public NonDisposingWrapperStream(Stream baseStream)
			: base(baseStream)
		{
		}

		public override void Close()
		{
		}

		protected override void Dispose(bool disposing)
		{
		}
	}
}
