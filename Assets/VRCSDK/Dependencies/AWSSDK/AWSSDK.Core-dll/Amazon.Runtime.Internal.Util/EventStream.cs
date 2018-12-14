using System;
using System.Globalization;
using System.IO;
using System.Threading;

namespace Amazon.Runtime.Internal.Util
{
	internal class EventStream : WrapperStream
	{
		internal delegate void ReadProgress(int bytesRead);

		private class AsyncResult : IAsyncResult
		{
			public object AsyncState
			{
				get;
				internal set;
			}

			public WaitHandle AsyncWaitHandle
			{
				get;
				internal set;
			}

			public bool CompletedSynchronously
			{
				get;
				internal set;
			}

			public bool IsCompleted
			{
				get;
				internal set;
			}

			internal object Return
			{
				get;
				set;
			}
		}

		private bool disableClose;

		public override bool CanRead => base.BaseStream.CanRead;

		public override bool CanSeek => base.BaseStream.CanSeek;

		public override bool CanTimeout => base.BaseStream.CanTimeout;

		public override bool CanWrite => base.BaseStream.CanWrite;

		public override long Length => base.BaseStream.Length;

		public override long Position
		{
			get
			{
				return base.BaseStream.Position;
			}
			set
			{
				base.BaseStream.Position = value;
			}
		}

		public override int ReadTimeout
		{
			get
			{
				return base.BaseStream.ReadTimeout;
			}
			set
			{
				base.BaseStream.ReadTimeout = value;
			}
		}

		public override int WriteTimeout
		{
			get
			{
				return base.BaseStream.WriteTimeout;
			}
			set
			{
				base.BaseStream.WriteTimeout = value;
			}
		}

		internal event ReadProgress OnRead;

		internal EventStream(Stream stream, bool disableClose)
			: base(stream)
		{
			this.disableClose = disableClose;
		}

		protected override void Dispose(bool disposing)
		{
		}

		public override void Flush()
		{
			base.BaseStream.Flush();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int num = base.BaseStream.Read(buffer, offset, count);
			if (this.OnRead != null)
			{
				this.OnRead(num);
			}
			return num;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return base.BaseStream.Seek(offset, origin);
		}

		public override void SetLength(long value)
		{
			base.BaseStream.SetLength(value);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException();
		}

		public override void WriteByte(byte value)
		{
			throw new NotImplementedException();
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			AsyncResult asyncResult = new AsyncResult
			{
				AsyncState = state,
				CompletedSynchronously = true,
				IsCompleted = true,
				AsyncWaitHandle = new ManualResetEvent(initialState: true)
			};
			try
			{
				int num = Read(buffer, offset, count);
				asyncResult.Return = num;
			}
			catch (Exception ex)
			{
				Exception ex3 = (Exception)(asyncResult.Return = ex);
			}
			callback?.Invoke(asyncResult);
			return asyncResult;
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			throw new NotImplementedException();
		}

		public override void Close()
		{
			if (!disableClose)
			{
				base.BaseStream.Close();
			}
		}

		public override int EndRead(IAsyncResult asyncResult)
		{
			AsyncResult asyncResult2 = asyncResult as AsyncResult;
			if (asyncResult2 == null)
			{
				throw new ArgumentException("Parameter asyncResult was not of type Amazon.Runtime.Internal.Util.AsyncResult", "asyncResult");
			}
			if (asyncResult2.Return is Exception)
			{
				throw (Exception)asyncResult2.Return;
			}
			return Convert.ToInt32(asyncResult2.Return, CultureInfo.InvariantCulture);
		}

		public override void EndWrite(IAsyncResult asyncResult)
		{
			throw new NotImplementedException();
		}
	}
}
