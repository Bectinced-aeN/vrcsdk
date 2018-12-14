using Amazon.Runtime.Internal;
using System;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class S3ErrorResponse : ErrorResponse
	{
		internal string Region
		{
			get;
			set;
		}

		public string Resource
		{
			get;
			set;
		}

		public string Id2
		{
			get;
			set;
		}

		public string AmzCfId
		{
			get;
			set;
		}

		public Exception ParsingException
		{
			get;
			set;
		}

		public S3ErrorResponse()
			: this()
		{
		}
	}
}
