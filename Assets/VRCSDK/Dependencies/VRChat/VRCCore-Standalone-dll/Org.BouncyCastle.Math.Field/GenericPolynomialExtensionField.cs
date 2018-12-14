using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Math.Field
{
	internal class GenericPolynomialExtensionField : IExtensionField, IFiniteField, IPolynomialExtensionField
	{
		protected readonly IFiniteField subfield;

		protected readonly IPolynomial minimalPolynomial;

		public virtual BigInteger Characteristic => subfield.Characteristic;

		public virtual int Dimension => subfield.Dimension * minimalPolynomial.Degree;

		public virtual IFiniteField Subfield => subfield;

		public virtual int Degree => minimalPolynomial.Degree;

		public virtual IPolynomial MinimalPolynomial => minimalPolynomial;

		internal GenericPolynomialExtensionField(IFiniteField subfield, IPolynomial polynomial)
		{
			this.subfield = subfield;
			minimalPolynomial = polynomial;
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			GenericPolynomialExtensionField genericPolynomialExtensionField = obj as GenericPolynomialExtensionField;
			if (genericPolynomialExtensionField == null)
			{
				return false;
			}
			return subfield.Equals(genericPolynomialExtensionField.subfield) && minimalPolynomial.Equals(genericPolynomialExtensionField.minimalPolynomial);
		}

		public override int GetHashCode()
		{
			return subfield.GetHashCode() ^ Integers.RotateLeft(minimalPolynomial.GetHashCode(), 16);
		}
	}
}
