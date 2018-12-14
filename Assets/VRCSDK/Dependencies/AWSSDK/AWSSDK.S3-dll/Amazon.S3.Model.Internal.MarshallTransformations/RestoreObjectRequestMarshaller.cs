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
	public class RestoreObjectRequestMarshaller : IMarshaller<IRequest, RestoreObjectRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((RestoreObjectRequest)input);
		}

		public IRequest Marshall(RestoreObjectRequest restoreObjectRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			IRequest val = new DefaultRequest(restoreObjectRequest, "AmazonS3");
			val.set_HttpMethod("POST");
			if (restoreObjectRequest.IsSetRequestPayer())
			{
				val.get_Headers().Add(S3Constants.AmzHeaderRequestPayer, S3Transforms.ToStringValue(((object)restoreObjectRequest.RequestPayer).ToString()));
			}
			val.set_ResourcePath(string.Format(CultureInfo.InvariantCulture, "/{0}/{1}", S3Transforms.ToStringValue(restoreObjectRequest.BucketName), S3Transforms.ToStringValue(restoreObjectRequest.Key)));
			val.AddSubResource("restore");
			if (restoreObjectRequest.IsSetVersionId())
			{
				val.AddSubResource("versionId", S3Transforms.ToStringValue(restoreObjectRequest.VersionId));
			}
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
			{
				Encoding = Encoding.UTF8,
				OmitXmlDeclaration = true
			}))
			{
				xmlWriter.WriteStartElement("RestoreRequest", "");
				xmlWriter.WriteElementString("Days", "", S3Transforms.ToXmlStringValue(restoreObjectRequest.Days));
				xmlWriter.WriteStartElement("GlacierJobParameters", "");
				xmlWriter.WriteElementString("Tier", "", S3Transforms.ToXmlStringValue(ConstantClass.op_Implicit(restoreObjectRequest.Tier)));
				xmlWriter.WriteEndElement();
				xmlWriter.WriteEndElement();
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
