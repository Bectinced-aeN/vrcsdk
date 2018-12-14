using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class ListBucketMetricsConfigurationsRequestMarshaller : IMarshaller<IRequest, ListBucketMetricsConfigurationsRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((ListBucketMetricsConfigurationsRequest)input);
		}

		public IRequest Marshall(ListBucketMetricsConfigurationsRequest listBucketMetricsConfigurationRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			IRequest val = new DefaultRequest(listBucketMetricsConfigurationRequest, "AmazonS3");
			val.set_HttpMethod("GET");
			val.set_ResourcePath("/" + S3Transforms.ToStringValue(listBucketMetricsConfigurationRequest.BucketName));
			val.AddSubResource("metrics");
			if (listBucketMetricsConfigurationRequest.IsSetContinuationToken())
			{
				val.AddSubResource("continuation-token", listBucketMetricsConfigurationRequest.ContinuationToken.ToString());
			}
			val.set_UseQueryString(true);
			return val;
		}
	}
}
