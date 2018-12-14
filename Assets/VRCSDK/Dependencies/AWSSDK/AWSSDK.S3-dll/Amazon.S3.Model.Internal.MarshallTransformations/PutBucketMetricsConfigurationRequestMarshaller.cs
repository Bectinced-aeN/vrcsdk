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
	public class PutBucketMetricsConfigurationRequestMarshaller : IMarshaller<IRequest, PutBucketMetricsConfigurationRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((PutBucketMetricsConfigurationRequest)input);
		}

		public IRequest Marshall(PutBucketMetricsConfigurationRequest PutBucketMetricsConfigurationRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			IRequest val = new DefaultRequest(PutBucketMetricsConfigurationRequest, "AmazonS3");
			val.set_HttpMethod("PUT");
			val.set_ResourcePath("/" + S3Transforms.ToStringValue(PutBucketMetricsConfigurationRequest.BucketName));
			val.AddSubResource("metrics");
			val.AddSubResource("id", PutBucketMetricsConfigurationRequest.MetricsId);
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
			{
				Encoding = Encoding.UTF8,
				OmitXmlDeclaration = true
			}))
			{
				MetricsConfiguration metricsConfiguration = PutBucketMetricsConfigurationRequest.MetricsConfiguration;
				if (metricsConfiguration != null)
				{
					xmlWriter.WriteStartElement("MetricsConfiguration", "http://s3.amazonaws.com/doc/2006-03-01/");
					if (metricsConfiguration != null)
					{
						if (metricsConfiguration.IsSetMetricsId())
						{
							xmlWriter.WriteElementString("Id", "http://s3.amazonaws.com/doc/2006-03-01/", S3Transforms.ToXmlStringValue(metricsConfiguration.MetricsId));
						}
						if (metricsConfiguration.IsSetMetricsFilter())
						{
							xmlWriter.WriteStartElement("Filter", "http://s3.amazonaws.com/doc/2006-03-01/");
							metricsConfiguration.MetricsFilter.MetricsFilterPredicate.Accept(new MetricsPredicateVisitor(xmlWriter));
							xmlWriter.WriteEndElement();
						}
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
