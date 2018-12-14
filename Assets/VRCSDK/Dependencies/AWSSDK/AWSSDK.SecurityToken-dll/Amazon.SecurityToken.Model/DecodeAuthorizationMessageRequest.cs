namespace Amazon.SecurityToken.Model
{
	public class DecodeAuthorizationMessageRequest : AmazonSecurityTokenServiceRequest
	{
		private string _encodedMessage;

		public string EncodedMessage
		{
			get
			{
				return _encodedMessage;
			}
			set
			{
				_encodedMessage = value;
			}
		}

		internal bool IsSetEncodedMessage()
		{
			return _encodedMessage != null;
		}
	}
}
