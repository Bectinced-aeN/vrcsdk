using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;

namespace Amazon.S3.Util
{
	public class S3PostUploadRequest
	{
		private S3StorageClass _storageClass;

		private HttpStatusCode _actionStatus;

		private RegionEndpoint _region = RegionEndpoint.USEast1;

		private bool _isSetStorageClass;

		private bool _isSetActionStatus;

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

		public string ContentType
		{
			get;
			set;
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

		public S3PostUploadRequest()
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
			if (string.IsNullOrEmpty(ContentType))
			{
				if (Key.IndexOf('.') > -1)
				{
					ContentType = AmazonS3Util.MimeTypeFromExtension(Key.Substring(Key.LastIndexOf('.')));
				}
				else if (!string.IsNullOrEmpty(Path) && Path.IndexOf('.') > -1)
				{
					ContentType = AmazonS3Util.MimeTypeFromExtension(Key.Substring(Path.LastIndexOf('.')));
				}
			}
			WriteFormDatum(outputStream, S3Constants.PostFormDataContentType, ContentType, boundary);
			if (SignedPolicy != null && !string.IsNullOrEmpty(SignedPolicy.SecurityToken))
			{
				Metadata[S3Constants.PostFormDataSecurityToken] = SignedPolicy.SecurityToken;
			}
			foreach (KeyValuePair<string, string> metadatum in Metadata)
			{
				string name = metadatum.Key.StartsWith(S3Constants.PostFormDataXAmzPrefix, StringComparison.Ordinal) ? metadatum.Key : (S3Constants.PostFormDataMetaPrefix + metadatum.Key);
				WriteFormDatum(outputStream, name, metadatum.Value, boundary);
			}
			if (SignedPolicy != null)
			{
				WriteFormDatum(outputStream, S3Constants.PostFormDataAccessKeyId, SignedPolicy.AccessKeyId, boundary);
				WriteFormDatum(outputStream, S3Constants.PostFormDataPolicy, SignedPolicy.Policy, boundary);
				WriteFormDatum(outputStream, S3Constants.PostFormDataSignature, SignedPolicy.Signature, boundary);
			}
		}

		private static void WriteFormDatum(Stream stream, string name, string value, string boundary)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(string.Format(CultureInfo.InvariantCulture, "--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}\r\n", boundary, name, value));
			stream.Write(bytes, 0, bytes.Length);
		}
	}
}
