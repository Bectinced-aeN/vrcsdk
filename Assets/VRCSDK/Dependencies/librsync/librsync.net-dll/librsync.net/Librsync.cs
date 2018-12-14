using System.IO;

namespace librsync.net
{
	public static class Librsync
	{
		public static Stream ComputeSignature(Stream inputFile)
		{
			return ComputeSignature(inputFile, new SignatureJobSettings
			{
				MagicNumber = MagicNumber.Blake2Signature,
				BlockLength = 2048,
				StrongSumLength = 32
			});
		}

		public static Stream ComputeSignature(Stream inputFile, SignatureJobSettings settings)
		{
			return new SignatureStream(inputFile, settings);
		}

		public static Stream ComputeDelta(Stream signature, Stream newFile)
		{
			return new DeltaStream(signature, newFile);
		}

		public static Stream ApplyDelta(Stream originalFile, Stream delta)
		{
			return new PatchedStream(originalFile, delta);
		}
	}
}
