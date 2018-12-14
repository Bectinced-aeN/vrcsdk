using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.CryptoPro;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Parameters
{
	internal abstract class ECKeyParameters : AsymmetricKeyParameter
	{
		private static readonly string[] algorithms = new string[6]
		{
			"EC",
			"ECDSA",
			"ECDH",
			"ECDHC",
			"ECGOST3410",
			"ECMQV"
		};

		private readonly string algorithm;

		private readonly ECDomainParameters parameters;

		private readonly DerObjectIdentifier publicKeyParamSet;

		public string AlgorithmName => algorithm;

		public ECDomainParameters Parameters => parameters;

		public DerObjectIdentifier PublicKeyParamSet => publicKeyParamSet;

		protected ECKeyParameters(string algorithm, bool isPrivate, ECDomainParameters parameters)
			: base(isPrivate)
		{
			if (algorithm == null)
			{
				throw new ArgumentNullException("algorithm");
			}
			if (parameters == null)
			{
				throw new ArgumentNullException("parameters");
			}
			this.algorithm = VerifyAlgorithmName(algorithm);
			this.parameters = parameters;
		}

		protected ECKeyParameters(string algorithm, bool isPrivate, DerObjectIdentifier publicKeyParamSet)
			: base(isPrivate)
		{
			if (algorithm == null)
			{
				throw new ArgumentNullException("algorithm");
			}
			if (publicKeyParamSet == null)
			{
				throw new ArgumentNullException("publicKeyParamSet");
			}
			this.algorithm = VerifyAlgorithmName(algorithm);
			parameters = LookupParameters(publicKeyParamSet);
			this.publicKeyParamSet = publicKeyParamSet;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			ECDomainParameters eCDomainParameters = obj as ECDomainParameters;
			if (eCDomainParameters == null)
			{
				return false;
			}
			return Equals(eCDomainParameters);
		}

		protected bool Equals(ECKeyParameters other)
		{
			return parameters.Equals(other.parameters) && Equals((AsymmetricKeyParameter)other);
		}

		public override int GetHashCode()
		{
			return parameters.GetHashCode() ^ base.GetHashCode();
		}

		internal ECKeyGenerationParameters CreateKeyGenerationParameters(SecureRandom random)
		{
			if (publicKeyParamSet != null)
			{
				return new ECKeyGenerationParameters(publicKeyParamSet, random);
			}
			return new ECKeyGenerationParameters(parameters, random);
		}

		internal static string VerifyAlgorithmName(string algorithm)
		{
			string result = Platform.ToUpperInvariant(algorithm);
			if (Array.IndexOf(algorithms, algorithm, 0, algorithms.Length) < 0)
			{
				throw new ArgumentException("unrecognised algorithm: " + algorithm, "algorithm");
			}
			return result;
		}

		internal static ECDomainParameters LookupParameters(DerObjectIdentifier publicKeyParamSet)
		{
			if (publicKeyParamSet == null)
			{
				throw new ArgumentNullException("publicKeyParamSet");
			}
			ECDomainParameters eCDomainParameters = ECGost3410NamedCurves.GetByOid(publicKeyParamSet);
			if (eCDomainParameters == null)
			{
				X9ECParameters x9ECParameters = ECKeyPairGenerator.FindECCurveByOid(publicKeyParamSet);
				if (x9ECParameters == null)
				{
					throw new ArgumentException("OID is not a valid public key parameter set", "publicKeyParamSet");
				}
				eCDomainParameters = new ECDomainParameters(x9ECParameters.Curve, x9ECParameters.G, x9ECParameters.N, x9ECParameters.H, x9ECParameters.GetSeed());
			}
			return eCDomainParameters;
		}
	}
}
