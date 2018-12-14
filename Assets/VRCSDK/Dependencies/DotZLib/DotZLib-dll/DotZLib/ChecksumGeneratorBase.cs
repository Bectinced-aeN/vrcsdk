using System.Text;

namespace DotZLib
{
	public abstract class ChecksumGeneratorBase : ChecksumGenerator
	{
		protected uint _current;

		public uint Value => _current;

		public ChecksumGeneratorBase()
		{
			_current = 0u;
		}

		public ChecksumGeneratorBase(uint initialValue)
		{
			_current = initialValue;
		}

		public void Reset()
		{
			_current = 0u;
		}

		public abstract void Update(byte[] data, int offset, int count);

		public void Update(byte[] data)
		{
			Update(data, 0, data.Length);
		}

		public void Update(string data)
		{
			Update(Encoding.UTF8.GetBytes(data));
		}

		public void Update(string data, Encoding encoding)
		{
			Update(encoding.GetBytes(data));
		}
	}
}
