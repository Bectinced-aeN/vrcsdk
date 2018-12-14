using Amazon.Runtime;
using System.Collections.Generic;

namespace Amazon.S3.Model
{
	public class GetObjectTaggingResponse : AmazonWebServiceResponse
	{
		private List<Tag> tagging = new List<Tag>();

		public List<Tag> Tagging
		{
			get
			{
				return tagging;
			}
			set
			{
				tagging = value;
			}
		}

		public GetObjectTaggingResponse()
			: this()
		{
		}
	}
}
