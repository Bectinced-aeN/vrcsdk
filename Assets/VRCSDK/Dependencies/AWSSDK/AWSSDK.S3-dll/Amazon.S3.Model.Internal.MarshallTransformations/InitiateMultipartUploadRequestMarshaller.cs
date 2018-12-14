using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using Amazon.S3.Util;
using System.Globalization;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class InitiateMultipartUploadRequestMarshaller : IMarshaller<IRequest, InitiateMultipartUploadRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((InitiateMultipartUploadRequest)input);
		}

		public IRequest Marshall(InitiateMultipartUploadRequest initiateMultipartUploadRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			IRequest val = new DefaultRequest(initiateMultipartUploadRequest, "AmazonS3");
			val.set_HttpMethod("POST");
			if (initiateMultipartUploadRequest.IsSetCannedACL())
			{
				val.get_Headers().Add("x-amz-acl", S3Transforms.ToStringValue(ConstantClass.op_Implicit(initiateMultipartUploadRequest.CannedACL)));
			}
			HeadersCollection headers = initiateMultipartUploadRequest.Headers;
			foreach (string key in headers.Keys)
			{
				val.get_Headers().Add(key, headers[key]);
			}
			HeaderACLRequestMarshaller.Marshall(val, initiateMultipartUploadRequest);
			if (initiateMultipartUploadRequest.IsSetServerSideEncryptionMethod())
			{
				val.get_Headers().Add("x-amz-server-side-encryption", S3Transforms.ToStringValue(ConstantClass.op_Implicit(initiateMultipartUploadRequest.ServerSideEncryptionMethod)));
			}
			if (initiateMultipartUploadRequest.IsSetServerSideEncryptionCustomerMethod())
			{
				val.get_Headers().Add("x-amz-server-side-encryption-customer-algorithm", ConstantClass.op_Implicit(initiateMultipartUploadRequest.ServerSideEncryptionCustomerMethod));
			}
			if (initiateMultipartUploadRequest.IsSetServerSideEncryptionCustomerProvidedKey())
			{
				val.get_Headers().Add("x-amz-server-side-encryption-customer-key", initiateMultipartUploadRequest.ServerSideEncryptionCustomerProvidedKey);
				if (initiateMultipartUploadRequest.IsSetServerSideEncryptionCustomerProvidedKeyMD5())
				{
					val.get_Headers().Add("x-amz-server-side-encryption-customer-key-MD5", initiateMultipartUploadRequest.ServerSideEncryptionCustomerProvidedKeyMD5);
				}
				else
				{
					val.get_Headers().Add("x-amz-server-side-encryption-customer-key-MD5", AmazonS3Util.ComputeEncodedMD5FromEncodedString(initiateMultipartUploadRequest.ServerSideEncryptionCustomerProvidedKey));
				}
			}
			if (initiateMultipartUploadRequest.IsSetServerSideEncryptionKeyManagementServiceKeyId())
			{
				val.get_Headers().Add("x-amz-server-side-encryption-aws-kms-key-id", initiateMultipartUploadRequest.ServerSideEncryptionKeyManagementServiceKeyId);
			}
			if (initiateMultipartUploadRequest.IsSetStorageClass())
			{
				val.get_Headers().Add("x-amz-storage-class", S3Transforms.ToStringValue(ConstantClass.op_Implicit(initiateMultipartUploadRequest.StorageClass)));
			}
			if (initiateMultipartUploadRequest.IsSetWebsiteRedirectLocation())
			{
				val.get_Headers().Add("x-amz-website-redirect-location", S3Transforms.ToStringValue(initiateMultipartUploadRequest.WebsiteRedirectLocation));
			}
			if (initiateMultipartUploadRequest.IsSetRequestPayer())
			{
				val.get_Headers().Add(S3Constants.AmzHeaderRequestPayer, S3Transforms.ToStringValue(((object)initiateMultipartUploadRequest.RequestPayer).ToString()));
			}
			AmazonS3Util.SetMetadataHeaders(val, initiateMultipartUploadRequest.Metadata);
			val.set_ResourcePath(string.Format(CultureInfo.InvariantCulture, "/{0}/{1}", S3Transforms.ToStringValue(initiateMultipartUploadRequest.BucketName), S3Transforms.ToStringValue(initiateMultipartUploadRequest.Key)));
			val.AddSubResource("uploads");
			val.set_UseQueryString(true);
			return val;
		}
	}
}
