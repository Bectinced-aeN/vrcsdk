using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using Amazon.S3.Util;
using Amazon.Util;
using System.Globalization;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class CopyPartRequestMarshaller : IMarshaller<IRequest, CopyPartRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((CopyPartRequest)input);
		}

		public IRequest Marshall(CopyPartRequest copyPartRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			IRequest val = new DefaultRequest(copyPartRequest, "AmazonS3");
			val.set_HttpMethod("PUT");
			if (copyPartRequest.IsSetSourceBucket())
			{
				val.get_Headers().Add("x-amz-copy-source", ConstructCopySourceHeaderValue(copyPartRequest.SourceBucket, copyPartRequest.SourceKey, copyPartRequest.SourceVersionId));
			}
			if (copyPartRequest.IsSetETagToMatch())
			{
				val.get_Headers().Add("x-amz-copy-source-if-match", AWSSDKUtils.Join(copyPartRequest.ETagToMatch));
			}
			if (copyPartRequest.IsSetETagToNotMatch())
			{
				val.get_Headers().Add("x-amz-copy-source-if-none-match", AWSSDKUtils.Join(copyPartRequest.ETagsToNotMatch));
			}
			if (copyPartRequest.IsSetModifiedSinceDate())
			{
				val.get_Headers().Add("x-amz-copy-source-if-modified-since", copyPartRequest.ModifiedSinceDate.ToUniversalTime().ToString("ddd, dd MMM yyyy HH:mm:ss \\G\\M\\T", CultureInfo.InvariantCulture));
			}
			if (copyPartRequest.IsSetUnmodifiedSinceDate())
			{
				val.get_Headers().Add("x-amz-copy-source-if-unmodified-since", copyPartRequest.UnmodifiedSinceDate.ToUniversalTime().ToString("ddd, dd MMM yyyy HH:mm:ss \\G\\M\\T", CultureInfo.InvariantCulture));
			}
			if (copyPartRequest.IsSetServerSideEncryptionCustomerMethod())
			{
				val.get_Headers().Add("x-amz-server-side-encryption-customer-algorithm", ConstantClass.op_Implicit(copyPartRequest.ServerSideEncryptionCustomerMethod));
			}
			if (copyPartRequest.IsSetServerSideEncryptionCustomerProvidedKey())
			{
				val.get_Headers().Add("x-amz-server-side-encryption-customer-key", copyPartRequest.ServerSideEncryptionCustomerProvidedKey);
				if (copyPartRequest.IsSetServerSideEncryptionCustomerProvidedKeyMD5())
				{
					val.get_Headers().Add("x-amz-server-side-encryption-customer-key-MD5", copyPartRequest.ServerSideEncryptionCustomerProvidedKeyMD5);
				}
				else
				{
					val.get_Headers().Add("x-amz-server-side-encryption-customer-key-MD5", AmazonS3Util.ComputeEncodedMD5FromEncodedString(copyPartRequest.ServerSideEncryptionCustomerProvidedKey));
				}
			}
			if (copyPartRequest.IsSetCopySourceServerSideEncryptionCustomerMethod())
			{
				val.get_Headers().Add("x-amz-copy-source-server-side-encryption-customer-algorithm", ConstantClass.op_Implicit(copyPartRequest.CopySourceServerSideEncryptionCustomerMethod));
			}
			if (copyPartRequest.IsSetCopySourceServerSideEncryptionCustomerProvidedKey())
			{
				val.get_Headers().Add("x-amz-copy-source-server-side-encryption-customer-key", copyPartRequest.CopySourceServerSideEncryptionCustomerProvidedKey);
				if (copyPartRequest.IsSetCopySourceServerSideEncryptionCustomerProvidedKeyMD5())
				{
					val.get_Headers().Add("x-amz-copy-source-server-side-encryption-customer-key-MD5", copyPartRequest.CopySourceServerSideEncryptionCustomerProvidedKeyMD5);
				}
				else
				{
					val.get_Headers().Add("x-amz-copy-source-server-side-encryption-customer-key-MD5", AmazonS3Util.ComputeEncodedMD5FromEncodedString(copyPartRequest.CopySourceServerSideEncryptionCustomerProvidedKey));
				}
			}
			if (copyPartRequest.IsSetServerSideEncryptionKeyManagementServiceKeyId())
			{
				val.get_Headers().Add("x-amz-server-side-encryption-aws-kms-key-id", copyPartRequest.ServerSideEncryptionKeyManagementServiceKeyId);
			}
			if (copyPartRequest.IsSetFirstByte() && copyPartRequest.IsSetLastByte())
			{
				val.get_Headers().Add("x-amz-copy-source-range", ConstructCopySourceRangeHeader(copyPartRequest.FirstByte, copyPartRequest.LastByte));
			}
			val.set_ResourcePath(string.Format(CultureInfo.InvariantCulture, "/{0}/{1}", S3Transforms.ToStringValue(copyPartRequest.DestinationBucket), S3Transforms.ToStringValue(copyPartRequest.DestinationKey)));
			val.AddSubResource("partNumber", S3Transforms.ToStringValue(copyPartRequest.PartNumber));
			val.AddSubResource("uploadId", S3Transforms.ToStringValue(copyPartRequest.UploadId));
			val.set_UseQueryString(true);
			return val;
		}

		private static string ConstructCopySourceHeaderValue(string bucket, string key, string version)
		{
			string text;
			if (!string.IsNullOrEmpty(key))
			{
				text = AmazonS3Util.UrlEncode("/" + bucket + "/" + key, path: true);
				if (!string.IsNullOrEmpty(version))
				{
					text = string.Format(CultureInfo.InvariantCulture, "{0}?versionId={1}", text, AmazonS3Util.UrlEncode(version, path: true));
				}
			}
			else
			{
				text = AmazonS3Util.UrlEncode(bucket, path: true);
			}
			return text;
		}

		private static string ConstructCopySourceRangeHeader(long firstByte, long lastByte)
		{
			return string.Format(CultureInfo.InvariantCulture, "bytes={0}-{1}", firstByte, lastByte);
		}
	}
}
