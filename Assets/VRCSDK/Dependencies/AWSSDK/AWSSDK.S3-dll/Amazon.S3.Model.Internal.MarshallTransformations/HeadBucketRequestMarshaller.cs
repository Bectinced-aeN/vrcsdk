using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class HeadBucketRequestMarshaller : IMarshaller<IRequest, HeadBucketRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((HeadBucketRequest)input);
		}

		public IRequest Marshall(HeadBucketRequest headBucketRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Expected O, but got Unknown
			DefaultRequest val = new DefaultRequest(headBucketRequest, "AmazonS3");
			val.set_HttpMethod("HEAD");
			val.set_ResourcePath("/" + S3Transforms.ToStringValue(headBucketRequest.BucketName));
			val.set_UseQueryString(true);
			return val;
		}
	}
}
