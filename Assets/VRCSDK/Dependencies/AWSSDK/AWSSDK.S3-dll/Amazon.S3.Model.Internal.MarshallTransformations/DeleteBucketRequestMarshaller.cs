using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class DeleteBucketRequestMarshaller : IMarshaller<IRequest, DeleteBucketRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((DeleteBucketRequest)input);
		}

		public IRequest Marshall(DeleteBucketRequest deleteBucketRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			IRequest val = new DefaultRequest(deleteBucketRequest, "AmazonS3");
			val.set_HttpMethod("DELETE");
			val.set_ResourcePath("/" + S3Transforms.ToStringValue(deleteBucketRequest.BucketName));
			if (deleteBucketRequest.BucketRegion != null)
			{
				val.set_AlternateEndpoint(RegionEndpoint.GetBySystemName(deleteBucketRequest.BucketRegion.get_Value()));
			}
			val.set_UseQueryString(true);
			return val;
		}
	}
}
