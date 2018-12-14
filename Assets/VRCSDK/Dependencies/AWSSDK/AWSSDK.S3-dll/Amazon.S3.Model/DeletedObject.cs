using System;

namespace Amazon.S3.Model
{
	[Serializable]
	public class DeletedObject
	{
		private bool? deleteMarker;

		private string deleteMarkerVersionId;

		private string key;

		private string versionId;

		public bool DeleteMarker
		{
			get
			{
				return deleteMarker ?? false;
			}
			set
			{
				deleteMarker = value;
			}
		}

		public string DeleteMarkerVersionId
		{
			get
			{
				return deleteMarkerVersionId;
			}
			set
			{
				deleteMarkerVersionId = value;
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

		public string VersionId
		{
			get
			{
				return versionId;
			}
			set
			{
				versionId = value;
			}
		}

		internal bool IsSetDeleteMarker()
		{
			return deleteMarker.HasValue;
		}

		internal bool IsSetDeleteMarkerVersionId()
		{
			return deleteMarkerVersionId != null;
		}

		internal bool IsSetKey()
		{
			return key != null;
		}

		internal bool IsSetVersionId()
		{
			return versionId != null;
		}
	}
}
