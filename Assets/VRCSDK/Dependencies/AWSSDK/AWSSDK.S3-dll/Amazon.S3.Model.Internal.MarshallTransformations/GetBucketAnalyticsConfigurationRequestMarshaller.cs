using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class GetBucketAnalyticsConfigurationRequestMarshaller : IMarshaller<IRequest, GetBucketAnalyticsConfigurationRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((GetBucketAnalyticsConfigurationRequest)input);
		}

		public IRequest Marshall(GetBucketAnalyticsConfigurationRequest getAnalyticsConfigurationRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Expected O, but got Unknown
			DefaultRequest val = new DefaultRequest(getAnalyticsConfigurationRequest, "AmazonS3");
			val.set_Suppress404Exceptions(true);
			val.set_HttpMethod("GET");
			val.set_ResourcePath("/" + S3Transforms.ToStringValue(getAnalyticsConfigurationRequest.BucketName));
			val.AddSubResource("analytics");
			val.AddSubResource("id", getAnalyticsConfigurationRequest.AnalyticsId);
			val.set_UseQueryString(true);
			return val;
		}
	}
}
