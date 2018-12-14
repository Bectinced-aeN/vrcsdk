namespace Org.BouncyCastle.Crypto.Tls
{
	internal abstract class ECBasisType
	{
		public const byte ec_basis_trinomial = 1;

		public const byte ec_basis_pentanomial = 2;

		public static bool IsValid(byte ecBasisType)
		{
			return ecBasisType >= 1 && ecBasisType <= 2;
		}
	}
}
