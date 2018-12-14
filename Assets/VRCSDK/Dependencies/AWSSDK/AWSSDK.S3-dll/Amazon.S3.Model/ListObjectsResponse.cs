using Amazon.Runtime;
using System.Collections.Generic;

namespace Amazon.S3.Model
{
	public class ListObjectsResponse : AmazonWebServiceResponse
	{
		private bool? isTruncated;

		private string nextMarker;

		private List<S3Object> contents = new List<S3Object>();

		private string name;

		private string prefix;

		private int? maxKeys;

		private List<string> commonPrefixes = new List<string>();

		private string delimiter;

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

		public string NextMarker
		{
			get
			{
				if (string.IsNullOrEmpty(nextMarker) && isTruncated.GetValueOrDefault() && S3Objects.Count > 0)
				{
					int index = S3Objects.Count - 1;
					nextMarker = S3Objects[index].Key;
				}
				return nextMarker;
			}
			set
			{
				nextMarker = value;
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

		internal bool IsSetIsTruncated()
		{
			return isTruncated.HasValue;
		}

		internal bool IsSetNextMarker()
		{
			return nextMarker != null;
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

		public ListObjectsResponse()
			: this()
		{
		}
	}
}
