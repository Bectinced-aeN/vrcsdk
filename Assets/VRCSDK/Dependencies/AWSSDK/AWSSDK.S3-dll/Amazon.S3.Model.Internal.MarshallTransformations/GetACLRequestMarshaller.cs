using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using System.Globalization;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class GetACLRequestMarshaller : IMarshaller<IRequest, GetACLRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((GetACLRequest)input);
		}

		public IRequest Marshall(GetACLRequest getObjectAclRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			IRequest val = new DefaultRequest(getObjectAclRequest, "AmazonS3");
			val.set_HttpMethod("GET");
			val.set_ResourcePath(string.Format(CultureInfo.InvariantCulture, "/{0}/{1}", S3Transforms.ToStringValue(getObjectAclRequest.BucketName), S3Transforms.ToStringValue(getObjectAclRequest.Key)));
			val.AddSubResource("acl");
			if (getObjectAclRequest.IsSetVersionId())
			{
				val.AddSubResource("versionId", S3Transforms.ToStringValue(getObjectAclRequest.VersionId));
			}
			val.set_UseQueryString(true);
			return val;
		}
	}
}
