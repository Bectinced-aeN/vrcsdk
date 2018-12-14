using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class ListMultipartUploadsRequestMarshaller : IMarshaller<IRequest, ListMultipartUploadsRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((ListMultipartUploadsRequest)input);
		}

		public IRequest Marshall(ListMultipartUploadsRequest listMultipartUploadsRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			IRequest val = new DefaultRequest(listMultipartUploadsRequest, "AmazonS3");
			val.set_HttpMethod("GET");
			val.set_ResourcePath("/" + S3Transforms.ToStringValue(listMultipartUploadsRequest.BucketName));
			val.AddSubResource("uploads");
			if (listMultipartUploadsRequest.IsSetDelimiter())
			{
				val.get_Parameters().Add("delimiter", S3Transforms.ToStringValue(listMultipartUploadsRequest.Delimiter));
			}
			if (listMultipartUploadsRequest.IsSetKeyMarker())
			{
				val.get_Parameters().Add("key-marker", S3Transforms.ToStringValue(listMultipartUploadsRequest.KeyMarker));
			}
			if (listMultipartUploadsRequest.IsSetMaxUploads())
			{
				val.get_Parameters().Add("max-uploads", S3Transforms.ToStringValue(listMultipartUploadsRequest.MaxUploads));
			}
			if (listMultipartUploadsRequest.IsSetPrefix())
			{
				val.get_Parameters().Add("prefix", S3Transforms.ToStringValue(listMultipartUploadsRequest.Prefix));
			}
			if (listMultipartUploadsRequest.IsSetUploadIdMarker())
			{
				val.get_Parameters().Add("upload-id-marker", S3Transforms.ToStringValue(listMultipartUploadsRequest.UploadIdMarker));
			}
			if (listMultipartUploadsRequest.IsSetEncoding())
			{
				val.get_Parameters().Add("encoding-type", S3Transforms.ToStringValue(ConstantClass.op_Implicit(listMultipartUploadsRequest.Encoding)));
			}
			val.set_UseQueryString(true);
			return val;
		}
	}
}
