using Amazon.Runtime;
using System.Collections.Generic;

namespace Amazon.S3.Model
{
	public class ListVersionsResponse : AmazonWebServiceResponse
	{
		private bool? isTruncated;

		private string keyMarker;

		private string versionIdMarker;

		private string nextKeyMarker;

		private string nextVersionIdMarker;

		private List<S3ObjectVersion> versions = new List<S3ObjectVersion>();

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

		public string KeyMarker
		{
			get
			{
				return keyMarker;
			}
			set
			{
				keyMarker = value;
			}
		}

		public string VersionIdMarker
		{
			get
			{
				return versionIdMarker;
			}
			set
			{
				versionIdMarker = value;
			}
		}

		public string NextKeyMarker
		{
			get
			{
				return nextKeyMarker;
			}
			set
			{
				nextKeyMarker = value;
			}
		}

		public string NextVersionIdMarker
		{
			get
			{
				return nextVersionIdMarker;
			}
			set
			{
				nextVersionIdMarker = value;
			}
		}

		public List<S3ObjectVersion> Versions
		{
			get
			{
				return versions;
			}
			set
			{
				versions = value;
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

		internal bool IsSetKeyMarker()
		{
			return keyMarker != null;
		}

		internal bool IsSetVersionIdMarker()
		{
			return versionIdMarker != null;
		}

		internal bool IsSetNextKeyMarker()
		{
			return nextKeyMarker != null;
		}

		internal bool IsSetNextVersionIdMarker()
		{
			return nextVersionIdMarker != null;
		}

		internal bool IsSetVersions()
		{
			return versions.Count > 0;
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

		public ListVersionsResponse()
			: this()
		{
		}
	}
}
