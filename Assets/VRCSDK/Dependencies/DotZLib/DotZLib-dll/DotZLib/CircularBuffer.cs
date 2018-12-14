using System;

namespace DotZLib
{
	internal class CircularBuffer
	{
		private int _capacity;

		private int _head;

		private int _tail;

		private int _size;

		private byte[] _buffer;

		public int Size => _size;

		public CircularBuffer(int capacity)
		{
			_buffer = new byte[capacity];
			_capacity = capacity;
			_head = 0;
			_tail = 0;
			_size = 0;
		}

		public int Put(byte[] source, int offset, int count)
		{
			int num = Math.Min(count, _capacity - Size);
			for (int i = 0; i < num; i++)
			{
				_buffer[(_tail + i) % _capacity] = source[offset + i];
			}
			_tail += num;
			_tail %= _capacity;
			_size += num;
			return num;
		}

		public bool Put(byte b)
		{
			if (Size == _capacity)
			{
				return false;
			}
			_buffer[_tail++] = b;
			_tail %= _capacity;
			_size++;
			return true;
		}

		public int Get(byte[] destination, int offset, int count)
		{
			int num = Math.Min(count, Size);
			for (int i = 0; i < num; i++)
			{
				destination[offset + i] = _buffer[(_head + i) % _capacity];
			}
			_head += num;
			_head %= _capacity;
			_size -= num;
			return num;
		}

		public int Get()
		{
			if (Size != 0)
			{
				byte result = _buffer[_head++ % _capacity];
				_size--;
				return result;
			}
			return -1;
		}
	}
}
