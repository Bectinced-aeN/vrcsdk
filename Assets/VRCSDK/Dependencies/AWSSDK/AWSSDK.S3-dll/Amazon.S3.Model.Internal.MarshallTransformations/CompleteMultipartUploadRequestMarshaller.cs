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
	public class CompleteMultipartUploadRequestMarshaller : IMarshaller<IRequest, CompleteMultipartUploadRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((CompleteMultipartUploadRequest)input);
		}

		public IRequest Marshall(CompleteMultipartUploadRequest completeMultipartUploadRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			IRequest val = new DefaultRequest(completeMultipartUploadRequest, "AmazonS3");
			val.set_HttpMethod("POST");
			if (completeMultipartUploadRequest.IsSetRequestPayer())
			{
				val.get_Headers().Add(S3Constants.AmzHeaderRequestPayer, S3Transforms.ToStringValue(((object)completeMultipartUploadRequest.RequestPayer).ToString()));
			}
			val.set_ResourcePath(string.Format(CultureInfo.InvariantCulture, "/{0}/{1}", S3Transforms.ToStringValue(completeMultipartUploadRequest.BucketName), S3Transforms.ToStringValue(completeMultipartUploadRequest.Key)));
			val.AddSubResource("uploadId", S3Transforms.ToStringValue(completeMultipartUploadRequest.UploadId));
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
			{
				Encoding = Encoding.UTF8,
				OmitXmlDeclaration = true
			}))
			{
				xmlWriter.WriteStartElement("CompleteMultipartUpload", "");
				List<PartETag> partETags = completeMultipartUploadRequest.PartETags;
				partETags.Sort();
				if (partETags != null && partETags.Count > 0)
				{
					foreach (PartETag item in partETags)
					{
						xmlWriter.WriteStartElement("Part", "");
						if (item.IsSetETag())
						{
							xmlWriter.WriteElementString("ETag", "", S3Transforms.ToXmlStringValue(item.ETag));
						}
						if (item.IsSetPartNumber())
						{
							xmlWriter.WriteElementString("PartNumber", "", S3Transforms.ToXmlStringValue(item.PartNumber));
						}
						xmlWriter.WriteEndElement();
					}
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
