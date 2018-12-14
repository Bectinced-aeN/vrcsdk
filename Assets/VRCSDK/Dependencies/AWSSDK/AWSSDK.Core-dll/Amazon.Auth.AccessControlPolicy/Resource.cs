namespace Amazon.Auth.AccessControlPolicy
{
	public class Resource
	{
		private string resource;

		public string Id => resource;

		public Resource(string resource)
		{
			this.resource = resource;
		}
	}
}
