using System;
using System.Runtime.InteropServices;

namespace DotZLib
{
	public abstract class CodecBase : Codec, IDisposable
	{
		internal ZStream _ztream;

		protected bool _isDisposed;

		protected const int kBufferSize = 16384;

		private byte[] _outBuffer = new byte[16384];

		private byte[] _inBuffer = new byte[16384];

		private GCHandle _hInput;

		private GCHandle _hOutput;

		private uint _checksum;

		public uint Checksum => _checksum;

		public event DataAvailableHandler DataAvailable;

		public CodecBase()
		{
			try
			{
				_hInput = GCHandle.Alloc(_inBuffer, GCHandleType.Pinned);
				_hOutput = GCHandle.Alloc(_outBuffer, GCHandleType.Pinned);
			}
			catch (Exception)
			{
				CleanUp(isDisposing: false);
				throw;
			}
		}

		protected void OnDataAvailable()
		{
			if (_ztream.total_out != 0)
			{
				if (this.DataAvailable != null)
				{
					this.DataAvailable(_outBuffer, 0, (int)_ztream.total_out);
				}
				resetOutput();
			}
		}

		public void Add(byte[] data)
		{
			Add(data, 0, data.Length);
		}

		public abstract void Add(byte[] data, int offset, int count);

		public abstract void Finish();

		~CodecBase()
		{
			CleanUp(isDisposing: false);
		}

		public void Dispose()
		{
			CleanUp(isDisposing: true);
		}

		protected abstract void CleanUp();

		private void CleanUp(bool isDisposing)
		{
			if (!_isDisposed)
			{
				CleanUp();
				if (_hInput.IsAllocated)
				{
					_hInput.Free();
				}
				if (_hOutput.IsAllocated)
				{
					_hOutput.Free();
				}
				_isDisposed = true;
			}
		}

		protected void copyInput(byte[] data, int startIndex, int count)
		{
			Array.Copy(data, startIndex, _inBuffer, 0, count);
			_ztream.next_in = _hInput.AddrOfPinnedObject();
			_ztream.total_in = 0u;
			_ztream.avail_in = (uint)count;
		}

		protected void resetOutput()
		{
			_ztream.total_out = 0u;
			_ztream.avail_out = 16384u;
			_ztream.next_out = _hOutput.AddrOfPinnedObject();
		}

		protected void setChecksum(uint newSum)
		{
			_checksum = newSum;
		}
	}
}
