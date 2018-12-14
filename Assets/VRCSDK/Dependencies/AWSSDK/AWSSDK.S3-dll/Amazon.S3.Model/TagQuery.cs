using System.Collections.Generic;

namespace Amazon.S3.Model
{
	public class TagQuery
	{
		private int? maxKeys;

		private bool includeTags;

		private string continuationToken;

		private EncodingType encodingType;

		private List<TagQueryFilter> and = new List<TagQueryFilter>();

		public int? MaxKeys
		{
			get
			{
				return maxKeys;
			}
			set
			{
				maxKeys = value;
			}
		}

		public bool IncludeTags
		{
			get
			{
				return includeTags;
			}
			set
			{
				includeTags = value;
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

		public EncodingType EncondingType
		{
			get
			{
				return encodingType;
			}
			set
			{
				encodingType = value;
			}
		}

		public List<TagQueryFilter> And
		{
			get
			{
				return and;
			}
			set
			{
				and = value;
			}
		}

		internal bool IsSetMaxKeys()
		{
			return maxKeys.HasValue;
		}

		internal bool IsSetContinuationToken()
		{
			return !string.IsNullOrEmpty(continuationToken);
		}

		internal bool IsSetEncodingType()
		{
			return encodingType != null;
		}
	}
}
