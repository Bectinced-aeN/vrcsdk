using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using Amazon.S3.Util;
using System.Globalization;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class AbortMultipartUploadRequestMarshaller : IMarshaller<IRequest, AbortMultipartUploadRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((AbortMultipartUploadRequest)input);
		}

		public IRequest Marshall(AbortMultipartUploadRequest abortMultipartUploadRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			IRequest val = new DefaultRequest(abortMultipartUploadRequest, "AmazonS3");
			val.set_HttpMethod("DELETE");
			if (abortMultipartUploadRequest.IsSetRequestPayer())
			{
				val.get_Headers().Add(S3Constants.AmzHeaderRequestPayer, S3Transforms.ToStringValue(((object)abortMultipartUploadRequest.RequestPayer).ToString()));
			}
			val.set_ResourcePath(string.Format(CultureInfo.InvariantCulture, "/{0}/{1}", S3Transforms.ToStringValue(abortMultipartUploadRequest.BucketName), S3Transforms.ToStringValue(abortMultipartUploadRequest.Key)));
			val.AddSubResource("uploadId", S3Transforms.ToStringValue(abortMultipartUploadRequest.UploadId));
			val.set_UseQueryString(true);
			return val;
		}
	}
}
