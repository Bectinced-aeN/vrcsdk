namespace ICSharpCode.SharpZipLib.Checksum
{
	public interface IChecksum
	{
		long Value
		{
			get;
		}

		void Reset();

		void Update(int bval);

		void Update(byte[] buffer);

		void Update(byte[] buffer, int offset, int count);
	}
}
