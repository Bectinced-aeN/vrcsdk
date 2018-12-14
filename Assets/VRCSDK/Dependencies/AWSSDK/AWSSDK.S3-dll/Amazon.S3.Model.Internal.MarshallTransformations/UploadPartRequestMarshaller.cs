using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using Amazon.Runtime.Internal.Util;
using Amazon.S3.Util;
using System.Globalization;
using System.IO;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class UploadPartRequestMarshaller : IMarshaller<IRequest, UploadPartRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((UploadPartRequest)input);
		}

		public IRequest Marshall(UploadPartRequest uploadPartRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Expected O, but got Unknown
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Expected O, but got Unknown
			IRequest val = new DefaultRequest(uploadPartRequest, "AmazonS3");
			val.set_HttpMethod("PUT");
			if (uploadPartRequest.IsSetMD5Digest())
			{
				val.get_Headers()["Content-MD5"] = uploadPartRequest.MD5Digest;
			}
			if (uploadPartRequest.IsSetServerSideEncryptionCustomerMethod())
			{
				val.get_Headers().Add("x-amz-server-side-encryption-customer-algorithm", ConstantClass.op_Implicit(uploadPartRequest.ServerSideEncryptionCustomerMethod));
			}
			if (uploadPartRequest.IsSetServerSideEncryptionCustomerProvidedKey())
			{
				val.get_Headers().Add("x-amz-server-side-encryption-customer-key", uploadPartRequest.ServerSideEncryptionCustomerProvidedKey);
				if (uploadPartRequest.IsSetServerSideEncryptionCustomerProvidedKeyMD5())
				{
					val.get_Headers().Add("x-amz-server-side-encryption-customer-key-MD5", uploadPartRequest.ServerSideEncryptionCustomerProvidedKeyMD5);
				}
				else
				{
					val.get_Headers().Add("x-amz-server-side-encryption-customer-key-MD5", AmazonS3Util.ComputeEncodedMD5FromEncodedString(uploadPartRequest.ServerSideEncryptionCustomerProvidedKey));
				}
			}
			if (uploadPartRequest.IsSetRequestPayer())
			{
				val.get_Headers().Add(S3Constants.AmzHeaderRequestPayer, S3Transforms.ToStringValue(((object)uploadPartRequest.RequestPayer).ToString()));
			}
			val.set_ResourcePath(string.Format(CultureInfo.InvariantCulture, "/{0}/{1}", S3Transforms.ToStringValue(uploadPartRequest.BucketName), S3Transforms.ToStringValue(uploadPartRequest.Key)));
			if (uploadPartRequest.IsSetPartNumber())
			{
				val.AddSubResource("partNumber", S3Transforms.ToStringValue(uploadPartRequest.PartNumber));
			}
			if (uploadPartRequest.IsSetUploadId())
			{
				val.AddSubResource("uploadId", S3Transforms.ToStringValue(uploadPartRequest.UploadId));
			}
			if (uploadPartRequest.InputStream != null)
			{
				PartialWrapperStream val2 = new PartialWrapperStream(uploadPartRequest.InputStream, uploadPartRequest.PartSize);
				if (((Stream)val2).Length > 0)
				{
					val.set_UseChunkEncoding(true);
				}
				if (!val.get_Headers().ContainsKey("Content-Length"))
				{
					val.get_Headers().Add("Content-Length", ((Stream)val2).Length.ToString(CultureInfo.InvariantCulture));
				}
				MD5Stream val3 = uploadPartRequest.InputStream = (Stream)new MD5Stream((Stream)val2, (byte[])null, ((Stream)val2).Length);
			}
			val.set_ContentStream(uploadPartRequest.InputStream);
			if (!val.get_Headers().ContainsKey("Content-Type"))
			{
				val.get_Headers().Add("Content-Type", "text/plain");
			}
			return val;
		}
	}
}
