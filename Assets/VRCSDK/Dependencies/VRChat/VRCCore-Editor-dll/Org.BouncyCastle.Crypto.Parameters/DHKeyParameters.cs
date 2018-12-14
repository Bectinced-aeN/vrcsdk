using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;

namespace Org.BouncyCastle.Crypto.Parameters
{
	internal class DHKeyParameters : AsymmetricKeyParameter
	{
		private readonly DHParameters parameters;

		private readonly DerObjectIdentifier algorithmOid;

		public DHParameters Parameters => parameters;

		public DerObjectIdentifier AlgorithmOid => algorithmOid;

		protected DHKeyParameters(bool isPrivate, DHParameters parameters)
			: this(isPrivate, parameters, PkcsObjectIdentifiers.DhKeyAgreement)
		{
		}

		protected DHKeyParameters(bool isPrivate, DHParameters parameters, DerObjectIdentifier algorithmOid)
			: base(isPrivate)
		{
			this.parameters = parameters;
			this.algorithmOid = algorithmOid;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			DHKeyParameters dHKeyParameters = obj as DHKeyParameters;
			if (dHKeyParameters == null)
			{
				return false;
			}
			return Equals(dHKeyParameters);
		}

		protected bool Equals(DHKeyParameters other)
		{
			return object.Equals(parameters, other.parameters) && Equals((AsymmetricKeyParameter)other);
		}

		public override int GetHashCode()
		{
			int num = base.GetHashCode();
			if (parameters != null)
			{
				num ^= parameters.GetHashCode();
			}
			return num;
		}
	}
}
