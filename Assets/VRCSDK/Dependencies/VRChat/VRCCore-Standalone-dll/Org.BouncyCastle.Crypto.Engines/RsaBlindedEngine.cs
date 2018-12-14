using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Engines
{
	internal class RsaBlindedEngine : IAsymmetricBlockCipher
	{
		private readonly RsaCoreEngine core = new RsaCoreEngine();

		private RsaKeyParameters key;

		private SecureRandom random;

		public virtual string AlgorithmName => "RSA";

		public virtual void Init(bool forEncryption, ICipherParameters param)
		{
			core.Init(forEncryption, param);
			if (param is ParametersWithRandom)
			{
				ParametersWithRandom parametersWithRandom = (ParametersWithRandom)param;
				key = (RsaKeyParameters)parametersWithRandom.Parameters;
				random = parametersWithRandom.Random;
			}
			else
			{
				key = (RsaKeyParameters)param;
				random = new SecureRandom();
			}
		}

		public virtual int GetInputBlockSize()
		{
			return core.GetInputBlockSize();
		}

		public virtual int GetOutputBlockSize()
		{
			return core.GetOutputBlockSize();
		}

		public virtual byte[] ProcessBlock(byte[] inBuf, int inOff, int inLen)
		{
			if (key == null)
			{
				throw new InvalidOperationException("RSA engine not initialised");
			}
			BigInteger bigInteger = core.ConvertInput(inBuf, inOff, inLen);
			BigInteger result;
			if (key is RsaPrivateCrtKeyParameters)
			{
				RsaPrivateCrtKeyParameters rsaPrivateCrtKeyParameters = (RsaPrivateCrtKeyParameters)key;
				BigInteger publicExponent = rsaPrivateCrtKeyParameters.PublicExponent;
				if (publicExponent != null)
				{
					BigInteger modulus = rsaPrivateCrtKeyParameters.Modulus;
					BigInteger bigInteger2 = BigIntegers.CreateRandomInRange(BigInteger.One, modulus.Subtract(BigInteger.One), random);
					BigInteger input = bigInteger2.ModPow(publicExponent, modulus).Multiply(bigInteger).Mod(modulus);
					BigInteger bigInteger3 = core.ProcessBlock(input);
					BigInteger val = bigInteger2.ModInverse(modulus);
					result = bigInteger3.Multiply(val).Mod(modulus);
				}
				else
				{
					result = core.ProcessBlock(bigInteger);
				}
			}
			else
			{
				result = core.ProcessBlock(bigInteger);
			}
			return core.ConvertOutput(result);
		}
	}
}
