using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class GetBucketMetricsConfigurationRequestMarshaller : IMarshaller<IRequest, GetBucketMetricsConfigurationRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((GetBucketMetricsConfigurationRequest)input);
		}

		public IRequest Marshall(GetBucketMetricsConfigurationRequest getBucketMetricsConfigurationRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Expected O, but got Unknown
			DefaultRequest val = new DefaultRequest(getBucketMetricsConfigurationRequest, "AmazonS3");
			val.set_Suppress404Exceptions(true);
			val.set_HttpMethod("GET");
			val.set_ResourcePath("/" + S3Transforms.ToStringValue(getBucketMetricsConfigurationRequest.BucketName));
			val.AddSubResource("metrics");
			val.AddSubResource("id", getBucketMetricsConfigurationRequest.MetricsId);
			val.set_UseQueryString(true);
			return val;
		}
	}
}
