using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.IO;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
	internal class TlsECDheKeyExchange : TlsECDHKeyExchange
	{
		protected TlsSignerCredentials mServerCredentials;

		public TlsECDheKeyExchange(int keyExchange, IList supportedSignatureAlgorithms, int[] namedCurves, byte[] clientECPointFormats, byte[] serverECPointFormats)
			: base(keyExchange, supportedSignatureAlgorithms, namedCurves, clientECPointFormats, serverECPointFormats)
		{
		}

		public override void ProcessServerCredentials(TlsCredentials serverCredentials)
		{
			if (!(serverCredentials is TlsSignerCredentials))
			{
				throw new TlsFatalAlert(80);
			}
			ProcessServerCertificate(serverCredentials.Certificate);
			mServerCredentials = (TlsSignerCredentials)serverCredentials;
		}

		public override byte[] GenerateServerKeyExchange()
		{
			DigestInputBuffer digestInputBuffer = new DigestInputBuffer();
			mECAgreePrivateKey = TlsEccUtilities.GenerateEphemeralServerKeyExchange(mContext.SecureRandom, mNamedCurves, mClientECPointFormats, digestInputBuffer);
			SignatureAndHashAlgorithm signatureAndHashAlgorithm = TlsUtilities.GetSignatureAndHashAlgorithm(mContext, mServerCredentials);
			IDigest digest = TlsUtilities.CreateHash(signatureAndHashAlgorithm);
			SecurityParameters securityParameters = mContext.SecurityParameters;
			digest.BlockUpdate(securityParameters.clientRandom, 0, securityParameters.clientRandom.Length);
			digest.BlockUpdate(securityParameters.serverRandom, 0, securityParameters.serverRandom.Length);
			digestInputBuffer.UpdateDigest(digest);
			byte[] hash = DigestUtilities.DoFinal(digest);
			byte[] signature = mServerCredentials.GenerateCertificateSignature(hash);
			DigitallySigned digitallySigned = new DigitallySigned(signatureAndHashAlgorithm, signature);
			digitallySigned.Encode(digestInputBuffer);
			return digestInputBuffer.ToArray();
		}

		public override void ProcessServerKeyExchange(Stream input)
		{
			SecurityParameters securityParameters = mContext.SecurityParameters;
			SignerInputBuffer signerInputBuffer = new SignerInputBuffer();
			Stream input2 = new TeeInputStream(input, signerInputBuffer);
			ECDomainParameters curve_params = TlsEccUtilities.ReadECParameters(mNamedCurves, mClientECPointFormats, input2);
			byte[] encoding = TlsUtilities.ReadOpaque8(input2);
			DigitallySigned digitallySigned = DigitallySigned.Parse(mContext, input);
			ISigner signer = InitVerifyer(mTlsSigner, digitallySigned.Algorithm, securityParameters);
			signerInputBuffer.UpdateSigner(signer);
			if (!signer.VerifySignature(digitallySigned.Signature))
			{
				throw new TlsFatalAlert(51);
			}
			mECAgreePublicKey = TlsEccUtilities.ValidateECPublicKey(TlsEccUtilities.DeserializeECPublicKey(mClientECPointFormats, curve_params, encoding));
		}

		public override void ValidateCertificateRequest(CertificateRequest certificateRequest)
		{
			byte[] certificateTypes = certificateRequest.CertificateTypes;
			int num = 0;
			while (true)
			{
				if (num >= certificateTypes.Length)
				{
					return;
				}
				byte b = certificateTypes[num];
				if (b != 1 && b != 2 && b != 64)
				{
					break;
				}
				num++;
			}
			throw new TlsFatalAlert(47);
		}

		public override void ProcessClientCredentials(TlsCredentials clientCredentials)
		{
			if (!(clientCredentials is TlsSignerCredentials))
			{
				throw new TlsFatalAlert(80);
			}
		}

		protected virtual ISigner InitVerifyer(TlsSigner tlsSigner, SignatureAndHashAlgorithm algorithm, SecurityParameters securityParameters)
		{
			ISigner signer = tlsSigner.CreateVerifyer(algorithm, mServerPublicKey);
			signer.BlockUpdate(securityParameters.clientRandom, 0, securityParameters.clientRandom.Length);
			signer.BlockUpdate(securityParameters.serverRandom, 0, securityParameters.serverRandom.Length);
			return signer;
		}
	}
}
