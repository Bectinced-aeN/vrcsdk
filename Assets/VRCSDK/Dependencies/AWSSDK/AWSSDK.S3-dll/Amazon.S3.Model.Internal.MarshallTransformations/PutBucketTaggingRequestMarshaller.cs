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
	public class PutBucketTaggingRequestMarshaller : IMarshaller<IRequest, PutBucketTaggingRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((PutBucketTaggingRequest)input);
		}

		public IRequest Marshall(PutBucketTaggingRequest putBucketTaggingRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			IRequest val = new DefaultRequest(putBucketTaggingRequest, "AmazonS3");
			val.set_HttpMethod("PUT");
			val.set_ResourcePath("/" + S3Transforms.ToStringValue(putBucketTaggingRequest.BucketName));
			val.AddSubResource("tagging");
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
			{
				Encoding = Encoding.UTF8,
				OmitXmlDeclaration = true
			}))
			{
				xmlWriter.WriteStartElement("Tagging", "");
				List<Tag> tagSet = putBucketTaggingRequest.TagSet;
				if (tagSet != null && tagSet.Count > 0)
				{
					xmlWriter.WriteStartElement("TagSet", "");
					foreach (Tag item in tagSet)
					{
						xmlWriter.WriteStartElement("Tag", "");
						if (item.IsSetKey())
						{
							xmlWriter.WriteElementString("Key", "", S3Transforms.ToXmlStringValue(item.Key));
						}
						if (item.IsSetValue())
						{
							xmlWriter.WriteElementString("Value", "", S3Transforms.ToXmlStringValue(item.Value));
						}
						xmlWriter.WriteEndElement();
					}
					xmlWriter.WriteEndElement();
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
