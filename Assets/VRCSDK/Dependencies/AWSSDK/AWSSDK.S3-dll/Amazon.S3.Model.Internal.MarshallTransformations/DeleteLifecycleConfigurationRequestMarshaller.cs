using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class DeleteLifecycleConfigurationRequestMarshaller : IMarshaller<IRequest, DeleteLifecycleConfigurationRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((DeleteLifecycleConfigurationRequest)input);
		}

		public IRequest Marshall(DeleteLifecycleConfigurationRequest deleteLifecycleConfigurationRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Expected O, but got Unknown
			DefaultRequest val = new DefaultRequest(deleteLifecycleConfigurationRequest, "AmazonS3");
			val.set_HttpMethod("DELETE");
			val.set_ResourcePath("/" + S3Transforms.ToStringValue(deleteLifecycleConfigurationRequest.BucketName));
			val.AddSubResource("lifecycle");
			val.set_UseQueryString(true);
			return val;
		}
	}
}
