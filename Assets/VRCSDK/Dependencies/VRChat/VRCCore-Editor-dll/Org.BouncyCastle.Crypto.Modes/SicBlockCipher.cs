using Org.BouncyCastle.Crypto.Parameters;
using System;

namespace Org.BouncyCastle.Crypto.Modes
{
	internal class SicBlockCipher : IBlockCipher
	{
		private readonly IBlockCipher cipher;

		private readonly int blockSize;

		private readonly byte[] IV;

		private readonly byte[] counter;

		private readonly byte[] counterOut;

		public string AlgorithmName => cipher.AlgorithmName + "/SIC";

		public bool IsPartialBlockOkay => true;

		public SicBlockCipher(IBlockCipher cipher)
		{
			this.cipher = cipher;
			blockSize = cipher.GetBlockSize();
			IV = new byte[blockSize];
			counter = new byte[blockSize];
			counterOut = new byte[blockSize];
		}

		public IBlockCipher GetUnderlyingCipher()
		{
			return cipher;
		}

		public void Init(bool forEncryption, ICipherParameters parameters)
		{
			if (!(parameters is ParametersWithIV))
			{
				throw new ArgumentException("SIC mode requires ParametersWithIV", "parameters");
			}
			ParametersWithIV parametersWithIV = (ParametersWithIV)parameters;
			byte[] iV = parametersWithIV.GetIV();
			Array.Copy(iV, 0, IV, 0, IV.Length);
			Reset();
			if (parametersWithIV.Parameters != null)
			{
				cipher.Init(forEncryption: true, parametersWithIV.Parameters);
			}
		}

		public int GetBlockSize()
		{
			return cipher.GetBlockSize();
		}

		public int ProcessBlock(byte[] input, int inOff, byte[] output, int outOff)
		{
			cipher.ProcessBlock(counter, 0, counterOut, 0);
			for (int i = 0; i < counterOut.Length; i++)
			{
				output[outOff + i] = (byte)(counterOut[i] ^ input[inOff + i]);
			}
			int num = counter.Length;
			while (--num >= 0 && ++counter[num] == 0)
			{
			}
			return counter.Length;
		}

		public void Reset()
		{
			Array.Copy(IV, 0, counter, 0, counter.Length);
			cipher.Reset();
		}
	}
}
