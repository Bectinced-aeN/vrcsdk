using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Parameters
{
	internal class ElGamalKeyGenerationParameters : KeyGenerationParameters
	{
		private readonly ElGamalParameters parameters;

		public ElGamalParameters Parameters => parameters;

		public ElGamalKeyGenerationParameters(SecureRandom random, ElGamalParameters parameters)
			: base(random, GetStrength(parameters))
		{
			this.parameters = parameters;
		}

		internal static int GetStrength(ElGamalParameters parameters)
		{
			return (parameters.L == 0) ? parameters.P.BitLength : parameters.L;
		}
	}
}
