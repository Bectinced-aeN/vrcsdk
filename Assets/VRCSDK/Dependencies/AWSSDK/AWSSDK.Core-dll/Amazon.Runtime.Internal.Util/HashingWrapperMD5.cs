using ThirdParty.MD5;

namespace Amazon.Runtime.Internal.Util
{
	public class HashingWrapperMD5 : HashingWrapper
	{
		public HashingWrapperMD5()
			: base(typeof(MD5Managed).FullName)
		{
		}
	}
}
