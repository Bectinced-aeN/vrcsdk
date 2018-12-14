namespace Amazon.S3.Model
{
	public class ResponseHeaderOverrides
	{
		internal const string RESPONSE_CONTENT_TYPE = "response-content-type";

		internal const string RESPONSE_CONTENT_LANGUAGE = "response-content-language";

		internal const string RESPONSE_EXPIRES = "response-expires";

		internal const string RESPONSE_CACHE_CONTROL = "response-cache-control";

		internal const string RESPONSE_CONTENT_DISPOSITION = "response-content-disposition";

		internal const string RESPONSE_CONTENT_ENCODING = "response-content-encoding";

		private string _contentType;

		private string _contentLanguage;

		private string _expires;

		private string _cacheControl;

		private string _contentDisposition;

		private string _contentEncoding;

		public string ContentType
		{
			get
			{
				return _contentType;
			}
			set
			{
				_contentType = value;
			}
		}

		public string ContentLanguage
		{
			get
			{
				return _contentLanguage;
			}
			set
			{
				_contentLanguage = value;
			}
		}

		public string Expires
		{
			get
			{
				return _expires;
			}
			set
			{
				_expires = value;
			}
		}

		public string CacheControl
		{
			get
			{
				return _cacheControl;
			}
			set
			{
				_cacheControl = value;
			}
		}

		public string ContentDisposition
		{
			get
			{
				return _contentDisposition;
			}
			set
			{
				_contentDisposition = value;
			}
		}

		public string ContentEncoding
		{
			get
			{
				return _contentEncoding;
			}
			set
			{
				_contentEncoding = value;
			}
		}
	}
}
