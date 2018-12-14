using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Iana;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Utilities;
using System.Collections;

namespace Org.BouncyCastle.Security
{
	internal sealed class MacUtilities
	{
		private static readonly IDictionary algorithms;

		private MacUtilities()
		{
		}

		static MacUtilities()
		{
			algorithms = Platform.CreateHashtable();
			algorithms[IanaObjectIdentifiers.HmacMD5.Id] = "HMAC-MD5";
			algorithms[IanaObjectIdentifiers.HmacRipeMD160.Id] = "HMAC-RIPEMD160";
			algorithms[IanaObjectIdentifiers.HmacSha1.Id] = "HMAC-SHA1";
			algorithms[IanaObjectIdentifiers.HmacTiger.Id] = "HMAC-TIGER";
			algorithms[PkcsObjectIdentifiers.IdHmacWithSha1.Id] = "HMAC-SHA1";
			algorithms[PkcsObjectIdentifiers.IdHmacWithSha224.Id] = "HMAC-SHA224";
			algorithms[PkcsObjectIdentifiers.IdHmacWithSha256.Id] = "HMAC-SHA256";
			algorithms[PkcsObjectIdentifiers.IdHmacWithSha384.Id] = "HMAC-SHA384";
			algorithms[PkcsObjectIdentifiers.IdHmacWithSha512.Id] = "HMAC-SHA512";
			algorithms["DES"] = "DESMAC";
			algorithms["DES/CFB8"] = "DESMAC/CFB8";
			algorithms["DES64"] = "DESMAC64";
			algorithms["DESEDE"] = "DESEDEMAC";
			algorithms[PkcsObjectIdentifiers.DesEde3Cbc.Id] = "DESEDEMAC";
			algorithms["DESEDE/CFB8"] = "DESEDEMAC/CFB8";
			algorithms["DESISO9797MAC"] = "DESWITHISO9797";
			algorithms["DESEDE64"] = "DESEDEMAC64";
			algorithms["DESEDE64WITHISO7816-4PADDING"] = "DESEDEMAC64WITHISO7816-4PADDING";
			algorithms["DESEDEISO9797ALG1MACWITHISO7816-4PADDING"] = "DESEDEMAC64WITHISO7816-4PADDING";
			algorithms["DESEDEISO9797ALG1WITHISO7816-4PADDING"] = "DESEDEMAC64WITHISO7816-4PADDING";
			algorithms["ISO9797ALG3"] = "ISO9797ALG3MAC";
			algorithms["ISO9797ALG3MACWITHISO7816-4PADDING"] = "ISO9797ALG3WITHISO7816-4PADDING";
			algorithms["SKIPJACK"] = "SKIPJACKMAC";
			algorithms["SKIPJACK/CFB8"] = "SKIPJACKMAC/CFB8";
			algorithms["IDEA"] = "IDEAMAC";
			algorithms["IDEA/CFB8"] = "IDEAMAC/CFB8";
			algorithms["RC2"] = "RC2MAC";
			algorithms["RC2/CFB8"] = "RC2MAC/CFB8";
			algorithms["RC5"] = "RC5MAC";
			algorithms["RC5/CFB8"] = "RC5MAC/CFB8";
			algorithms["GOST28147"] = "GOST28147MAC";
			algorithms["VMPC"] = "VMPCMAC";
			algorithms["VMPC-MAC"] = "VMPCMAC";
			algorithms["SIPHASH"] = "SIPHASH-2-4";
			algorithms["PBEWITHHMACSHA"] = "PBEWITHHMACSHA1";
			algorithms["1.3.14.3.2.26"] = "PBEWITHHMACSHA1";
		}

		public static IMac GetMac(DerObjectIdentifier id)
		{
			return GetMac(id.Id);
		}

		public static IMac GetMac(string algorithm)
		{
			string text = Platform.ToUpperInvariant(algorithm);
			string text2 = (string)algorithms[text];
			if (text2 == null)
			{
				text2 = text;
			}
			if (text2.StartsWith("PBEWITH"))
			{
				text2 = text2.Substring("PBEWITH".Length);
			}
			if (text2.StartsWith("HMAC"))
			{
				string algorithm2 = (!text2.StartsWith("HMAC-") && !text2.StartsWith("HMAC/")) ? text2.Substring(4) : text2.Substring(5);
				return new HMac(DigestUtilities.GetDigest(algorithm2));
			}
			if (text2 == "AESCMAC")
			{
				return new CMac(new AesFastEngine());
			}
			if (text2 == "DESMAC")
			{
				return new CbcBlockCipherMac(new DesEngine());
			}
			if (text2 == "DESMAC/CFB8")
			{
				return new CfbBlockCipherMac(new DesEngine());
			}
			if (text2 == "DESMAC64")
			{
				return new CbcBlockCipherMac(new DesEngine(), 64);
			}
			if (text2 == "DESEDECMAC")
			{
				return new CMac(new DesEdeEngine());
			}
			if (text2 == "DESEDEMAC")
			{
				return new CbcBlockCipherMac(new DesEdeEngine());
			}
			if (text2 == "DESEDEMAC/CFB8")
			{
				return new CfbBlockCipherMac(new DesEdeEngine());
			}
			if (text2 == "DESEDEMAC64")
			{
				return new CbcBlockCipherMac(new DesEdeEngine(), 64);
			}
			if (text2 == "DESEDEMAC64WITHISO7816-4PADDING")
			{
				return new CbcBlockCipherMac(new DesEdeEngine(), 64, new ISO7816d4Padding());
			}
			if (text2 == "DESWITHISO9797" || text2 == "ISO9797ALG3MAC")
			{
				return new ISO9797Alg3Mac(new DesEngine());
			}
			if (text2 == "ISO9797ALG3WITHISO7816-4PADDING")
			{
				return new ISO9797Alg3Mac(new DesEngine(), new ISO7816d4Padding());
			}
			if (text2 == "SKIPJACKMAC")
			{
				return new CbcBlockCipherMac(new SkipjackEngine());
			}
			if (text2 == "SKIPJACKMAC/CFB8")
			{
				return new CfbBlockCipherMac(new SkipjackEngine());
			}
			if (text2 == "IDEAMAC")
			{
				return new CbcBlockCipherMac(new IdeaEngine());
			}
			if (text2 == "IDEAMAC/CFB8")
			{
				return new CfbBlockCipherMac(new IdeaEngine());
			}
			if (text2 == "RC2MAC")
			{
				return new CbcBlockCipherMac(new RC2Engine());
			}
			if (text2 == "RC2MAC/CFB8")
			{
				return new CfbBlockCipherMac(new RC2Engine());
			}
			if (text2 == "RC5MAC")
			{
				return new CbcBlockCipherMac(new RC532Engine());
			}
			if (text2 == "RC5MAC/CFB8")
			{
				return new CfbBlockCipherMac(new RC532Engine());
			}
			if (text2 == "GOST28147MAC")
			{
				return new Gost28147Mac();
			}
			if (text2 == "VMPCMAC")
			{
				return new VmpcMac();
			}
			if (text2 == "SIPHASH-2-4")
			{
				return new SipHash();
			}
			throw new SecurityUtilityException("Mac " + text2 + " not recognised.");
		}

		public static string GetAlgorithmName(DerObjectIdentifier oid)
		{
			return (string)algorithms[oid.Id];
		}

		public static byte[] DoFinal(IMac mac)
		{
			byte[] array = new byte[mac.GetMacSize()];
			mac.DoFinal(array, 0);
			return array;
		}

		public static byte[] DoFinal(IMac mac, byte[] input)
		{
			mac.BlockUpdate(input, 0, input.Length);
			return DoFinal(mac);
		}
	}
}
