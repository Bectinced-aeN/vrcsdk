namespace Amazon.Runtime
{
	public abstract class AWSCredentials
	{
		public abstract ImmutableCredentials GetCredentials();

		protected virtual void Validate()
		{
		}
	}
}
