using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Math.Field;
using System;

namespace Org.BouncyCastle.Asn1.X9
{
	internal class X9ECParameters : Asn1Encodable
	{
		private X9FieldID fieldID;

		private ECCurve curve;

		private ECPoint g;

		private BigInteger n;

		private BigInteger h;

		private byte[] seed;

		public ECCurve Curve => curve;

		public ECPoint G => g;

		public BigInteger N => n;

		public BigInteger H
		{
			get
			{
				if (h == null)
				{
					return BigInteger.One;
				}
				return h;
			}
		}

		public X9ECParameters(Asn1Sequence seq)
		{
			if (!(seq[0] is DerInteger) || !((DerInteger)seq[0]).Value.Equals(BigInteger.One))
			{
				throw new ArgumentException("bad version in X9ECParameters");
			}
			X9Curve x9Curve = null;
			x9Curve = ((!(seq[2] is X9Curve)) ? new X9Curve(new X9FieldID((Asn1Sequence)seq[1]), (Asn1Sequence)seq[2]) : ((X9Curve)seq[2]));
			curve = x9Curve.Curve;
			if (seq[3] is X9ECPoint)
			{
				g = ((X9ECPoint)seq[3]).Point;
			}
			else
			{
				g = new X9ECPoint(curve, (Asn1OctetString)seq[3]).Point;
			}
			n = ((DerInteger)seq[4]).Value;
			seed = x9Curve.GetSeed();
			if (seq.Count == 6)
			{
				h = ((DerInteger)seq[5]).Value;
			}
		}

		public X9ECParameters(ECCurve curve, ECPoint g, BigInteger n)
			: this(curve, g, n, BigInteger.One, null)
		{
		}

		public X9ECParameters(ECCurve curve, ECPoint g, BigInteger n, BigInteger h)
			: this(curve, g, n, h, null)
		{
		}

		public X9ECParameters(ECCurve curve, ECPoint g, BigInteger n, BigInteger h, byte[] seed)
		{
			this.curve = curve;
			this.g = g.Normalize();
			this.n = n;
			this.h = h;
			this.seed = seed;
			if (ECAlgorithms.IsFpCurve(curve))
			{
				fieldID = new X9FieldID(curve.Field.Characteristic);
			}
			else
			{
				if (!ECAlgorithms.IsF2mCurve(curve))
				{
					throw new ArgumentException("'curve' is of an unsupported type");
				}
				IPolynomialExtensionField polynomialExtensionField = (IPolynomialExtensionField)curve.Field;
				int[] exponentsPresent = polynomialExtensionField.MinimalPolynomial.GetExponentsPresent();
				if (exponentsPresent.Length == 3)
				{
					fieldID = new X9FieldID(exponentsPresent[2], exponentsPresent[1]);
				}
				else
				{
					if (exponentsPresent.Length != 5)
					{
						throw new ArgumentException("Only trinomial and pentomial curves are supported");
					}
					fieldID = new X9FieldID(exponentsPresent[4], exponentsPresent[1], exponentsPresent[2], exponentsPresent[3]);
				}
			}
		}

		public byte[] GetSeed()
		{
			return seed;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new DerInteger(1), fieldID, new X9Curve(curve, seed), new X9ECPoint(g), new DerInteger(n));
			if (h != null)
			{
				asn1EncodableVector.Add(new DerInteger(h));
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
