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
	public class PutBucketRequestPaymentRequestMarshaller : IMarshaller<IRequest, PutBucketRequestPaymentRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((PutBucketRequestPaymentRequest)input);
		}

		public IRequest Marshall(PutBucketRequestPaymentRequest putBucketRequestPaymentRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			IRequest val = new DefaultRequest(putBucketRequestPaymentRequest, "AmazonS3");
			val.set_HttpMethod("PUT");
			val.set_ResourcePath("/" + S3Transforms.ToStringValue(putBucketRequestPaymentRequest.BucketName));
			val.AddSubResource("requestPayment");
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
			{
				Encoding = Encoding.UTF8,
				OmitXmlDeclaration = true
			}))
			{
				RequestPaymentConfiguration requestPaymentConfiguration = putBucketRequestPaymentRequest.RequestPaymentConfiguration;
				if (requestPaymentConfiguration != null)
				{
					xmlWriter.WriteStartElement("RequestPaymentConfiguration", "");
					if (requestPaymentConfiguration.IsSetPayer())
					{
						xmlWriter.WriteElementString("Payer", "", S3Transforms.ToXmlStringValue(requestPaymentConfiguration.Payer));
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
