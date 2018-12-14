using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using Amazon.S3.Util;
using System;
using System.Globalization;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class GetObjectTorrentRequestMarshaller : IMarshaller<IRequest, GetObjectTorrentRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((GetObjectTorrentRequest)input);
		}

		public IRequest Marshall(GetObjectTorrentRequest getObjectTorrentRequest)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Expected O, but got Unknown
			if (string.IsNullOrEmpty(getObjectTorrentRequest.Key))
			{
				throw new ArgumentException("Key is a required property and must be set before making this call.", "GetObjectTorrentRequest.Key");
			}
			IRequest val = new DefaultRequest(getObjectTorrentRequest, "AmazonS3");
			val.set_HttpMethod("GET");
			if (getObjectTorrentRequest.IsSetRequestPayer())
			{
				val.get_Headers().Add(S3Constants.AmzHeaderRequestPayer, S3Transforms.ToStringValue(((object)getObjectTorrentRequest.RequestPayer).ToString()));
			}
			if (getObjectTorrentRequest.IsSetRequestPayer())
			{
				val.get_Headers().Add(S3Constants.AmzHeaderRequestPayer, S3Transforms.ToStringValue(((object)getObjectTorrentRequest.RequestPayer).ToString()));
			}
			val.set_ResourcePath(string.Format(CultureInfo.InvariantCulture, "/{0}/{1}", S3Transforms.ToStringValue(getObjectTorrentRequest.BucketName), S3Transforms.ToStringValue(getObjectTorrentRequest.Key)));
			val.AddSubResource("torrent");
			val.set_UseQueryString(true);
			return val;
		}
	}
}
