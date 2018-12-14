namespace Amazon.S3.Model
{
	public class RequestPaymentConfiguration
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
	}
}
