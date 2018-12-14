using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Parameters
{
	internal class DHKeyGenerationParameters : KeyGenerationParameters
	{
		private readonly DHParameters parameters;

		public DHParameters Parameters => parameters;

		public DHKeyGenerationParameters(SecureRandom random, DHParameters parameters)
			: base(random, GetStrength(parameters))
		{
			this.parameters = parameters;
		}

		internal static int GetStrength(DHParameters parameters)
		{
			return (parameters.L == 0) ? parameters.P.BitLength : parameters.L;
		}
	}
}
