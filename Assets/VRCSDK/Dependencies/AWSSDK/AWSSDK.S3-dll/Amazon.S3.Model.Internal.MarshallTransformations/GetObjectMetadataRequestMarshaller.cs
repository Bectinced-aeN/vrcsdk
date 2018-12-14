using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using Amazon.S3.Util;
using System;
using System.Globalization;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class GetObjectMetadataRequestMarshaller : IMarshaller<IRequest, GetObjectMetadataRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((GetObjectMetadataRequest)input);
		}

		public IRequest Marshall(GetObjectMetadataRequest headObjectRequest)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Expected O, but got Unknown
			if (string.IsNullOrEmpty(headObjectRequest.Key))
			{
				throw new ArgumentException("Key is a required property and must be set before making this call.", "GetObjectMetadataRequest.Key");
			}
			IRequest val = new DefaultRequest(headObjectRequest, "AmazonS3");
			val.set_HttpMethod("HEAD");
			if (headObjectRequest.IsSetEtagToMatch())
			{
				val.get_Headers().Add("If-Match", S3Transforms.ToStringValue(headObjectRequest.EtagToMatch));
			}
			if (headObjectRequest.IsSetModifiedSinceDate())
			{
				val.get_Headers().Add("If-Modified-Since", S3Transforms.ToStringValue(headObjectRequest.ModifiedSinceDate));
			}
			if (headObjectRequest.IsSetEtagToNotMatch())
			{
				val.get_Headers().Add("If-None-Match", S3Transforms.ToStringValue(headObjectRequest.EtagToNotMatch));
			}
			if (headObjectRequest.IsSetUnmodifiedSinceDate())
			{
				val.get_Headers().Add("If-Unmodified-Since", S3Transforms.ToStringValue(headObjectRequest.UnmodifiedSinceDate));
			}
			if (headObjectRequest.IsSetServerSideEncryptionCustomerMethod())
			{
				val.get_Headers().Add("x-amz-server-side-encryption-customer-algorithm", ConstantClass.op_Implicit(headObjectRequest.ServerSideEncryptionCustomerMethod));
			}
			if (headObjectRequest.IsSetServerSideEncryptionCustomerProvidedKey())
			{
				val.get_Headers().Add("x-amz-server-side-encryption-customer-key", headObjectRequest.ServerSideEncryptionCustomerProvidedKey);
				if (headObjectRequest.IsSetServerSideEncryptionCustomerProvidedKeyMD5())
				{
					val.get_Headers().Add("x-amz-server-side-encryption-customer-key-MD5", headObjectRequest.ServerSideEncryptionCustomerProvidedKeyMD5);
				}
				else
				{
					val.get_Headers().Add("x-amz-server-side-encryption-customer-key-MD5", AmazonS3Util.ComputeEncodedMD5FromEncodedString(headObjectRequest.ServerSideEncryptionCustomerProvidedKey));
				}
			}
			if (headObjectRequest.IsSetRequestPayer())
			{
				val.get_Headers().Add(S3Constants.AmzHeaderRequestPayer, S3Transforms.ToStringValue(((object)headObjectRequest.RequestPayer).ToString()));
			}
			val.set_ResourcePath(string.Format(CultureInfo.InvariantCulture, "/{0}/{1}", S3Transforms.ToStringValue(headObjectRequest.BucketName), S3Transforms.ToStringValue(headObjectRequest.Key)));
			if (headObjectRequest.IsSetVersionId())
			{
				val.AddSubResource("versionId", S3Transforms.ToStringValue(headObjectRequest.VersionId));
			}
			if (headObjectRequest.IsSetPartNumber())
			{
				val.AddSubResource("partNumber", S3Transforms.ToStringValue(headObjectRequest.PartNumber.Value));
			}
			val.set_UseQueryString(true);
			return val;
		}
	}
}
