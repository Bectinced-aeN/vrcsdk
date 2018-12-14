using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using System.Globalization;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class DeleteObjectTaggingRequestMarshaller : IMarshaller<IRequest, DeleteObjectTaggingRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((DeleteObjectTaggingRequest)input);
		}

		public IRequest Marshall(DeleteObjectTaggingRequest deleteObjectTaggingRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			IRequest val = new DefaultRequest(deleteObjectTaggingRequest, "AmazonS3");
			val.set_HttpMethod("DELETE");
			val.set_ResourcePath(string.Format(CultureInfo.InvariantCulture, "/{0}/{1}", S3Transforms.ToStringValue(deleteObjectTaggingRequest.BucketName), S3Transforms.ToStringValue(deleteObjectTaggingRequest.Key)));
			val.AddSubResource("tagging");
			if (deleteObjectTaggingRequest.IsSetVersionId())
			{
				val.AddSubResource("versionId", S3Transforms.ToStringValue(deleteObjectTaggingRequest.VersionId));
			}
			return val;
		}
	}
}
