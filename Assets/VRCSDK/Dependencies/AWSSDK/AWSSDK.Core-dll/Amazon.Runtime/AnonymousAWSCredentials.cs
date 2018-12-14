using System;

namespace Amazon.Runtime
{
	public class AnonymousAWSCredentials : AWSCredentials
	{
		public override ImmutableCredentials GetCredentials()
		{
			throw new NotSupportedException("AnonymousAWSCredentials do not support this operation");
		}
	}
}
