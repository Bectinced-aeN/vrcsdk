using Amazon.Runtime.Internal.Auth;
using Amazon.Util;
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Amazon.Runtime.Internal.Util
{
	public class ChunkedUploadWrapperStream : WrapperStream
	{
		private enum ReadStrategy
		{
			ReadDirect,
			ReadAndCopy
		}

		public static readonly int DefaultChunkSize = 131072;

		private const string CLRF = "\r\n";

		private const string CHUNK_STRING_TO_SIGN_PREFIX = "AWS4-HMAC-SHA256-PAYLOAD";

		private const string CHUNK_SIGNATURE_HEADER = ";chunk-signature=";

		private const int SIGNATURE_LENGTH = 64;

		private byte[] _inputBuffer;

		private readonly byte[] _outputBuffer;

		private int _outputBufferPos = -1;

		private int _outputBufferDataLen = -1;

		private readonly int _wrappedStreamBufferSize;

		private bool _wrappedStreamConsumed;

		private bool _outputBufferIsTerminatingChunk;

		private readonly ReadStrategy _readStrategy;

		private AWS4SigningResult HeaderSigningResult
		{
			get;
			set;
		}

		private string PreviousChunkSignature
		{
			get;
			set;
		}

		public override long Length
		{
			get
			{
				if (base.BaseStream != null)
				{
					return ComputeChunkedContentLength(base.BaseStream.Length);
				}
				return 0L;
			}
		}

		public override bool CanSeek => false;

		internal ChunkedUploadWrapperStream(Stream stream, int wrappedStreamBufferSize, AWS4SigningResult headerSigningResult)
			: base(stream)
		{
			HeaderSigningResult = headerSigningResult;
			PreviousChunkSignature = headerSigningResult.Signature;
			_wrappedStreamBufferSize = wrappedStreamBufferSize;
			_inputBuffer = new byte[DefaultChunkSize];
			_outputBuffer = new byte[CalculateChunkHeaderLength(DefaultChunkSize)];
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (_outputBufferPos == -1)
			{
				if (_wrappedStreamConsumed && _outputBufferIsTerminatingChunk)
				{
					return 0;
				}
				int num = FillInputBuffer();
				ConstructOutputBufferChunk(num);
				_outputBufferIsTerminatingChunk = (_wrappedStreamConsumed && num == 0);
			}
			int num2 = _outputBufferDataLen - _outputBufferPos;
			if (num2 < count)
			{
				count = num2;
			}
			Buffer.BlockCopy(_outputBuffer, _outputBufferPos, buffer, offset, count);
			_outputBufferPos += count;
			if (_outputBufferPos >= _outputBufferDataLen)
			{
				_outputBufferPos = -1;
			}
			return count;
		}

		private void ConstructOutputBufferChunk(int dataLen)
		{
			if (dataLen > 0 && dataLen < _inputBuffer.Length)
			{
				byte[] array = new byte[dataLen];
				Buffer.BlockCopy(_inputBuffer, 0, array, 0, dataLen);
				_inputBuffer = array;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(dataLen.ToString("X", CultureInfo.InvariantCulture));
			string data = "AWS4-HMAC-SHA256-PAYLOAD\n" + HeaderSigningResult.ISO8601DateTime + "\n" + HeaderSigningResult.Scope + "\n" + PreviousChunkSignature + "\n" + AWSSDKUtils.ToHex(AWS4Signer.ComputeHash(""), lowercase: true) + "\n" + ((dataLen > 0) ? AWSSDKUtils.ToHex(AWS4Signer.ComputeHash(_inputBuffer), lowercase: true) : "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855");
			string str = PreviousChunkSignature = AWSSDKUtils.ToHex(AWS4Signer.SignBlob(HeaderSigningResult.SigningKey, data), lowercase: true);
			stringBuilder.Append(";chunk-signature=" + str);
			stringBuilder.Append("\r\n");
			try
			{
				byte[] bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
				byte[] bytes2 = Encoding.UTF8.GetBytes("\r\n");
				int num = 0;
				Buffer.BlockCopy(bytes, 0, _outputBuffer, num, bytes.Length);
				num += bytes.Length;
				if (dataLen > 0)
				{
					Buffer.BlockCopy(_inputBuffer, 0, _outputBuffer, num, dataLen);
					num += dataLen;
				}
				Buffer.BlockCopy(bytes2, 0, _outputBuffer, num, bytes2.Length);
				_outputBufferPos = 0;
				_outputBufferDataLen = bytes.Length + dataLen + bytes2.Length;
			}
			catch (Exception ex)
			{
				throw new AmazonClientException("Unable to sign the chunked data. " + ex.Message, ex);
			}
		}

		public static long ComputeChunkedContentLength(long originalLength)
		{
			if (originalLength < 0)
			{
				throw new ArgumentOutOfRangeException("originalLength", "Expected 0 or greater value for originalLength.");
			}
			if (originalLength == 0L)
			{
				return CalculateChunkHeaderLength(0L);
			}
			long num = originalLength / DefaultChunkSize;
			long num2 = originalLength % DefaultChunkSize;
			return num * CalculateChunkHeaderLength(DefaultChunkSize) + ((num2 > 0) ? CalculateChunkHeaderLength(num2) : 0) + CalculateChunkHeaderLength(0L);
		}

		private static long CalculateChunkHeaderLength(long chunkDataSize)
		{
			return chunkDataSize.ToString("X", CultureInfo.InvariantCulture).Length + ";chunk-signature=".Length + 64 + "\r\n".Length + chunkDataSize + "\r\n".Length;
		}

		private int FillInputBuffer()
		{
			if (_wrappedStreamConsumed)
			{
				return 0;
			}
			int num = 0;
			if (_readStrategy == ReadStrategy.ReadDirect)
			{
				while (num < _inputBuffer.Length && !_wrappedStreamConsumed)
				{
					int num2 = _inputBuffer.Length - num;
					if (num2 > _wrappedStreamBufferSize)
					{
						num2 = _wrappedStreamBufferSize;
					}
					int num3 = base.BaseStream.Read(_inputBuffer, num, num2);
					if (num3 == 0)
					{
						_wrappedStreamConsumed = true;
					}
					else
					{
						num += num3;
					}
				}
			}
			else
			{
				byte[] array = new byte[_wrappedStreamBufferSize];
				while (num < _inputBuffer.Length && !_wrappedStreamConsumed)
				{
					int num4 = base.BaseStream.Read(array, 0, _wrappedStreamBufferSize);
					if (num4 == 0)
					{
						_wrappedStreamConsumed = true;
					}
					else
					{
						Buffer.BlockCopy(array, 0, _inputBuffer, num, num4);
						num += num4;
					}
				}
			}
			return num;
		}
	}
}
