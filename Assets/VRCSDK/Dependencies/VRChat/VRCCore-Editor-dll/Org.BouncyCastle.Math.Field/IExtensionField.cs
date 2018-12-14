namespace Org.BouncyCastle.Math.Field
{
	internal interface IExtensionField : IFiniteField
	{
		IFiniteField Subfield
		{
			get;
		}

		int Degree
		{
			get;
		}
	}
}
