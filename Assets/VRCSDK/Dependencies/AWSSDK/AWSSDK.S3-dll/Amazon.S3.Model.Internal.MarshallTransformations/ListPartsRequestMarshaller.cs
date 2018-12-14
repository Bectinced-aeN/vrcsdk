using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using Amazon.S3.Util;
using System.Globalization;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class ListPartsRequestMarshaller : IMarshaller<IRequest, ListPartsRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((ListPartsRequest)input);
		}

		public IRequest Marshall(ListPartsRequest listPartsRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			IRequest val = new DefaultRequest(listPartsRequest, "AmazonS3");
			if (listPartsRequest.IsSetRequestPayer())
			{
				val.get_Headers().Add(S3Constants.AmzHeaderRequestPayer, S3Transforms.ToStringValue(((object)listPartsRequest.RequestPayer).ToString()));
			}
			val.set_HttpMethod("GET");
			val.set_ResourcePath(string.Format(CultureInfo.InvariantCulture, "/{0}/{1}", S3Transforms.ToStringValue(listPartsRequest.BucketName), S3Transforms.ToStringValue(listPartsRequest.Key)));
			if (listPartsRequest.IsSetUploadId())
			{
				val.AddSubResource("uploadId", S3Transforms.ToStringValue(listPartsRequest.UploadId));
			}
			if (listPartsRequest.IsSetMaxParts())
			{
				val.get_Parameters().Add("max-parts", S3Transforms.ToStringValue(listPartsRequest.MaxParts));
			}
			if (listPartsRequest.IsSetPartNumberMarker())
			{
				val.get_Parameters().Add("part-number-marker", S3Transforms.ToStringValue(listPartsRequest.PartNumberMarker));
			}
			if (listPartsRequest.IsSetEncoding())
			{
				val.get_Parameters().Add("encoding-type", S3Transforms.ToStringValue(ConstantClass.op_Implicit(listPartsRequest.Encoding)));
			}
			val.set_UseQueryString(true);
			return val;
		}
	}
}
