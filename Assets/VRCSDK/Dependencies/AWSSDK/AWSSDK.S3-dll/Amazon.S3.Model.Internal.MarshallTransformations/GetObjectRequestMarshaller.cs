using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using Amazon.S3.Util;
using System;
using System.Globalization;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class GetObjectRequestMarshaller : IMarshaller<IRequest, GetObjectRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((GetObjectRequest)input);
		}

		public IRequest Marshall(GetObjectRequest getObjectRequest)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Expected O, but got Unknown
			if (string.IsNullOrEmpty(getObjectRequest.Key))
			{
				throw new ArgumentException("Key is a required property and must be set before making this call.", "GetObjectRequest.Key");
			}
			IRequest val = new DefaultRequest(getObjectRequest, "AmazonS3");
			val.set_HttpMethod("GET");
			if (getObjectRequest.IsSetEtagToMatch())
			{
				val.get_Headers().Add("If-Match", S3Transforms.ToStringValue(getObjectRequest.EtagToMatch));
			}
			if (getObjectRequest.IsSetModifiedSinceDate())
			{
				val.get_Headers().Add("If-Modified-Since", S3Transforms.ToStringValue(getObjectRequest.ModifiedSinceDate));
			}
			if (getObjectRequest.IsSetEtagToNotMatch())
			{
				val.get_Headers().Add("If-None-Match", S3Transforms.ToStringValue(getObjectRequest.EtagToNotMatch));
			}
			if (getObjectRequest.IsSetUnmodifiedSinceDate())
			{
				val.get_Headers().Add("If-Unmodified-Since", S3Transforms.ToStringValue(getObjectRequest.UnmodifiedSinceDate));
			}
			if (getObjectRequest.IsSetByteRange())
			{
				val.get_Headers().Add("Range", getObjectRequest.ByteRange.FormattedByteRange);
			}
			if (getObjectRequest.IsSetServerSideEncryptionCustomerMethod())
			{
				val.get_Headers().Add("x-amz-server-side-encryption-customer-algorithm", ConstantClass.op_Implicit(getObjectRequest.ServerSideEncryptionCustomerMethod));
			}
			if (getObjectRequest.IsSetServerSideEncryptionCustomerProvidedKey())
			{
				val.get_Headers().Add("x-amz-server-side-encryption-customer-key", getObjectRequest.ServerSideEncryptionCustomerProvidedKey);
				if (getObjectRequest.IsSetServerSideEncryptionCustomerProvidedKeyMD5())
				{
					val.get_Headers().Add("x-amz-server-side-encryption-customer-key-MD5", getObjectRequest.ServerSideEncryptionCustomerProvidedKeyMD5);
				}
				else
				{
					val.get_Headers().Add("x-amz-server-side-encryption-customer-key-MD5", AmazonS3Util.ComputeEncodedMD5FromEncodedString(getObjectRequest.ServerSideEncryptionCustomerProvidedKey));
				}
			}
			if (getObjectRequest.IsSetRequestPayer())
			{
				val.get_Headers().Add(S3Constants.AmzHeaderRequestPayer, S3Transforms.ToStringValue(((object)getObjectRequest.RequestPayer).ToString()));
			}
			val.set_ResourcePath(string.Format(CultureInfo.InvariantCulture, "/{0}/{1}", S3Transforms.ToStringValue(getObjectRequest.BucketName), S3Transforms.ToStringValue(getObjectRequest.Key)));
			ResponseHeaderOverrides responseHeaderOverrides = getObjectRequest.ResponseHeaderOverrides;
			if (responseHeaderOverrides.CacheControl != null)
			{
				val.get_Parameters().Add("response-cache-control", S3Transforms.ToStringValue(responseHeaderOverrides.CacheControl));
			}
			if (responseHeaderOverrides.ContentDisposition != null)
			{
				val.get_Parameters().Add("response-content-disposition", S3Transforms.ToStringValue(responseHeaderOverrides.ContentDisposition));
			}
			if (responseHeaderOverrides.ContentEncoding != null)
			{
				val.get_Parameters().Add("response-content-encoding", S3Transforms.ToStringValue(responseHeaderOverrides.ContentEncoding));
			}
			if (responseHeaderOverrides.ContentLanguage != null)
			{
				val.get_Parameters().Add("response-content-language", S3Transforms.ToStringValue(responseHeaderOverrides.ContentLanguage));
			}
			if (responseHeaderOverrides.ContentType != null)
			{
				val.get_Parameters().Add("response-content-type", S3Transforms.ToStringValue(responseHeaderOverrides.ContentType));
			}
			if (getObjectRequest.IsSetResponseExpires())
			{
				val.get_Parameters().Add("response-expires", S3Transforms.ToStringValue(getObjectRequest.ResponseExpires));
			}
			if (getObjectRequest.IsSetVersionId())
			{
				val.AddSubResource("versionId", S3Transforms.ToStringValue(getObjectRequest.VersionId));
			}
			if (getObjectRequest.IsSetPartNumber())
			{
				val.AddSubResource("partNumber", S3Transforms.ToStringValue(getObjectRequest.PartNumber.Value));
			}
			val.set_UseQueryString(true);
			return val;
		}
	}
}
