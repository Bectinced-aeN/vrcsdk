using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class GetCORSConfigurationResponse : AmazonWebServiceResponse
	{
		private CORSConfiguration configuration;

		public CORSConfiguration Configuration
		{
			get
			{
				return configuration;
			}
			set
			{
				configuration = value;
			}
		}

		internal bool IsSetConfiguration()
		{
			return configuration != null;
		}

		public GetCORSConfigurationResponse()
			: this()
		{
		}
	}
}
