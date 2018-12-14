using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class GetBucketRequestPaymentResponse : AmazonWebServiceResponse
	{
		private string payer;

		public string Payer
		{
			get
			{
				return payer;
			}
			set
			{
				payer = value;
			}
		}

		internal bool IsSetPayer()
		{
			return payer != null;
		}

		public GetBucketRequestPaymentResponse()
			: this()
		{
		}
	}
}
