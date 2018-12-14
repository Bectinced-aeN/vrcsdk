using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class ListBucketAnalyticsConfigurationsRequestMarshaller : IMarshaller<IRequest, ListBucketAnalyticsConfigurationsRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((ListBucketAnalyticsConfigurationsRequest)input);
		}

		public IRequest Marshall(ListBucketAnalyticsConfigurationsRequest listBucketAnalyticsConfigurationsRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			IRequest val = new DefaultRequest(listBucketAnalyticsConfigurationsRequest, "AmazonS3");
			val.set_HttpMethod("GET");
			val.set_ResourcePath("/" + S3Transforms.ToStringValue(listBucketAnalyticsConfigurationsRequest.BucketName));
			val.AddSubResource("analytics");
			if (listBucketAnalyticsConfigurationsRequest.IsSetContinuationToken())
			{
				val.AddSubResource("continuation-token", listBucketAnalyticsConfigurationsRequest.ContinuationToken.ToString());
			}
			val.set_UseQueryString(true);
			return val;
		}
	}
}
