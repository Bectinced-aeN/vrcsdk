namespace Org.BouncyCastle.Utilities
{
	internal interface IMemoable
	{
		IMemoable Copy();

		void Reset(IMemoable other);
	}
}
