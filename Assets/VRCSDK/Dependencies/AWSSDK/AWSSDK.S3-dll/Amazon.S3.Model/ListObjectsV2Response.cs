using Amazon.Runtime;
using System.Collections.Generic;

namespace Amazon.S3.Model
{
	public class ListObjectsV2Response : AmazonWebServiceResponse
	{
		private bool? isTruncated;

		private List<S3Object> contents = new List<S3Object>();

		private string name;

		private string prefix;

		private string delimiter;

		private int? maxKeys;

		private List<string> commonPrefixes = new List<string>();

		private EncodingType encoding;

		private int? keyCount;

		private string continuationToken;

		private string nextContinuationToken;

		private string startAfter;

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

		public List<S3Object> S3Objects
		{
			get
			{
				return contents;
			}
			set
			{
				contents = value;
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		public string Prefix
		{
			get
			{
				return prefix;
			}
			set
			{
				prefix = value;
			}
		}

		public int MaxKeys
		{
			get
			{
				return maxKeys ?? 0;
			}
			set
			{
				maxKeys = value;
			}
		}

		public List<string> CommonPrefixes
		{
			get
			{
				return commonPrefixes;
			}
			set
			{
				commonPrefixes = value;
			}
		}

		public string Delimiter
		{
			get
			{
				return delimiter;
			}
			set
			{
				delimiter = value;
			}
		}

		public EncodingType Encoding
		{
			get
			{
				return encoding;
			}
			set
			{
				encoding = value;
			}
		}

		public int KeyCount
		{
			get
			{
				return keyCount ?? 0;
			}
			set
			{
				keyCount = value;
			}
		}

		public string ContinuationToken
		{
			get
			{
				return continuationToken;
			}
			set
			{
				continuationToken = value;
			}
		}

		public string NextContinuationToken
		{
			get
			{
				return nextContinuationToken;
			}
			set
			{
				nextContinuationToken = value;
			}
		}

		public string StartAfter
		{
			get
			{
				return startAfter;
			}
			set
			{
				startAfter = value;
			}
		}

		internal bool IsSetIsTruncated()
		{
			return isTruncated.HasValue;
		}

		internal bool IsSetContents()
		{
			return contents.Count > 0;
		}

		internal bool IsSetName()
		{
			return name != null;
		}

		internal bool IsSetPrefix()
		{
			return prefix != null;
		}

		internal bool IsSetMaxKeys()
		{
			return maxKeys.HasValue;
		}

		internal bool IsSetCommonPrefixes()
		{
			return commonPrefixes.Count > 0;
		}

		internal bool IsSetEncoding()
		{
			return encoding != null;
		}

		internal bool IsSetKeyCount()
		{
			return keyCount.HasValue;
		}

		internal bool IsSetContinuationToken()
		{
			return continuationToken != null;
		}

		internal bool IsSetNextContinuationToken()
		{
			return nextContinuationToken != null;
		}

		internal bool IsSetStartAfter()
		{
			return startAfter != null;
		}

		public ListObjectsV2Response()
			: this()
		{
		}
	}
}
