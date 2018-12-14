using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using Amazon.S3.Util;
using System;
using System.Globalization;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class CopyObjectRequestMarshaller : IMarshaller<IRequest, CopyObjectRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((CopyObjectRequest)input);
		}

		public IRequest Marshall(CopyObjectRequest copyObjectRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			IRequest val = new DefaultRequest(copyObjectRequest, "AmazonS3");
			val.set_HttpMethod("PUT");
			if (copyObjectRequest.IsSetCannedACL())
			{
				val.get_Headers().Add("x-amz-acl", S3Transforms.ToStringValue(ConstantClass.op_Implicit(copyObjectRequest.CannedACL)));
			}
			HeadersCollection headers = copyObjectRequest.Headers;
			foreach (string key in headers.Keys)
			{
				val.get_Headers()[key] = headers[key];
			}
			HeaderACLRequestMarshaller.Marshall(val, copyObjectRequest);
			if (copyObjectRequest.IsSetSourceBucket())
			{
				val.get_Headers().Add("x-amz-copy-source", ConstructCopySourceHeaderValue(copyObjectRequest.SourceBucket, copyObjectRequest.SourceKey, copyObjectRequest.SourceVersionId));
			}
			if (copyObjectRequest.IsSetETagToMatch())
			{
				val.get_Headers().Add("x-amz-copy-source-if-match", S3Transforms.ToStringValue(copyObjectRequest.ETagToMatch));
			}
			if (copyObjectRequest.IsSetModifiedSinceDate())
			{
				val.get_Headers().Add("x-amz-copy-source-if-modified-since", S3Transforms.ToStringValue(copyObjectRequest.ModifiedSinceDate));
			}
			if (copyObjectRequest.IsSetETagToNotMatch())
			{
				val.get_Headers().Add("x-amz-copy-source-if-none-match", S3Transforms.ToStringValue(copyObjectRequest.ETagToNotMatch));
			}
			if (copyObjectRequest.IsSetUnmodifiedSinceDate())
			{
				val.get_Headers().Add("x-amz-copy-source-if-unmodified-since", S3Transforms.ToStringValue(copyObjectRequest.UnmodifiedSinceDate));
			}
			if (copyObjectRequest.IsSetTagSet())
			{
				val.get_Headers().Add(S3Constants.AmzHeaderTagging, AmazonS3Util.TagSetToQueryString(copyObjectRequest.TagSet));
				val.get_Headers().Add(S3Constants.AmzHeaderTaggingDirective, TaggingDirective.REPLACE.get_Value());
			}
			else
			{
				val.get_Headers().Add(S3Constants.AmzHeaderTaggingDirective, TaggingDirective.COPY.get_Value());
			}
			val.get_Headers().Add("x-amz-metadata-directive", S3Transforms.ToStringValue(copyObjectRequest.MetadataDirective.ToString()));
			if (copyObjectRequest.IsSetServerSideEncryptionMethod())
			{
				val.get_Headers().Add("x-amz-server-side-encryption", S3Transforms.ToStringValue(ConstantClass.op_Implicit(copyObjectRequest.ServerSideEncryptionMethod)));
			}
			if (copyObjectRequest.IsSetServerSideEncryptionCustomerMethod())
			{
				val.get_Headers().Add("x-amz-server-side-encryption-customer-algorithm", ConstantClass.op_Implicit(copyObjectRequest.ServerSideEncryptionCustomerMethod));
			}
			if (copyObjectRequest.IsSetServerSideEncryptionCustomerProvidedKey())
			{
				val.get_Headers().Add("x-amz-server-side-encryption-customer-key", copyObjectRequest.ServerSideEncryptionCustomerProvidedKey);
				if (copyObjectRequest.IsSetServerSideEncryptionCustomerProvidedKeyMD5())
				{
					val.get_Headers().Add("x-amz-server-side-encryption-customer-key-MD5", copyObjectRequest.ServerSideEncryptionCustomerProvidedKeyMD5);
				}
				else
				{
					val.get_Headers().Add("x-amz-server-side-encryption-customer-key-MD5", AmazonS3Util.ComputeEncodedMD5FromEncodedString(copyObjectRequest.ServerSideEncryptionCustomerProvidedKey));
				}
			}
			if (copyObjectRequest.IsSetCopySourceServerSideEncryptionCustomerMethod())
			{
				val.get_Headers().Add("x-amz-copy-source-server-side-encryption-customer-algorithm", ConstantClass.op_Implicit(copyObjectRequest.CopySourceServerSideEncryptionCustomerMethod));
			}
			if (copyObjectRequest.IsSetCopySourceServerSideEncryptionCustomerProvidedKey())
			{
				val.get_Headers().Add("x-amz-copy-source-server-side-encryption-customer-key", copyObjectRequest.CopySourceServerSideEncryptionCustomerProvidedKey);
				if (copyObjectRequest.IsSetCopySourceServerSideEncryptionCustomerProvidedKeyMD5())
				{
					val.get_Headers().Add("x-amz-copy-source-server-side-encryption-customer-key-MD5", copyObjectRequest.CopySourceServerSideEncryptionCustomerProvidedKeyMD5);
				}
				else
				{
					val.get_Headers().Add("x-amz-copy-source-server-side-encryption-customer-key-MD5", AmazonS3Util.ComputeEncodedMD5FromEncodedString(copyObjectRequest.CopySourceServerSideEncryptionCustomerProvidedKey));
				}
			}
			if (copyObjectRequest.IsSetServerSideEncryptionKeyManagementServiceKeyId())
			{
				val.get_Headers().Add("x-amz-server-side-encryption-aws-kms-key-id", copyObjectRequest.ServerSideEncryptionKeyManagementServiceKeyId);
			}
			if (copyObjectRequest.IsSetStorageClass())
			{
				val.get_Headers().Add("x-amz-storage-class", S3Transforms.ToStringValue(ConstantClass.op_Implicit(copyObjectRequest.StorageClass)));
			}
			if (copyObjectRequest.IsSetWebsiteRedirectLocation())
			{
				val.get_Headers().Add("x-amz-website-redirect-location", S3Transforms.ToStringValue(copyObjectRequest.WebsiteRedirectLocation));
			}
			if (copyObjectRequest.IsSetRequestPayer())
			{
				val.get_Headers().Add(S3Constants.AmzHeaderRequestPayer, S3Transforms.ToStringValue(((object)copyObjectRequest.RequestPayer).ToString()));
			}
			AmazonS3Util.SetMetadataHeaders(val, copyObjectRequest.Metadata);
			string value = copyObjectRequest.DestinationKey.StartsWith("/", StringComparison.Ordinal) ? copyObjectRequest.DestinationKey.Substring(1) : copyObjectRequest.DestinationKey;
			val.set_ResourcePath(string.Format(CultureInfo.InvariantCulture, "/{0}/{1}", S3Transforms.ToStringValue(copyObjectRequest.DestinationBucket), S3Transforms.ToStringValue(value)));
			val.set_UseQueryString(true);
			return val;
		}

		private static string ConstructCopySourceHeaderValue(string bucket, string key, string version)
		{
			string text;
			if (!string.IsNullOrEmpty(key))
			{
				string str = key.StartsWith("/", StringComparison.Ordinal) ? key.Substring(1) : key;
				text = AmazonS3Util.UrlEncode("/" + bucket + "/" + str, path: true);
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
	}
}
