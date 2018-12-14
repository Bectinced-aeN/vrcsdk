using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using Amazon.S3.Util;
using System;
using System.Globalization;
using System.Text;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class PutObjectTaggingRequestMarshaller : IMarshaller<IRequest, PutObjectTaggingRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((PutObjectTaggingRequest)input);
		}

		public IRequest Marshall(PutObjectTaggingRequest putObjectTaggingRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			IRequest val = new DefaultRequest(putObjectTaggingRequest, "AmazonS3");
			val.set_HttpMethod("PUT");
			val.set_ResourcePath(string.Format(CultureInfo.InvariantCulture, "/{0}/{1}", S3Transforms.ToStringValue(putObjectTaggingRequest.BucketName), S3Transforms.ToStringValue(putObjectTaggingRequest.Key)));
			val.AddSubResource("tagging");
			try
			{
				string text = AmazonS3Util.SerializeTaggingToXml(putObjectTaggingRequest.Tagging);
				val.set_Content(Encoding.UTF8.GetBytes(text));
				val.get_Headers()["Content-Type"] = "application/xml";
				string value = AmazonS3Util.GenerateChecksumForContent(text, fBase64Encode: true);
				val.get_Headers()["Content-MD5"] = value;
				return val;
			}
			catch (EncoderFallbackException ex)
			{
				throw new AmazonServiceException("Unable to marhsall request to XML", (Exception)ex);
			}
		}
	}
}
