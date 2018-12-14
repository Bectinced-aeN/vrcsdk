using Amazon.Runtime;
using System;
using System.Collections.Generic;

namespace Amazon.S3.Model
{
	public class ListPartsResponse : AmazonWebServiceResponse
	{
		private string bucketName;

		private string key;

		private string uploadId;

		private Owner owner;

		private Initiator initiator;

		private S3StorageClass storageClass;

		private int? partNumberMarker;

		private int? nextPartNumberMarker;

		private int? maxParts;

		private bool? isTruncated;

		private List<PartDetail> parts = new List<PartDetail>();

		private DateTime? abortDate;

		private string abortRuleId;

		private RequestCharged requestCharged;

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

		public int PartNumberMarker
		{
			get
			{
				return partNumberMarker ?? 0;
			}
			set
			{
				partNumberMarker = value;
			}
		}

		public int NextPartNumberMarker
		{
			get
			{
				return nextPartNumberMarker ?? 0;
			}
			set
			{
				nextPartNumberMarker = value;
			}
		}

		public int MaxParts
		{
			get
			{
				return maxParts ?? 0;
			}
			set
			{
				maxParts = value;
			}
		}

		public bool IsTruncated
		{
			get
			{
				return isTruncated ?? false;
			}
			set
			{
				isTruncated = value;
			}
		}

		public List<PartDetail> Parts
		{
			get
			{
				return parts;
			}
			set
			{
				parts = value;
			}
		}

		public Initiator Initiator
		{
			get
			{
				return initiator;
			}
			set
			{
				initiator = value;
			}
		}

		public Owner Owner
		{
			get
			{
				return owner;
			}
			set
			{
				owner = value;
			}
		}

		public string StorageClass
		{
			get
			{
				return ConstantClass.op_Implicit(storageClass);
			}
			set
			{
				storageClass = value;
			}
		}

		public DateTime AbortDate
		{
			get
			{
				return abortDate.GetValueOrDefault();
			}
			set
			{
				abortDate = value;
			}
		}

		public string AbortRuleId
		{
			get
			{
				return abortRuleId;
			}
			set
			{
				abortRuleId = value;
			}
		}

		public RequestCharged RequestCharged
		{
			get
			{
				return requestCharged;
			}
			set
			{
				requestCharged = value;
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

		internal bool IsSetUploadId()
		{
			return uploadId != null;
		}

		internal bool IsSetPartNumberMarker()
		{
			return partNumberMarker.HasValue;
		}

		internal bool IsSetNextPartNumberMarker()
		{
			return nextPartNumberMarker.HasValue;
		}

		internal bool IsSetMaxParts()
		{
			return maxParts.HasValue;
		}

		internal bool IsSetIsTruncated()
		{
			return isTruncated.HasValue;
		}

		internal bool IsSetParts()
		{
			return parts.Count > 0;
		}

		internal bool IsSetInitiator()
		{
			return initiator != null;
		}

		internal bool IsSetOwner()
		{
			return owner != null;
		}

		internal bool IsSetStorageClass()
		{
			return storageClass != null;
		}

		internal bool IsSetAbortDate()
		{
			return abortDate.HasValue;
		}

		internal bool IsSetAbortRuleId()
		{
			return abortRuleId != null;
		}

		internal bool IsSetRequestCharged()
		{
			return requestCharged != null;
		}

		public ListPartsResponse()
			: this()
		{
		}
	}
}
