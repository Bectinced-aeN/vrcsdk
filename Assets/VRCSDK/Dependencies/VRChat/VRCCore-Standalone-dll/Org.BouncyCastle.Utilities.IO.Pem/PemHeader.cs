namespace Org.BouncyCastle.Utilities.IO.Pem
{
	internal class PemHeader
	{
		private string name;

		private string val;

		public virtual string Name => name;

		public virtual string Value => val;

		public PemHeader(string name, string val)
		{
			this.name = name;
			this.val = val;
		}

		public override int GetHashCode()
		{
			return GetHashCode(name) + 31 * GetHashCode(val);
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			if (!(obj is PemHeader))
			{
				return false;
			}
			PemHeader pemHeader = (PemHeader)obj;
			return object.Equals(name, pemHeader.name) && object.Equals(val, pemHeader.val);
		}

		private int GetHashCode(string s)
		{
			return s?.GetHashCode() ?? 1;
		}
	}
}
