using System;
using System.IO;
using System.Threading;
using VRC.Core.BestHTTP;

internal sealed class UploadStream : Stream
{
	private MemoryStream ReadBuffer = new MemoryStream();

	private MemoryStream WriteBuffer = new MemoryStream();

	private bool noMoreData;

	private AutoResetEvent ARE = new AutoResetEvent(initialState: false);

	private object locker = new object();

	public string Name
	{
		get;
		private set;
	}

	private bool IsReadBufferEmpty
	{
		get
		{
			lock (locker)
			{
				return ReadBuffer.Position == ReadBuffer.Length;
				IL_002b:
				bool result;
				return result;
			}
		}
	}

	public override bool CanRead
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public override bool CanSeek
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public override bool CanWrite
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public override long Length
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public override long Position
	{
		get
		{
			throw new NotImplementedException();
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	public UploadStream(string name)
		: this()
	{
		Name = name;
	}

	public UploadStream()
	{
		ReadBuffer = new MemoryStream();
		WriteBuffer = new MemoryStream();
		Name = string.Empty;
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		if (noMoreData)
		{
			if (ReadBuffer.Position != ReadBuffer.Length)
			{
				return ReadBuffer.Read(buffer, offset, count);
			}
			if (WriteBuffer.Length <= 0)
			{
				HTTPManager.Logger.Information("UploadStream", $"{Name} - Read - End Of Stream");
				return -1;
			}
			SwitchBuffers();
		}
		if (IsReadBufferEmpty)
		{
			ARE.WaitOne();
			lock (locker)
			{
				if (IsReadBufferEmpty && WriteBuffer.Length > 0)
				{
					SwitchBuffers();
				}
			}
		}
		int num = -1;
		lock (locker)
		{
			return ReadBuffer.Read(buffer, offset, count);
		}
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		if (noMoreData)
		{
			throw new ArgumentException("noMoreData already set!");
		}
		lock (locker)
		{
			WriteBuffer.Write(buffer, offset, count);
			SwitchBuffers();
		}
		ARE.Set();
	}

	public override void Flush()
	{
		Finish();
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			HTTPManager.Logger.Information("UploadStream", $"{Name} - Dispose");
			ReadBuffer.Dispose();
			ReadBuffer = null;
			WriteBuffer.Dispose();
			WriteBuffer = null;
			ARE.Close();
			ARE = null;
		}
		base.Dispose(disposing);
	}

	public void Finish()
	{
		if (noMoreData)
		{
			throw new ArgumentException("noMoreData already set!");
		}
		HTTPManager.Logger.Information("UploadStream", $"{Name} - Finish");
		noMoreData = true;
		ARE.Set();
	}

	private bool SwitchBuffers()
	{
		lock (locker)
		{
			if (ReadBuffer.Position == ReadBuffer.Length)
			{
				WriteBuffer.Seek(0L, SeekOrigin.Begin);
				ReadBuffer.SetLength(0L);
				MemoryStream writeBuffer = WriteBuffer;
				WriteBuffer = ReadBuffer;
				ReadBuffer = writeBuffer;
				return true;
			}
		}
		return false;
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		throw new NotImplementedException();
	}

	public override void SetLength(long value)
	{
		throw new NotImplementedException();
	}
}
