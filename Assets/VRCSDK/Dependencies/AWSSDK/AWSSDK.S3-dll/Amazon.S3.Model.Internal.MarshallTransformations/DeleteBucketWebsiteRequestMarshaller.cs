using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class DeleteBucketWebsiteRequestMarshaller : IMarshaller<IRequest, DeleteBucketWebsiteRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((DeleteBucketWebsiteRequest)input);
		}

		public IRequest Marshall(DeleteBucketWebsiteRequest deleteBucketWebsiteRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Expected O, but got Unknown
			DefaultRequest val = new DefaultRequest(deleteBucketWebsiteRequest, "AmazonS3");
			val.set_HttpMethod("DELETE");
			val.set_ResourcePath("/" + S3Transforms.ToStringValue(deleteBucketWebsiteRequest.BucketName));
			val.AddSubResource("website");
			val.set_UseQueryString(true);
			return val;
		}
	}
}
