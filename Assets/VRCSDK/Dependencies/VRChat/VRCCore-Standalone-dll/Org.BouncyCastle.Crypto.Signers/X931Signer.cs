using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections;

namespace Org.BouncyCastle.Crypto.Signers
{
	internal class X931Signer : ISigner
	{
		public const int TRAILER_IMPLICIT = 188;

		public const int TRAILER_RIPEMD160 = 12748;

		public const int TRAILER_RIPEMD128 = 13004;

		public const int TRAILER_SHA1 = 13260;

		public const int TRAILER_SHA256 = 13516;

		public const int TRAILER_SHA512 = 13772;

		public const int TRAILER_SHA384 = 14028;

		public const int TRAILER_WHIRLPOOL = 14284;

		public const int TRAILER_SHA224 = 14540;

		private static readonly IDictionary trailerMap;

		private IDigest digest;

		private IAsymmetricBlockCipher cipher;

		private RsaKeyParameters kParam;

		private int trailer;

		private int keyBits;

		private byte[] block;

		public virtual string AlgorithmName => digest.AlgorithmName + "with" + cipher.AlgorithmName + "/X9.31";

		public X931Signer(IAsymmetricBlockCipher cipher, IDigest digest, bool isImplicit)
		{
			this.cipher = cipher;
			this.digest = digest;
			if (isImplicit)
			{
				trailer = 188;
			}
			else
			{
				string algorithmName = digest.AlgorithmName;
				if (!trailerMap.Contains(algorithmName))
				{
					throw new ArgumentException("no valid trailer", "digest");
				}
				trailer = (int)trailerMap[algorithmName];
			}
		}

		public X931Signer(IAsymmetricBlockCipher cipher, IDigest digest)
			: this(cipher, digest, isImplicit: false)
		{
		}

		static X931Signer()
		{
			trailerMap = Platform.CreateHashtable();
			trailerMap.Add("RIPEMD128", 13004);
			trailerMap.Add("RIPEMD160", 12748);
			trailerMap.Add("SHA-1", 13260);
			trailerMap.Add("SHA-224", 14540);
			trailerMap.Add("SHA-256", 13516);
			trailerMap.Add("SHA-384", 14028);
			trailerMap.Add("SHA-512", 13772);
			trailerMap.Add("Whirlpool", 14284);
		}

		public virtual void Init(bool forSigning, ICipherParameters parameters)
		{
			kParam = (RsaKeyParameters)parameters;
			cipher.Init(forSigning, kParam);
			keyBits = kParam.Modulus.BitLength;
			block = new byte[(keyBits + 7) / 8];
			Reset();
		}

		private void ClearBlock(byte[] block)
		{
			Array.Clear(block, 0, block.Length);
		}

		public virtual void Update(byte b)
		{
			digest.Update(b);
		}

		public virtual void BlockUpdate(byte[] input, int off, int len)
		{
			digest.BlockUpdate(input, off, len);
		}

		public virtual void Reset()
		{
			digest.Reset();
		}

		public virtual byte[] GenerateSignature()
		{
			CreateSignatureBlock();
			BigInteger bigInteger = new BigInteger(1, cipher.ProcessBlock(block, 0, block.Length));
			ClearBlock(block);
			bigInteger = bigInteger.Min(kParam.Modulus.Subtract(bigInteger));
			return BigIntegers.AsUnsignedByteArray((kParam.Modulus.BitLength + 7) / 8, bigInteger);
		}

		private void CreateSignatureBlock()
		{
			int digestSize = digest.GetDigestSize();
			int num;
			if (trailer == 188)
			{
				num = block.Length - digestSize - 1;
				digest.DoFinal(block, num);
				block[block.Length - 1] = 188;
			}
			else
			{
				num = block.Length - digestSize - 2;
				digest.DoFinal(block, num);
				block[block.Length - 2] = (byte)(trailer >> 8);
				block[block.Length - 1] = (byte)trailer;
			}
			block[0] = 107;
			for (int num2 = num - 2; num2 != 0; num2--)
			{
				block[num2] = 187;
			}
			block[num - 1] = 186;
		}

		public virtual bool VerifySignature(byte[] signature)
		{
			try
			{
				block = cipher.ProcessBlock(signature, 0, signature.Length);
			}
			catch (Exception)
			{
				return false;
				IL_0024:;
			}
			BigInteger bigInteger = new BigInteger(block);
			BigInteger n;
			if ((bigInteger.IntValue & 0xF) == 12)
			{
				n = bigInteger;
			}
			else
			{
				bigInteger = kParam.Modulus.Subtract(bigInteger);
				if ((bigInteger.IntValue & 0xF) != 12)
				{
					return false;
				}
				n = bigInteger;
			}
			CreateSignatureBlock();
			byte[] b = BigIntegers.AsUnsignedByteArray(block.Length, n);
			bool result = Arrays.ConstantTimeAreEqual(block, b);
			ClearBlock(block);
			ClearBlock(b);
			return result;
		}
	}
}
