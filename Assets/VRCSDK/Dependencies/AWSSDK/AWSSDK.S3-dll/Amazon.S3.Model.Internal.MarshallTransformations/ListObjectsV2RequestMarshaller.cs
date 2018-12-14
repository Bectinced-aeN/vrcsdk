using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using Amazon.S3.Util;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class ListObjectsV2RequestMarshaller : IMarshaller<IRequest, ListObjectsV2Request>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((ListObjectsV2Request)input);
		}

		public IRequest Marshall(ListObjectsV2Request listObjectsRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			IRequest val = new DefaultRequest(listObjectsRequest, "AmazonS3");
			if (listObjectsRequest.IsSetRequestPayer())
			{
				val.get_Headers().Add(S3Constants.AmzHeaderRequestPayer, S3Transforms.ToStringValue(((object)listObjectsRequest.RequestPayer).ToString()));
			}
			val.set_HttpMethod("GET");
			val.set_ResourcePath("/" + S3Transforms.ToStringValue(listObjectsRequest.BucketName));
			if (listObjectsRequest.IsSetDelimiter())
			{
				val.get_Parameters().Add("delimiter", S3Transforms.ToStringValue(listObjectsRequest.Delimiter));
			}
			if (listObjectsRequest.IsSetEncoding())
			{
				val.get_Parameters().Add("encoding-type", S3Transforms.ToStringValue(ConstantClass.op_Implicit(listObjectsRequest.Encoding)));
			}
			if (listObjectsRequest.IsSetMaxKeys())
			{
				val.get_Parameters().Add("max-keys", S3Transforms.ToStringValue(listObjectsRequest.MaxKeys));
			}
			if (listObjectsRequest.IsSetPrefix())
			{
				val.get_Parameters().Add("prefix", S3Transforms.ToStringValue(listObjectsRequest.Prefix));
			}
			if (listObjectsRequest.IsSetContinuationToken())
			{
				val.get_Parameters().Add("continuation-token", S3Transforms.ToStringValue(listObjectsRequest.ContinuationToken));
			}
			if (listObjectsRequest.IsSetFetchOwner())
			{
				val.get_Parameters().Add("fetch-owner", listObjectsRequest.FetchOwner.ToString().ToLowerInvariant());
			}
			if (listObjectsRequest.IsSetStartAfter())
			{
				val.get_Parameters().Add("start-after", S3Transforms.ToStringValue(listObjectsRequest.StartAfter));
			}
			val.get_Parameters().Add("list-type", "2");
			val.set_UseQueryString(true);
			return val;
		}
	}
}
