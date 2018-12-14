using System.IO;

namespace Amazon.Runtime.Internal.Util
{
	public class MD5Stream : HashStream<HashingWrapperMD5>
	{
		private Logger _logger;

		public MD5Stream(Stream baseStream, byte[] expectedHash, long expectedLength)
			: base(baseStream, expectedHash, expectedLength)
		{
			_logger = Logger.GetLogger(GetType());
		}

		public override void CalculateHash()
		{
			if (!base.FinishedHashing)
			{
				if (base.ExpectedLength < 0 || base.CurrentPosition == base.ExpectedLength)
				{
					base.CalculatedHash = base.Algorithm.AppendLastBlock(new byte[0]);
				}
				else
				{
					base.CalculatedHash = new byte[0];
				}
				if (base.CalculatedHash.Length != 0 && base.ExpectedHash != null && base.ExpectedHash.Length != 0 && !HashStream.CompareHashes(base.ExpectedHash, base.CalculatedHash))
				{
					_logger.InfoFormat("The expected hash is not equal to the calculated hash. This can occur when the Http client decompresses a response body from gzip format before the hash is calculated, in which case thisis not an error.");
				}
			}
		}
	}
}
