using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.IO;
using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.X509
{
	internal class X509CrlParser
	{
		private static readonly PemParser PemCrlParser = new PemParser("CRL");

		private readonly bool lazyAsn1;

		private Asn1Set sCrlData;

		private int sCrlDataObjectCount;

		private Stream currentCrlStream;

		public X509CrlParser()
			: this(lazyAsn1: false)
		{
		}

		public X509CrlParser(bool lazyAsn1)
		{
			this.lazyAsn1 = lazyAsn1;
		}

		private X509Crl ReadPemCrl(Stream inStream)
		{
			Asn1Sequence asn1Sequence = PemCrlParser.ReadPemObject(inStream);
			return (asn1Sequence != null) ? CreateX509Crl(CertificateList.GetInstance(asn1Sequence)) : null;
		}

		private X509Crl ReadDerCrl(Asn1InputStream dIn)
		{
			Asn1Sequence asn1Sequence = (Asn1Sequence)dIn.ReadObject();
			if (asn1Sequence.Count > 1 && asn1Sequence[0] is DerObjectIdentifier && asn1Sequence[0].Equals(PkcsObjectIdentifiers.SignedData))
			{
				sCrlData = SignedData.GetInstance(Asn1Sequence.GetInstance((Asn1TaggedObject)asn1Sequence[1], explicitly: true)).Crls;
				return GetCrl();
			}
			return CreateX509Crl(CertificateList.GetInstance(asn1Sequence));
		}

		private X509Crl GetCrl()
		{
			if (sCrlData == null || sCrlDataObjectCount >= sCrlData.Count)
			{
				return null;
			}
			return CreateX509Crl(CertificateList.GetInstance(sCrlData[sCrlDataObjectCount++]));
		}

		protected virtual X509Crl CreateX509Crl(CertificateList c)
		{
			return new X509Crl(c);
		}

		public X509Crl ReadCrl(byte[] input)
		{
			return ReadCrl(new MemoryStream(input, writable: false));
		}

		public ICollection ReadCrls(byte[] input)
		{
			return ReadCrls(new MemoryStream(input, writable: false));
		}

		public X509Crl ReadCrl(Stream inStream)
		{
			if (inStream == null)
			{
				throw new ArgumentNullException("inStream");
			}
			if (inStream.CanRead)
			{
				if (currentCrlStream == null)
				{
					currentCrlStream = inStream;
					sCrlData = null;
					sCrlDataObjectCount = 0;
				}
				else if (currentCrlStream != inStream)
				{
					currentCrlStream = inStream;
					sCrlData = null;
					sCrlDataObjectCount = 0;
				}
				try
				{
					if (sCrlData != null)
					{
						if (sCrlDataObjectCount != sCrlData.Count)
						{
							return GetCrl();
						}
						sCrlData = null;
						sCrlDataObjectCount = 0;
						return null;
					}
					PushbackStream pushbackStream = new PushbackStream(inStream);
					int num = pushbackStream.ReadByte();
					if (num < 0)
					{
						return null;
					}
					pushbackStream.Unread(num);
					if (num != 48)
					{
						return ReadPemCrl(pushbackStream);
					}
					Asn1InputStream dIn = (!lazyAsn1) ? new Asn1InputStream(pushbackStream) : new LazyAsn1InputStream(pushbackStream);
					return ReadDerCrl(dIn);
					IL_011b:
					X509Crl result;
					return result;
				}
				catch (CrlException ex)
				{
					throw ex;
					IL_0123:
					X509Crl result;
					return result;
				}
				catch (Exception ex2)
				{
					throw new CrlException(ex2.ToString());
					IL_0137:
					X509Crl result;
					return result;
				}
			}
			throw new ArgumentException("inStream must be read-able", "inStream");
		}

		public ICollection ReadCrls(Stream inStream)
		{
			IList list = Platform.CreateArrayList();
			X509Crl value;
			while ((value = ReadCrl(inStream)) != null)
			{
				list.Add(value);
			}
			return list;
		}
	}
}
