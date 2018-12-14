using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class DeleteBucketInventoryConfigurationRequestMarshaller : IMarshaller<IRequest, DeleteBucketInventoryConfigurationRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((DeleteBucketInventoryConfigurationRequest)input);
		}

		public IRequest Marshall(DeleteBucketInventoryConfigurationRequest deleteInventoryConfigurationRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Expected O, but got Unknown
			DefaultRequest val = new DefaultRequest(deleteInventoryConfigurationRequest, "AmazonS3");
			val.set_HttpMethod("DELETE");
			val.set_ResourcePath("/" + S3Transforms.ToStringValue(deleteInventoryConfigurationRequest.BucketName));
			val.AddSubResource("inventory");
			val.AddSubResource("id", deleteInventoryConfigurationRequest.InventoryId);
			val.set_UseQueryString(true);
			return val;
		}
	}
}
