using System.IO;

namespace Org.BouncyCastle.Utilities.IO
{
	internal class TeeOutputStream : BaseOutputStream
	{
		private readonly Stream output;

		private readonly Stream tee;

		public TeeOutputStream(Stream output, Stream tee)
		{
			this.output = output;
			this.tee = tee;
		}

		protected override void Dispose(bool isDisposing)
		{
			try
			{
				output.Dispose();
				tee.Dispose();
			}
			finally
			{
				base.Dispose(isDisposing);
			}
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			output.Write(buffer, offset, count);
			tee.Write(buffer, offset, count);
		}

		public override void WriteByte(byte b)
		{
			output.WriteByte(b);
			tee.WriteByte(b);
		}
	}
}
