using System;
using System.IO;

namespace Amazon.Runtime.Internal.Util
{
	public abstract class HashStream : WrapperStream
	{
		protected IHashingWrapper Algorithm
		{
			get;
			set;
		}

		protected bool FinishedHashing => CalculatedHash != null;

		protected long CurrentPosition
		{
			get;
			private set;
		}

		public byte[] CalculatedHash
		{
			get;
			protected set;
		}

		public byte[] ExpectedHash
		{
			get;
			private set;
		}

		public long ExpectedLength
		{
			get;
			protected set;
		}

		public override bool CanSeek => false;

		public override long Position
		{
			get
			{
				throw new NotSupportedException("HashStream does not support seeking");
			}
			set
			{
				throw new NotSupportedException("HashStream does not support seeking");
			}
		}

		public override long Length => ExpectedLength;

		protected HashStream(Stream baseStream, byte[] expectedHash, long expectedLength)
			: base(baseStream)
		{
			ExpectedHash = expectedHash;
			ExpectedLength = expectedLength;
			ValidateBaseStream();
			Reset();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int num = base.Read(buffer, offset, count);
			CurrentPosition += num;
			if (!FinishedHashing)
			{
				Algorithm.AppendBlock(buffer, offset, num);
			}
			return num;
		}

		public override void Close()
		{
			CalculateHash();
			base.Close();
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				CalculateHash();
				if (disposing && Algorithm != null)
				{
					Algorithm.Dispose();
					Algorithm = null;
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException("HashStream does not support seeking");
		}

		public virtual void CalculateHash()
		{
			if (!FinishedHashing)
			{
				if (ExpectedLength < 0 || CurrentPosition == ExpectedLength)
				{
					CalculatedHash = Algorithm.AppendLastBlock(new byte[0]);
				}
				else
				{
					CalculatedHash = new byte[0];
				}
				if (CalculatedHash.Length != 0 && ExpectedHash != null && ExpectedHash.Length != 0 && !CompareHashes(ExpectedHash, CalculatedHash))
				{
					throw new AmazonClientException("Expected hash not equal to calculated hash");
				}
			}
		}

		public void Reset()
		{
			CurrentPosition = 0L;
			CalculatedHash = null;
			if (Algorithm != null)
			{
				Algorithm.Clear();
			}
			(base.BaseStream as HashStream)?.Reset();
		}

		private void ValidateBaseStream()
		{
			if (!base.BaseStream.CanRead && !base.BaseStream.CanWrite)
			{
				throw new InvalidDataException("HashStream does not support base streams that are not capable of reading or writing");
			}
		}

		protected static bool CompareHashes(byte[] expected, byte[] actual)
		{
			if (expected == actual)
			{
				return true;
			}
			if (expected == null || actual == null)
			{
				return expected == actual;
			}
			if (expected.Length != actual.Length)
			{
				return false;
			}
			for (int i = 0; i < expected.Length; i++)
			{
				if (expected[i] != actual[i])
				{
					return false;
				}
			}
			return true;
		}
	}
	public class HashStream<T> : HashStream where T : IHashingWrapper, new()
	{
		public HashStream(Stream baseStream, byte[] expectedHash, long expectedLength)
			: base(baseStream, expectedHash, expectedLength)
		{
			base.Algorithm = (IHashingWrapper)(object)new T();
		}
	}
}
