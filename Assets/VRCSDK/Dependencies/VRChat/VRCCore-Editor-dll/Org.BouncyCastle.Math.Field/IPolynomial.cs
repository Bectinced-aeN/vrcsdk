namespace Org.BouncyCastle.Math.Field
{
	internal interface IPolynomial
	{
		int Degree
		{
			get;
		}

		int[] GetExponentsPresent();
	}
}
