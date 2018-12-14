using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class ListVersionsRequestMarshaller : IMarshaller<IRequest, ListVersionsRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((ListVersionsRequest)input);
		}

		public IRequest Marshall(ListVersionsRequest listVersionsRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			IRequest val = new DefaultRequest(listVersionsRequest, "AmazonS3");
			val.set_HttpMethod("GET");
			val.set_ResourcePath("/" + S3Transforms.ToStringValue(listVersionsRequest.BucketName));
			val.AddSubResource("versions");
			if (listVersionsRequest.IsSetDelimiter())
			{
				val.get_Parameters().Add("delimiter", S3Transforms.ToStringValue(listVersionsRequest.Delimiter));
			}
			if (listVersionsRequest.IsSetKeyMarker())
			{
				val.get_Parameters().Add("key-marker", S3Transforms.ToStringValue(listVersionsRequest.KeyMarker));
			}
			if (listVersionsRequest.IsSetMaxKeys())
			{
				val.get_Parameters().Add("max-keys", S3Transforms.ToStringValue(listVersionsRequest.MaxKeys));
			}
			if (listVersionsRequest.IsSetPrefix())
			{
				val.get_Parameters().Add("prefix", S3Transforms.ToStringValue(listVersionsRequest.Prefix));
			}
			if (listVersionsRequest.IsSetVersionIdMarker())
			{
				val.get_Parameters().Add("version-id-marker", S3Transforms.ToStringValue(listVersionsRequest.VersionIdMarker));
			}
			if (listVersionsRequest.IsSetEncoding())
			{
				val.get_Parameters().Add("encoding-type", S3Transforms.ToStringValue(ConstantClass.op_Implicit(listVersionsRequest.Encoding)));
			}
			val.set_UseQueryString(true);
			return val;
		}
	}
}
