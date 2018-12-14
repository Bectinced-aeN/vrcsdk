using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Misc;
using Org.BouncyCastle.Asn1.Utilities;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.X509.Extension;
using System;
using System.Collections;
using System.Text;

namespace Org.BouncyCastle.X509
{
	internal class X509Certificate : X509ExtensionBase
	{
		private readonly X509CertificateStructure c;

		private readonly BasicConstraints basicConstraints;

		private readonly bool[] keyUsage;

		private bool hashValueSet;

		private int hashValue;

		public virtual X509CertificateStructure CertificateStructure => c;

		public virtual bool IsValidNow => IsValid(DateTime.UtcNow);

		public virtual int Version => c.Version;

		public virtual BigInteger SerialNumber => c.SerialNumber.Value;

		public virtual X509Name IssuerDN => c.Issuer;

		public virtual X509Name SubjectDN => c.Subject;

		public virtual DateTime NotBefore => c.StartDate.ToDateTime();

		public virtual DateTime NotAfter => c.EndDate.ToDateTime();

		public virtual string SigAlgName => SignerUtilities.GetEncodingName(c.SignatureAlgorithm.ObjectID);

		public virtual string SigAlgOid => c.SignatureAlgorithm.ObjectID.Id;

		public virtual DerBitString IssuerUniqueID => c.TbsCertificate.IssuerUniqueID;

		public virtual DerBitString SubjectUniqueID => c.TbsCertificate.SubjectUniqueID;

		protected X509Certificate()
		{
		}

		public X509Certificate(X509CertificateStructure c)
		{
			this.c = c;
			try
			{
				Asn1OctetString extensionValue = GetExtensionValue(new DerObjectIdentifier("2.5.29.19"));
				if (extensionValue != null)
				{
					basicConstraints = BasicConstraints.GetInstance(X509ExtensionUtilities.FromExtensionValue(extensionValue));
				}
			}
			catch (Exception arg)
			{
				throw new CertificateParsingException("cannot construct BasicConstraints: " + arg);
				IL_004c:;
			}
			try
			{
				Asn1OctetString extensionValue2 = GetExtensionValue(new DerObjectIdentifier("2.5.29.15"));
				if (extensionValue2 != null)
				{
					DerBitString instance = DerBitString.GetInstance(X509ExtensionUtilities.FromExtensionValue(extensionValue2));
					byte[] bytes = instance.GetBytes();
					int num = bytes.Length * 8 - instance.PadBits;
					keyUsage = new bool[(num >= 9) ? num : 9];
					for (int i = 0; i != num; i++)
					{
						keyUsage[i] = ((bytes[i / 8] & (128 >> i % 8)) != 0);
					}
				}
				else
				{
					keyUsage = null;
				}
			}
			catch (Exception arg2)
			{
				throw new CertificateParsingException("cannot construct KeyUsage: " + arg2);
				IL_0108:;
			}
		}

		public virtual bool IsValid(DateTime time)
		{
			return time.CompareTo(NotBefore) >= 0 && time.CompareTo(NotAfter) <= 0;
		}

		public virtual void CheckValidity()
		{
			CheckValidity(DateTime.UtcNow);
		}

		public virtual void CheckValidity(DateTime time)
		{
			if (time.CompareTo(NotAfter) > 0)
			{
				throw new CertificateExpiredException("certificate expired on " + c.EndDate.GetTime());
			}
			if (time.CompareTo(NotBefore) < 0)
			{
				throw new CertificateNotYetValidException("certificate not valid until " + c.StartDate.GetTime());
			}
		}

		public virtual byte[] GetTbsCertificate()
		{
			return c.TbsCertificate.GetDerEncoded();
		}

		public virtual byte[] GetSignature()
		{
			return c.Signature.GetBytes();
		}

		public virtual byte[] GetSigAlgParams()
		{
			if (c.SignatureAlgorithm.Parameters != null)
			{
				return c.SignatureAlgorithm.Parameters.GetDerEncoded();
			}
			return null;
		}

		public virtual bool[] GetKeyUsage()
		{
			return (keyUsage != null) ? ((bool[])keyUsage.Clone()) : null;
		}

		public virtual IList GetExtendedKeyUsage()
		{
			Asn1OctetString extensionValue = GetExtensionValue(new DerObjectIdentifier("2.5.29.37"));
			if (extensionValue != null)
			{
				try
				{
					Asn1Sequence instance = Asn1Sequence.GetInstance(X509ExtensionUtilities.FromExtensionValue(extensionValue));
					IList list = Platform.CreateArrayList();
					foreach (DerObjectIdentifier item in instance)
					{
						list.Add(item.Id);
					}
					return list;
					IL_0081:
					IList result;
					return result;
				}
				catch (Exception exception)
				{
					throw new CertificateParsingException("error processing extended key usage extension", exception);
					IL_0095:
					IList result;
					return result;
				}
			}
			return null;
		}

		public virtual int GetBasicConstraints()
		{
			if (basicConstraints != null && basicConstraints.IsCA())
			{
				if (basicConstraints.PathLenConstraint == null)
				{
					return 2147483647;
				}
				return basicConstraints.PathLenConstraint.IntValue;
			}
			return -1;
		}

		public virtual ICollection GetSubjectAlternativeNames()
		{
			return GetAlternativeNames("2.5.29.17");
		}

		public virtual ICollection GetIssuerAlternativeNames()
		{
			return GetAlternativeNames("2.5.29.18");
		}

		protected virtual ICollection GetAlternativeNames(string oid)
		{
			Asn1OctetString extensionValue = GetExtensionValue(new DerObjectIdentifier(oid));
			if (extensionValue == null)
			{
				return null;
			}
			Asn1Object obj = X509ExtensionUtilities.FromExtensionValue(extensionValue);
			GeneralNames instance = GeneralNames.GetInstance(obj);
			IList list = Platform.CreateArrayList();
			GeneralName[] names = instance.GetNames();
			foreach (GeneralName generalName in names)
			{
				IList list2 = Platform.CreateArrayList();
				list2.Add(generalName.TagNo);
				list2.Add(generalName.Name.ToString());
				list.Add(list2);
			}
			return list;
		}

		protected override X509Extensions GetX509Extensions()
		{
			return (c.Version != 3) ? null : c.TbsCertificate.Extensions;
		}

		public virtual AsymmetricKeyParameter GetPublicKey()
		{
			return PublicKeyFactory.CreateKey(c.SubjectPublicKeyInfo);
		}

		public virtual byte[] GetEncoded()
		{
			return c.GetDerEncoded();
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			X509Certificate x509Certificate = obj as X509Certificate;
			if (x509Certificate == null)
			{
				return false;
			}
			return c.Equals(x509Certificate.c);
		}

		public override int GetHashCode()
		{
			lock (this)
			{
				if (!hashValueSet)
				{
					hashValue = c.GetHashCode();
					hashValueSet = true;
				}
			}
			return hashValue;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			string newLine = Platform.NewLine;
			stringBuilder.Append("  [0]         Version: ").Append(Version).Append(newLine);
			stringBuilder.Append("         SerialNumber: ").Append(SerialNumber).Append(newLine);
			stringBuilder.Append("             IssuerDN: ").Append(IssuerDN).Append(newLine);
			stringBuilder.Append("           Start Date: ").Append(NotBefore).Append(newLine);
			stringBuilder.Append("           Final Date: ").Append(NotAfter).Append(newLine);
			stringBuilder.Append("            SubjectDN: ").Append(SubjectDN).Append(newLine);
			stringBuilder.Append("           Public Key: ").Append(GetPublicKey()).Append(newLine);
			stringBuilder.Append("  Signature Algorithm: ").Append(SigAlgName).Append(newLine);
			byte[] signature = GetSignature();
			stringBuilder.Append("            Signature: ").Append(Hex.ToHexString(signature, 0, 20)).Append(newLine);
			for (int i = 20; i < signature.Length; i += 20)
			{
				int length = System.Math.Min(20, signature.Length - i);
				stringBuilder.Append("                       ").Append(Hex.ToHexString(signature, i, length)).Append(newLine);
			}
			X509Extensions extensions = c.TbsCertificate.Extensions;
			if (extensions != null)
			{
				IEnumerator enumerator = extensions.ExtensionOids.GetEnumerator();
				if (enumerator.MoveNext())
				{
					stringBuilder.Append("       Extensions: \n");
				}
				do
				{
					DerObjectIdentifier derObjectIdentifier = (DerObjectIdentifier)enumerator.Current;
					X509Extension extension = extensions.GetExtension(derObjectIdentifier);
					if (extension.Value != null)
					{
						byte[] octets = extension.Value.GetOctets();
						Asn1Object asn1Object = Asn1Object.FromByteArray(octets);
						stringBuilder.Append("                       critical(").Append(extension.IsCritical).Append(") ");
						try
						{
							if (derObjectIdentifier.Equals(X509Extensions.BasicConstraints))
							{
								stringBuilder.Append(BasicConstraints.GetInstance(asn1Object));
							}
							else if (derObjectIdentifier.Equals(X509Extensions.KeyUsage))
							{
								stringBuilder.Append(KeyUsage.GetInstance(asn1Object));
							}
							else if (derObjectIdentifier.Equals(MiscObjectIdentifiers.NetscapeCertType))
							{
								stringBuilder.Append(new NetscapeCertType((DerBitString)asn1Object));
							}
							else if (derObjectIdentifier.Equals(MiscObjectIdentifiers.NetscapeRevocationUrl))
							{
								stringBuilder.Append(new NetscapeRevocationUrl((DerIA5String)asn1Object));
							}
							else if (derObjectIdentifier.Equals(MiscObjectIdentifiers.VerisignCzagExtension))
							{
								stringBuilder.Append(new VerisignCzagExtension((DerIA5String)asn1Object));
							}
							else
							{
								stringBuilder.Append(derObjectIdentifier.Id);
								stringBuilder.Append(" value = ").Append(Asn1Dump.DumpAsString(asn1Object));
							}
						}
						catch (Exception)
						{
							stringBuilder.Append(derObjectIdentifier.Id);
							stringBuilder.Append(" value = ").Append("*****");
						}
					}
					stringBuilder.Append(newLine);
				}
				while (enumerator.MoveNext());
			}
			return stringBuilder.ToString();
		}

		public virtual void Verify(AsymmetricKeyParameter key)
		{
			string signatureName = X509SignatureUtilities.GetSignatureName(c.SignatureAlgorithm);
			ISigner signer = SignerUtilities.GetSigner(signatureName);
			CheckSignature(key, signer);
		}

		protected virtual void CheckSignature(AsymmetricKeyParameter publicKey, ISigner signature)
		{
			if (!IsAlgIDEqual(c.SignatureAlgorithm, c.TbsCertificate.Signature))
			{
				throw new CertificateException("signature algorithm in TBS cert not same as outer cert");
			}
			Asn1Encodable parameters = c.SignatureAlgorithm.Parameters;
			X509SignatureUtilities.SetSignatureParameters(signature, parameters);
			signature.Init(forSigning: false, publicKey);
			byte[] tbsCertificate = GetTbsCertificate();
			signature.BlockUpdate(tbsCertificate, 0, tbsCertificate.Length);
			byte[] signature2 = GetSignature();
			if (!signature.VerifySignature(signature2))
			{
				throw new InvalidKeyException("Public key presented not for certificate signature");
			}
		}

		private static bool IsAlgIDEqual(AlgorithmIdentifier id1, AlgorithmIdentifier id2)
		{
			if (!id1.ObjectID.Equals(id2.ObjectID))
			{
				return false;
			}
			Asn1Encodable parameters = id1.Parameters;
			Asn1Encodable parameters2 = id2.Parameters;
			if (parameters == null == (parameters2 == null))
			{
				return object.Equals(parameters, parameters2);
			}
			return (parameters != null) ? (parameters.ToAsn1Object() is Asn1Null) : (parameters2.ToAsn1Object() is Asn1Null);
		}
	}
}
