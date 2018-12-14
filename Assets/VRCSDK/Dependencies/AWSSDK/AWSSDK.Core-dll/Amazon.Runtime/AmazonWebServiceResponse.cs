using System;
using System.Net;

namespace Amazon.Runtime
{
	[Serializable]
	public class AmazonWebServiceResponse
	{
		private ResponseMetadata responseMetadataField;

		private long contentLength;

		private HttpStatusCode httpStatusCode;

		public ResponseMetadata ResponseMetadata
		{
			get
			{
				return responseMetadataField;
			}
			set
			{
				responseMetadataField = value;
			}
		}

		public long ContentLength
		{
			get
			{
				return contentLength;
			}
			set
			{
				contentLength = value;
			}
		}

		public HttpStatusCode HttpStatusCode
		{
			get
			{
				return httpStatusCode;
			}
			set
			{
				httpStatusCode = value;
			}
		}
	}
}
