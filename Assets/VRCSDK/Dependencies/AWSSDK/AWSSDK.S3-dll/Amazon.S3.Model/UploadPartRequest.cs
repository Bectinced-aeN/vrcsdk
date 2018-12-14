using Amazon.Runtime;
using System;
using System.IO;

namespace Amazon.S3.Model
{
	public class UploadPartRequest : AmazonWebServiceRequest
	{
		private Stream inputStream;

		private string bucketName;

		private string key;

		private int? partNumber;

		private string uploadId;

		private long? partSize;

		private string md5Digest;

		private ServerSideEncryptionCustomerMethod serverSideCustomerEncryption;

		private string serverSideEncryptionCustomerProvidedKey;

		private string serverSideEncryptionCustomerProvidedKeyMD5;

		private string filePath;

		private long? filePosition;

		private bool lastPart;

		private RequestPayer requestPayer;

		internal int IVSize
		{
			get;
			set;
		}

		public bool IsLastPart
		{
			get
			{
				return lastPart;
			}
			set
			{
				lastPart = value;
			}
		}

		public Stream InputStream
		{
			get
			{
				return inputStream;
			}
			set
			{
				inputStream = value;
			}
		}

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

		public int PartNumber
		{
			get
			{
				return partNumber.GetValueOrDefault();
			}
			set
			{
				partNumber = value;
			}
		}

		public long PartSize
		{
			get
			{
				return partSize.GetValueOrDefault();
			}
			set
			{
				partSize = value;
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

		public string MD5Digest
		{
			get
			{
				return md5Digest;
			}
			set
			{
				md5Digest = value;
			}
		}

		public ServerSideEncryptionCustomerMethod ServerSideEncryptionCustomerMethod
		{
			get
			{
				return serverSideCustomerEncryption;
			}
			set
			{
				serverSideCustomerEncryption = value;
			}
		}

		public string ServerSideEncryptionCustomerProvidedKey
		{
			get
			{
				return serverSideEncryptionCustomerProvidedKey;
			}
			set
			{
				serverSideEncryptionCustomerProvidedKey = value;
			}
		}

		public string ServerSideEncryptionCustomerProvidedKeyMD5
		{
			get
			{
				return serverSideEncryptionCustomerProvidedKeyMD5;
			}
			set
			{
				serverSideEncryptionCustomerProvidedKeyMD5 = value;
			}
		}

		public string FilePath
		{
			get
			{
				return filePath;
			}
			set
			{
				filePath = value;
			}
		}

		public long FilePosition
		{
			get
			{
				return filePosition.GetValueOrDefault();
			}
			set
			{
				filePosition = value;
			}
		}

		public EventHandler<StreamTransferProgressArgs> StreamTransferProgress
		{
			get
			{
				return this.get_StreamUploadProgressCallback();
			}
			set
			{
				this.set_StreamUploadProgressCallback(value);
			}
		}

		protected override bool IncludeSHA256Header => false;

		protected override bool Expect100Continue => true;

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

		internal bool IsSetInputStream()
		{
			return inputStream != null;
		}

		internal bool IsSetBucketName()
		{
			return bucketName != null;
		}

		internal bool IsSetKey()
		{
			return key != null;
		}

		internal bool IsSetPartNumber()
		{
			return partNumber.HasValue;
		}

		internal bool IsSetPartSize()
		{
			return partSize.HasValue;
		}

		internal bool IsSetUploadId()
		{
			return uploadId != null;
		}

		internal bool IsSetServerSideEncryptionCustomerMethod()
		{
			if (serverSideCustomerEncryption != null)
			{
				return serverSideCustomerEncryption != ServerSideEncryptionCustomerMethod.None;
			}
			return false;
		}

		internal bool IsSetServerSideEncryptionCustomerProvidedKey()
		{
			return !string.IsNullOrEmpty(serverSideEncryptionCustomerProvidedKey);
		}

		internal bool IsSetServerSideEncryptionCustomerProvidedKeyMD5()
		{
			return !string.IsNullOrEmpty(serverSideEncryptionCustomerProvidedKeyMD5);
		}

		internal bool IsSetFilePath()
		{
			return !string.IsNullOrEmpty(filePath);
		}

		internal bool IsSetFilePosition()
		{
			return filePosition.HasValue;
		}

		internal bool IsSetMD5Digest()
		{
			return !string.IsNullOrEmpty(md5Digest);
		}

		internal bool IsSetRequestPayer()
		{
			return requestPayer != null;
		}

		internal void SetupForFilePath()
		{
			FileStream fileStream = File.Open(FilePath, FileMode.Open);
			fileStream.Position = FilePosition;
			InputStream = fileStream;
		}

		public UploadPartRequest()
			: this()
		{
		}
	}
}
