using Amazon.Runtime;
using Amazon.S3.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;

namespace Amazon.S3.Model
{
	public class PostObjectRequest : AmazonWebServiceRequest
	{
		private S3StorageClass _storageClass;

		private HttpStatusCode _actionStatus;

		private RegionEndpoint _region = RegionEndpoint.USEast1;

		private bool _isSetStorageClass;

		private bool _isSetActionStatus;

		private HeadersCollectionPost headersCollection = new HeadersCollectionPost();

		public string Bucket
		{
			get;
			set;
		}

		public string Key
		{
			get;
			set;
		}

		public Stream InputStream
		{
			get;
			set;
		}

		public string Path
		{
			get;
			set;
		}

		[Obsolete("This API is deprecated. We recommend that you use Headers.ContentType instead.")]
		public string ContentType
		{
			get
			{
				return Headers.ContentType;
			}
			set
			{
				Headers.ContentType = value;
			}
		}

		public S3CannedACL CannedACL
		{
			get;
			set;
		}

		public S3PostUploadSignedPolicy SignedPolicy
		{
			get;
			set;
		}

		public string SuccessActionRedirect
		{
			get;
			set;
		}

		public HttpStatusCode SuccessActionStatus
		{
			get
			{
				return _actionStatus;
			}
			set
			{
				_actionStatus = value;
				_isSetActionStatus = true;
			}
		}

		public S3StorageClass StorageClass
		{
			get
			{
				return _storageClass;
			}
			set
			{
				_storageClass = value;
				_isSetStorageClass = true;
			}
		}

		public RegionEndpoint Region
		{
			get
			{
				return _region;
			}
			set
			{
				_region = value;
			}
		}

		public IDictionary<string, string> Metadata
		{
			get;
			set;
		}

		public HeadersCollectionPost Headers
		{
			get
			{
				if (headersCollection == null)
				{
					headersCollection = new HeadersCollectionPost();
				}
				return headersCollection;
			}
			internal set
			{
				headersCollection = value;
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

		public PostObjectRequest()
			: this()
		{
			Metadata = new Dictionary<string, string>();
			CannedACL = S3CannedACL.Private;
			_actionStatus = HttpStatusCode.NoContent;
			_storageClass = S3StorageClass.Standard;
		}

		internal void WriteFormData(string boundary, Stream outputStream)
		{
			if (!string.IsNullOrEmpty(Key))
			{
				WriteFormDatum(outputStream, S3Constants.PostFormDataObjectKey, Key, boundary);
			}
			WriteFormDatum(outputStream, S3Constants.PostFormDataAcl, ConstantClass.op_Implicit(CannedACL), boundary);
			if (_isSetStorageClass)
			{
				WriteFormDatum(outputStream, "x-amz-storage-class", ConstantClass.op_Implicit(StorageClass), boundary);
			}
			if (_isSetActionStatus)
			{
				WriteFormDatum(outputStream, S3Constants.PostFormDataStatus, ((int)SuccessActionStatus).ToString((IFormatProvider)CultureInfo.InvariantCulture), boundary);
			}
			if (!string.IsNullOrEmpty(SuccessActionRedirect))
			{
				WriteFormDatum(outputStream, S3Constants.PostFormDataRedirect, SuccessActionRedirect, boundary);
			}
			if (SignedPolicy != null && !string.IsNullOrEmpty(SignedPolicy.SecurityToken))
			{
				Metadata[S3Constants.PostFormDataSecurityToken] = SignedPolicy.SecurityToken;
			}
			foreach (string key in Headers.Keys)
			{
				if (!string.IsNullOrEmpty(Headers[key]))
				{
					WriteFormDatum(outputStream, key, Headers[key], boundary);
				}
			}
			foreach (KeyValuePair<string, string> metadatum in Metadata)
			{
				string name = metadatum.Key.StartsWith(S3Constants.PostFormDataXAmzPrefix, StringComparison.Ordinal) ? metadatum.Key : (S3Constants.PostFormDataMetaPrefix + metadatum.Key);
				WriteFormDatum(outputStream, name, metadatum.Value, boundary);
			}
			if (SignedPolicy != null)
			{
				if (SignedPolicy.SignatureVersion == "2")
				{
					WriteFormDatum(outputStream, S3Constants.PostFormDataPolicy, SignedPolicy.Policy, boundary);
					WriteFormDatum(outputStream, S3Constants.PostFormDataSignature, SignedPolicy.Signature, boundary);
					WriteFormDatum(outputStream, S3Constants.PostFormDataAccessKeyId, SignedPolicy.AccessKeyId, boundary);
				}
				else
				{
					WriteFormDatum(outputStream, S3Constants.PostFormDataPolicy, SignedPolicy.Policy, boundary);
					WriteFormDatum(outputStream, S3Constants.PostFormDataXAmzSignature, SignedPolicy.Signature, boundary);
					WriteFormDatum(outputStream, S3Constants.PostFormDataXAmzAlgorithm, SignedPolicy.Algorithm, boundary);
					WriteFormDatum(outputStream, S3Constants.PostFormDataXAmzCredential, SignedPolicy.Credential, boundary);
					WriteFormDatum(outputStream, S3Constants.PostFormDataXAmzDate, SignedPolicy.Date, boundary);
				}
			}
		}

		private static void WriteFormDatum(Stream stream, string name, string value, string boundary)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(string.Format(CultureInfo.InvariantCulture, "--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}\r\n", boundary, name, value));
			stream.Write(bytes, 0, bytes.Length);
		}
	}
}
