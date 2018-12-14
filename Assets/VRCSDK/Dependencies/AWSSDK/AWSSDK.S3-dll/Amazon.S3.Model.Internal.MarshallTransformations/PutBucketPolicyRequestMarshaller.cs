using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using System.IO;
using System.Text;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class PutBucketPolicyRequestMarshaller : IMarshaller<IRequest, PutBucketPolicyRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((PutBucketPolicyRequest)input);
		}

		public IRequest Marshall(PutBucketPolicyRequest putBucketPolicyRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			IRequest val = new DefaultRequest(putBucketPolicyRequest, "AmazonS3");
			val.set_HttpMethod("PUT");
			if (putBucketPolicyRequest.IsSetContentMD5())
			{
				val.get_Headers().Add("Content-MD5", S3Transforms.ToStringValue(putBucketPolicyRequest.ContentMD5));
			}
			if (!val.get_Headers().ContainsKey("Content-Type"))
			{
				val.get_Headers().Add("Content-Type", "text/plain");
			}
			val.set_ResourcePath("/" + S3Transforms.ToStringValue(putBucketPolicyRequest.BucketName));
			val.AddSubResource("policy");
			val.set_ContentStream((Stream)new MemoryStream(Encoding.UTF8.GetBytes(putBucketPolicyRequest.Policy)));
			return val;
		}
	}
}
