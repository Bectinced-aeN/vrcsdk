using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class GetLifecycleConfigurationResponse : AmazonWebServiceResponse
	{
		private LifecycleConfiguration configuration;

		public LifecycleConfiguration Configuration
		{
			get
			{
				if (configuration == null)
				{
					configuration = new LifecycleConfiguration();
				}
				return configuration;
			}
			set
			{
				configuration = value;
			}
		}

		public GetLifecycleConfigurationResponse()
			: this()
		{
		}
	}
}
