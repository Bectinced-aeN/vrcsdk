using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class ListBucketInventoryConfigurationsRequestMarshaller : IMarshaller<IRequest, ListBucketInventoryConfigurationsRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((ListBucketInventoryConfigurationsRequest)input);
		}

		public IRequest Marshall(ListBucketInventoryConfigurationsRequest listBucketInventoryConfigurationsRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			IRequest val = new DefaultRequest(listBucketInventoryConfigurationsRequest, "AmazonS3");
			val.set_HttpMethod("GET");
			val.set_ResourcePath("/" + S3Transforms.ToStringValue(listBucketInventoryConfigurationsRequest.BucketName));
			val.AddSubResource("inventory");
			if (listBucketInventoryConfigurationsRequest.IsSetContinuationToken())
			{
				val.AddSubResource("continuation-token", listBucketInventoryConfigurationsRequest.ContinuationToken.ToString());
			}
			val.set_UseQueryString(true);
			return val;
		}
	}
}
