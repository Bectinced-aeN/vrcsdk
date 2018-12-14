using Amazon.Runtime;
using System.Collections.Generic;

namespace Amazon.S3.Model
{
	public class DeleteObjectsRequest : AmazonWebServiceRequest
	{
		private string bucketName;

		private List<KeyVersion> objects = new List<KeyVersion>();

		private bool? quiet;

		private MfaCodes mfaCodes;

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

		public List<KeyVersion> Objects
		{
			get
			{
				if (objects == null)
				{
					objects = new List<KeyVersion>();
				}
				return objects;
			}
			set
			{
				objects = value;
			}
		}

		public bool Quiet
		{
			get
			{
				return quiet ?? false;
			}
			set
			{
				quiet = value;
			}
		}

		public MfaCodes MfaCodes
		{
			get
			{
				return mfaCodes;
			}
			set
			{
				mfaCodes = value;
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

		internal bool IsSetObjects()
		{
			return objects.Count > 0;
		}

		internal bool IsSetQuiet()
		{
			return quiet.HasValue;
		}

		internal bool IsSetMfaCodes()
		{
			if (mfaCodes != null && !string.IsNullOrEmpty(MfaCodes.SerialNumber))
			{
				return !string.IsNullOrEmpty(MfaCodes.AuthenticationValue);
			}
			return false;
		}

		internal bool IsSetRequestPayer()
		{
			return requestPayer != null;
		}

		public void AddKey(string key)
		{
			AddKey(new KeyVersion
			{
				Key = key
			});
		}

		public void AddKey(string key, string version)
		{
			AddKey(new KeyVersion
			{
				Key = key,
				VersionId = version
			});
		}

		private void AddKey(KeyVersion keyVersion)
		{
			if (Objects == null)
			{
				Objects = new List<KeyVersion>();
			}
			Objects.Add(keyVersion);
		}

		public DeleteObjectsRequest()
			: this()
		{
		}
	}
}
