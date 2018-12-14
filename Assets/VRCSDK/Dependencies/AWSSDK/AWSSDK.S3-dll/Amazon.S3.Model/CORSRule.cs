using System.Collections.Generic;

namespace Amazon.S3.Model
{
	public class CORSRule
	{
		private string id;

		private List<string> allowedMethods = new List<string>();

		private List<string> allowedOrigins = new List<string>();

		private List<string> exposeHeaders = new List<string>();

		private List<string> allowedHeaders = new List<string>();

		private int? maxAgeSeconds;

		public List<string> AllowedMethods
		{
			get
			{
				return allowedMethods;
			}
			set
			{
				allowedMethods = value;
			}
		}

		public List<string> AllowedOrigins
		{
			get
			{
				return allowedOrigins;
			}
			set
			{
				allowedOrigins = value;
			}
		}

		public string Id
		{
			get
			{
				return id;
			}
			set
			{
				id = value;
			}
		}

		public List<string> ExposeHeaders
		{
			get
			{
				return exposeHeaders;
			}
			set
			{
				exposeHeaders = value;
			}
		}

		public int MaxAgeSeconds
		{
			get
			{
				return maxAgeSeconds ?? 0;
			}
			set
			{
				maxAgeSeconds = value;
			}
		}

		public List<string> AllowedHeaders
		{
			get
			{
				return allowedHeaders;
			}
			set
			{
				allowedHeaders = value;
			}
		}

		internal bool IsSetAllowedMethods()
		{
			return allowedMethods.Count > 0;
		}

		internal bool IsSetAllowedOrigins()
		{
			return allowedOrigins.Count > 0;
		}

		internal bool IsSetId()
		{
			return id != null;
		}

		internal bool IsSetExposeHeaders()
		{
			return exposeHeaders.Count > 0;
		}

		internal bool IsSetMaxAgeSeconds()
		{
			return maxAgeSeconds.HasValue;
		}

		internal bool IsSetAllowedHeaders()
		{
			return AllowedHeaders.Count > 0;
		}
	}
}
