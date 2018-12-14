using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using Amazon.S3.Util;
using System.Globalization;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class DeleteObjectRequestMarshaller : IMarshaller<IRequest, DeleteObjectRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((DeleteObjectRequest)input);
		}

		public IRequest Marshall(DeleteObjectRequest deleteObjectRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			IRequest val = new DefaultRequest(deleteObjectRequest, "AmazonS3");
			val.set_HttpMethod("DELETE");
			if (deleteObjectRequest.IsSetMfaCodes())
			{
				val.get_Headers().Add("x-amz-mfa", deleteObjectRequest.MfaCodes.FormattedMfaCodes);
			}
			val.set_ResourcePath(string.Format(CultureInfo.InvariantCulture, "/{0}/{1}", S3Transforms.ToStringValue(deleteObjectRequest.BucketName), S3Transforms.ToStringValue(deleteObjectRequest.Key)));
			if (deleteObjectRequest.IsSetVersionId())
			{
				val.AddSubResource("versionId", S3Transforms.ToStringValue(deleteObjectRequest.VersionId));
			}
			if (deleteObjectRequest.IsSetRequestPayer())
			{
				val.get_Headers().Add(S3Constants.AmzHeaderRequestPayer, S3Transforms.ToStringValue(((object)deleteObjectRequest.RequestPayer).ToString()));
			}
			val.set_UseQueryString(true);
			return val;
		}
	}
}
