using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using Amazon.Runtime.Internal.Util;
using Amazon.S3.Util;
using System;
using System.Globalization;
using System.IO;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class PutObjectRequestMarshaller : IMarshaller<IRequest, PutObjectRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((PutObjectRequest)input);
		}

		public IRequest Marshall(PutObjectRequest putObjectRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Expected O, but got Unknown
			IRequest val = new DefaultRequest(putObjectRequest, "AmazonS3");
			val.set_HttpMethod("PUT");
			if (putObjectRequest.IsSetCannedACL())
			{
				val.get_Headers().Add("x-amz-acl", S3Transforms.ToStringValue(ConstantClass.op_Implicit(putObjectRequest.CannedACL)));
			}
			HeadersCollection headers = putObjectRequest.Headers;
			foreach (string key in headers.Keys)
			{
				val.get_Headers()[key] = headers[key];
			}
			if (putObjectRequest.IsSetMD5Digest())
			{
				val.get_Headers()["Content-MD5"] = putObjectRequest.MD5Digest;
			}
			HeaderACLRequestMarshaller.Marshall(val, putObjectRequest);
			if (putObjectRequest.IsSetServerSideEncryptionMethod())
			{
				val.get_Headers().Add("x-amz-server-side-encryption", S3Transforms.ToStringValue(ConstantClass.op_Implicit(putObjectRequest.ServerSideEncryptionMethod)));
			}
			if (putObjectRequest.IsSetStorageClass())
			{
				val.get_Headers().Add("x-amz-storage-class", S3Transforms.ToStringValue(ConstantClass.op_Implicit(putObjectRequest.StorageClass)));
			}
			if (putObjectRequest.IsSetWebsiteRedirectLocation())
			{
				val.get_Headers().Add("x-amz-website-redirect-location", S3Transforms.ToStringValue(putObjectRequest.WebsiteRedirectLocation));
			}
			if (putObjectRequest.IsSetServerSideEncryptionCustomerMethod())
			{
				val.get_Headers().Add("x-amz-server-side-encryption-customer-algorithm", ConstantClass.op_Implicit(putObjectRequest.ServerSideEncryptionCustomerMethod));
			}
			if (putObjectRequest.IsSetServerSideEncryptionCustomerProvidedKey())
			{
				val.get_Headers().Add("x-amz-server-side-encryption-customer-key", putObjectRequest.ServerSideEncryptionCustomerProvidedKey);
				if (putObjectRequest.IsSetServerSideEncryptionCustomerProvidedKeyMD5())
				{
					val.get_Headers().Add("x-amz-server-side-encryption-customer-key-MD5", putObjectRequest.ServerSideEncryptionCustomerProvidedKeyMD5);
				}
				else
				{
					val.get_Headers().Add("x-amz-server-side-encryption-customer-key-MD5", AmazonS3Util.ComputeEncodedMD5FromEncodedString(putObjectRequest.ServerSideEncryptionCustomerProvidedKey));
				}
			}
			if (putObjectRequest.IsSetServerSideEncryptionKeyManagementServiceKeyId())
			{
				val.get_Headers().Add("x-amz-server-side-encryption-aws-kms-key-id", putObjectRequest.ServerSideEncryptionKeyManagementServiceKeyId);
			}
			if (putObjectRequest.IsSetRequestPayer())
			{
				val.get_Headers().Add(S3Constants.AmzHeaderRequestPayer, S3Transforms.ToStringValue(((object)putObjectRequest.RequestPayer).ToString()));
			}
			if (putObjectRequest.IsSetTagSet())
			{
				val.get_Headers().Add(S3Constants.AmzHeaderTagging, AmazonS3Util.TagSetToQueryString(putObjectRequest.TagSet));
			}
			AmazonS3Util.SetMetadataHeaders(val, putObjectRequest.Metadata);
			val.set_ResourcePath(string.Format(CultureInfo.InvariantCulture, "/{0}/{1}", S3Transforms.ToStringValue(putObjectRequest.BucketName), S3Transforms.ToStringValue(putObjectRequest.Key)));
			if (putObjectRequest.InputStream != null)
			{
				Stream streamWithLength = GetStreamWithLength(putObjectRequest.InputStream, putObjectRequest.Headers.ContentLength);
				if (streamWithLength.Length > 0)
				{
					val.set_UseChunkEncoding(true);
				}
				long num = streamWithLength.Length - streamWithLength.Position;
				if (!val.get_Headers().ContainsKey("Content-Length"))
				{
					val.get_Headers().Add("Content-Length", num.ToString(CultureInfo.InvariantCulture));
				}
				MD5Stream val2 = putObjectRequest.InputStream = (Stream)new MD5Stream(streamWithLength, (byte[])null, num);
			}
			val.set_ContentStream(putObjectRequest.InputStream);
			if (!val.get_Headers().ContainsKey("Content-Type"))
			{
				val.get_Headers().Add("Content-Type", "text/plain");
			}
			return val;
		}

		private static Stream GetStreamWithLength(Stream baseStream, long hintLength)
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Expected O, but got Unknown
			Stream result = baseStream;
			bool flag = false;
			long num = -1L;
			try
			{
				num = baseStream.Length - baseStream.Position;
			}
			catch (NotSupportedException)
			{
				flag = true;
				num = hintLength;
			}
			if (num < 0)
			{
				throw new AmazonS3Exception("Could not determine content length");
			}
			if (flag)
			{
				result = (Stream)new PartialReadOnlyWrapperStream(baseStream, num);
			}
			return result;
		}
	}
}
