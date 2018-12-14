using System.Text;

namespace DotZLib
{
	public interface ChecksumGenerator
	{
		uint Value
		{
			get;
		}

		void Reset();

		void Update(byte[] data);

		void Update(byte[] data, int offset, int count);

		void Update(string data);

		void Update(string data, Encoding encoding);
	}
}
