namespace Org.BouncyCastle.Asn1.X9
{
	internal class X962Parameters : Asn1Encodable, IAsn1Choice
	{
		private readonly Asn1Object _params;

		public bool IsNamedCurve => _params is DerObjectIdentifier;

		public Asn1Object Parameters => _params;

		public X962Parameters(X9ECParameters ecParameters)
		{
			_params = ecParameters.ToAsn1Object();
		}

		public X962Parameters(DerObjectIdentifier namedCurve)
		{
			_params = namedCurve;
		}

		public X962Parameters(Asn1Object obj)
		{
			_params = obj;
		}

		public override Asn1Object ToAsn1Object()
		{
			return _params;
		}
	}
}
