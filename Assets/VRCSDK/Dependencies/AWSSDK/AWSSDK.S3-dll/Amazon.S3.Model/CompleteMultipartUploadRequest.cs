using Amazon.Runtime;
using System.Collections.Generic;

namespace Amazon.S3.Model
{
	public class CompleteMultipartUploadRequest : AmazonWebServiceRequest
	{
		private string bucketName;

		private string key;

		private List<PartETag> partETags = new List<PartETag>();

		private string uploadId;

		private RequestPayer requestPayer;

		public string BucketName
		{
			get
			{
				return bucketName;
			}
			set
			{
				bucketName = value;
			}
		}

		public string Key
		{
			get
			{
				return key;
			}
			set
			{
				key = value;
			}
		}

		public List<PartETag> PartETags
		{
			get
			{
				if (partETags == null)
				{
					partETags = new List<PartETag>();
				}
				return partETags;
			}
			set
			{
				partETags = value;
			}
		}

		public string UploadId
		{
			get
			{
				return uploadId;
			}
			set
			{
				uploadId = value;
			}
		}

		public RequestPayer RequestPayer
		{
			get
			{
				return requestPayer;
			}
			set
			{
				requestPayer = value;
			}
		}

		internal bool IsSetBucketName()
		{
			return bucketName != null;
		}

		internal bool IsSetKey()
		{
			return key != null;
		}

		public void AddPartETags(params PartETag[] partETags)
		{
			foreach (PartETag item in partETags)
			{
				PartETags.Add(item);
			}
		}

		public void AddPartETags(IEnumerable<PartETag> partETags)
		{
			foreach (PartETag partETag in partETags)
			{
				PartETags.Add(partETag);
			}
		}

		public void AddPartETags(params UploadPartResponse[] responses)
		{
			foreach (UploadPartResponse uploadPartResponse in responses)
			{
				PartETags.Add(new PartETag(uploadPartResponse.PartNumber, uploadPartResponse.ETag));
			}
		}

		public void AddPartETags(IEnumerable<UploadPartResponse> responses)
		{
			foreach (UploadPartResponse response in responses)
			{
				PartETags.Add(new PartETag(response.PartNumber, response.ETag));
			}
		}

		public void AddPartETags(params CopyPartResponse[] responses)
		{
			foreach (CopyPartResponse copyPartResponse in responses)
			{
				PartETags.Add(new PartETag(copyPartResponse.PartNumber, copyPartResponse.ETag));
			}
		}

		public void AddPartETags(IEnumerable<CopyPartResponse> responses)
		{
			foreach (CopyPartResponse response in responses)
			{
				PartETags.Add(new PartETag(response.PartNumber, response.ETag));
			}
		}

		internal bool IsSetUploadId()
		{
			return uploadId != null;
		}

		internal bool IsSetRequestPayer()
		{
			return requestPayer != null;
		}

		public CompleteMultipartUploadRequest()
			: this()
		{
		}
	}
}
