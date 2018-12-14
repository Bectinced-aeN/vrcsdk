using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class ListBucketsRequestMarshaller : IMarshaller<IRequest, ListBucketsRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((ListBucketsRequest)input);
		}

		public IRequest Marshall(ListBucketsRequest listBucketsRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Expected O, but got Unknown
			DefaultRequest val = new DefaultRequest(listBucketsRequest, "AmazonS3");
			val.set_HttpMethod("GET");
			val.set_ResourcePath("/");
			val.set_UseQueryString(true);
			return val;
		}
	}
}
