using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class GetBucketLoggingRequestMarshaller : IMarshaller<IRequest, GetBucketLoggingRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((GetBucketLoggingRequest)input);
		}

		public IRequest Marshall(GetBucketLoggingRequest getBucketLoggingRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Expected O, but got Unknown
			DefaultRequest val = new DefaultRequest(getBucketLoggingRequest, "AmazonS3");
			val.set_Suppress404Exceptions(true);
			val.set_HttpMethod("GET");
			val.set_ResourcePath("/" + S3Transforms.ToStringValue(getBucketLoggingRequest.BucketName));
			val.AddSubResource("logging");
			val.set_UseQueryString(true);
			return val;
		}
	}
}
