using Amazon.Runtime;

namespace Amazon.SecurityToken.Model
{
	public class DecodeAuthorizationMessageResponse : AmazonWebServiceResponse
	{
		private string _decodedMessage;

		public string DecodedMessage
		{
			get
			{
				return _decodedMessage;
			}
			set
			{
				_decodedMessage = value;
			}
		}

		internal bool IsSetDecodedMessage()
		{
			return _decodedMessage != null;
		}

		public DecodeAuthorizationMessageResponse()
			: this()
		{
		}
	}
}
