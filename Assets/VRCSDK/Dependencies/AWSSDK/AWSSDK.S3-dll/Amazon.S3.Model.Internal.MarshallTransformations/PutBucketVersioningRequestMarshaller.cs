using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using Amazon.S3.Util;
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class PutBucketVersioningRequestMarshaller : IMarshaller<IRequest, PutBucketVersioningRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((PutBucketVersioningRequest)input);
		}

		public IRequest Marshall(PutBucketVersioningRequest putBucketVersioningRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			IRequest val = new DefaultRequest(putBucketVersioningRequest, "AmazonS3");
			val.set_HttpMethod("PUT");
			if (putBucketVersioningRequest.IsSetMfaCodes())
			{
				val.get_Headers().Add("x-amz-mfa", putBucketVersioningRequest.MfaCodes.FormattedMfaCodes);
			}
			val.set_ResourcePath("/" + S3Transforms.ToStringValue(putBucketVersioningRequest.BucketName));
			val.AddSubResource("versioning");
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
			{
				Encoding = Encoding.UTF8,
				OmitXmlDeclaration = true
			}))
			{
				S3BucketVersioningConfig versioningConfig = putBucketVersioningRequest.VersioningConfig;
				if (versioningConfig != null)
				{
					xmlWriter.WriteStartElement("VersioningConfiguration", "");
					if (versioningConfig.IsSetEnableMfaDelete())
					{
						xmlWriter.WriteElementString("MfaDelete", "", versioningConfig.EnableMfaDelete ? "Enabled" : "Disabled");
					}
					if (versioningConfig.IsSetStatus())
					{
						xmlWriter.WriteElementString("Status", "", S3Transforms.ToXmlStringValue(ConstantClass.op_Implicit(versioningConfig.Status)));
					}
					xmlWriter.WriteEndElement();
				}
			}
			try
			{
				string text = stringWriter.ToString();
				val.set_Content(Encoding.UTF8.GetBytes(text));
				val.get_Headers()["Content-Type"] = "application/xml";
				string value = AmazonS3Util.GenerateChecksumForContent(text, fBase64Encode: true);
				val.get_Headers()["Content-MD5"] = value;
				return val;
			}
			catch (EncoderFallbackException ex)
			{
				throw new AmazonServiceException("Unable to marshall request to XML", (Exception)ex);
			}
		}
	}
}
