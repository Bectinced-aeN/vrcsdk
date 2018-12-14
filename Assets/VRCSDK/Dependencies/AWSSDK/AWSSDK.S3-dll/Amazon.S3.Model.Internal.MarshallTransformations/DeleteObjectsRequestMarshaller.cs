using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using Amazon.S3.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class DeleteObjectsRequestMarshaller : IMarshaller<IRequest, DeleteObjectsRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((DeleteObjectsRequest)input);
		}

		public IRequest Marshall(DeleteObjectsRequest deleteObjectsRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			IRequest val = new DefaultRequest(deleteObjectsRequest, "AmazonS3");
			val.set_HttpMethod("POST");
			if (deleteObjectsRequest.IsSetMfaCodes())
			{
				val.get_Headers().Add("x-amz-mfa", deleteObjectsRequest.MfaCodes.FormattedMfaCodes);
			}
			if (deleteObjectsRequest.IsSetRequestPayer())
			{
				val.get_Headers().Add(S3Constants.AmzHeaderRequestPayer, S3Transforms.ToStringValue(((object)deleteObjectsRequest.RequestPayer).ToString()));
			}
			val.set_ResourcePath("/" + S3Transforms.ToStringValue(deleteObjectsRequest.BucketName));
			val.AddSubResource("delete");
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
			{
				Encoding = Encoding.UTF8,
				OmitXmlDeclaration = true
			}))
			{
				xmlWriter.WriteStartElement("Delete", "");
				List<KeyVersion> objects = deleteObjectsRequest.Objects;
				if (objects != null && objects.Count > 0)
				{
					foreach (KeyVersion item in objects)
					{
						xmlWriter.WriteStartElement("Object", "");
						if (item.IsSetKey())
						{
							xmlWriter.WriteElementString("Key", "", S3Transforms.ToXmlStringValue(item.Key));
						}
						if (item.IsSetVersionId())
						{
							xmlWriter.WriteElementString("VersionId", "", S3Transforms.ToXmlStringValue(item.VersionId));
						}
						xmlWriter.WriteEndElement();
					}
				}
				if (deleteObjectsRequest.IsSetQuiet())
				{
					xmlWriter.WriteElementString("Quiet", "", deleteObjectsRequest.Quiet.ToString().ToLowerInvariant());
				}
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
