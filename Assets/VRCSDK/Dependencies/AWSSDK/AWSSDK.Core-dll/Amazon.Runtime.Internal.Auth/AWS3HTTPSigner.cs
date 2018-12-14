namespace Amazon.Runtime.Internal.Auth
{
	internal class AWS3HTTPSigner : AWS3Signer
	{
		public AWS3HTTPSigner()
			: base(useAws3Https: false)
		{
		}
	}
}
