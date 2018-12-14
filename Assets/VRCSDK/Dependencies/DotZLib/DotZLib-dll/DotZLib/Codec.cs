namespace DotZLib
{
	public interface Codec
	{
		uint Checksum
		{
			get;
		}

		event DataAvailableHandler DataAvailable;

		void Add(byte[] data);

		void Add(byte[] data, int offset, int count);

		void Finish();
	}
}
