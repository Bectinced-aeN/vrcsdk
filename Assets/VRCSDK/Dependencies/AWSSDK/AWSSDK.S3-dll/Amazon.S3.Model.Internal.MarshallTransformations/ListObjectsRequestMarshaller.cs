using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using Amazon.S3.Util;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class ListObjectsRequestMarshaller : IMarshaller<IRequest, ListObjectsRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((ListObjectsRequest)input);
		}

		public IRequest Marshall(ListObjectsRequest listObjectsRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			IRequest val = new DefaultRequest(listObjectsRequest, "AmazonS3");
			val.set_HttpMethod("GET");
			if (listObjectsRequest.IsSetRequestPayer())
			{
				val.get_Headers().Add(S3Constants.AmzHeaderRequestPayer, S3Transforms.ToStringValue(((object)listObjectsRequest.RequestPayer).ToString()));
			}
			val.set_ResourcePath("/" + S3Transforms.ToStringValue(listObjectsRequest.BucketName));
			if (listObjectsRequest.IsSetDelimiter())
			{
				val.get_Parameters().Add("delimiter", S3Transforms.ToStringValue(listObjectsRequest.Delimiter));
			}
			if (listObjectsRequest.IsSetMarker())
			{
				val.get_Parameters().Add("marker", S3Transforms.ToStringValue(listObjectsRequest.Marker));
			}
			if (listObjectsRequest.IsSetMaxKeys())
			{
				val.get_Parameters().Add("max-keys", S3Transforms.ToStringValue(listObjectsRequest.MaxKeys));
			}
			if (listObjectsRequest.IsSetPrefix())
			{
				val.get_Parameters().Add("prefix", S3Transforms.ToStringValue(listObjectsRequest.Prefix));
			}
			if (listObjectsRequest.IsSetEncoding())
			{
				val.get_Parameters().Add("encoding-type", S3Transforms.ToStringValue(ConstantClass.op_Implicit(listObjectsRequest.Encoding)));
			}
			val.set_UseQueryString(true);
			return val;
		}
	}
}
