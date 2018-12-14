using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using System.Globalization;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class GetObjectTaggingRequestMarshaller : IMarshaller<IRequest, GetObjectTaggingRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((GetObjectTaggingRequest)input);
		}

		public IRequest Marshall(GetObjectTaggingRequest getObjectTaggingRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			IRequest val = new DefaultRequest(getObjectTaggingRequest, "AmazonS3");
			val.set_HttpMethod("GET");
			val.set_UseQueryString(true);
			val.set_ResourcePath(string.Format(CultureInfo.InvariantCulture, "/{0}/{1}", S3Transforms.ToStringValue(getObjectTaggingRequest.BucketName), S3Transforms.ToStringValue(getObjectTaggingRequest.Key)));
			val.AddSubResource("tagging");
			return val;
		}
	}
}
