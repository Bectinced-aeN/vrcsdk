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
	public class PutBucketAnalyticsConfigurationRequestMarshaller : IMarshaller<IRequest, PutBucketAnalyticsConfigurationRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((PutBucketAnalyticsConfigurationRequest)input);
		}

		public IRequest Marshall(PutBucketAnalyticsConfigurationRequest putBucketAnalyticsConfigurationRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			IRequest val = new DefaultRequest(putBucketAnalyticsConfigurationRequest, "AmazonS3");
			val.set_HttpMethod("PUT");
			val.set_ResourcePath("/" + S3Transforms.ToStringValue(putBucketAnalyticsConfigurationRequest.BucketName));
			val.AddSubResource("analytics");
			if (putBucketAnalyticsConfigurationRequest.IsSetAnalyticsId())
			{
				val.AddSubResource("id", S3Transforms.ToStringValue(putBucketAnalyticsConfigurationRequest.AnalyticsId));
			}
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
			{
				Encoding = Encoding.UTF8,
				OmitXmlDeclaration = true
			}))
			{
				if (putBucketAnalyticsConfigurationRequest.IsSetAnalyticsConfiguration())
				{
					AnalyticsConfiguration analyticsConfiguration = putBucketAnalyticsConfigurationRequest.AnalyticsConfiguration;
					xmlWriter.WriteStartElement("AnalyticsConfiguration", "http://s3.amazonaws.com/doc/2006-03-01/");
					if (analyticsConfiguration.IsSetAnalyticsId())
					{
						xmlWriter.WriteElementString("Id", "http://s3.amazonaws.com/doc/2006-03-01/", analyticsConfiguration.AnalyticsId);
					}
					if (analyticsConfiguration.IsSetAnalyticsFilter())
					{
						xmlWriter.WriteStartElement("Filter", "http://s3.amazonaws.com/doc/2006-03-01/");
						analyticsConfiguration.AnalyticsFilter.AnalyticsFilterPredicate.Accept(new AnalyticsPredicateVisitor(xmlWriter));
						xmlWriter.WriteEndElement();
					}
					if (analyticsConfiguration.IsSetStorageClassAnalysis() && analyticsConfiguration.IsSetStorageClassAnalysis())
					{
						StorageClassAnalysis storageClassAnalysis = analyticsConfiguration.StorageClassAnalysis;
						xmlWriter.WriteStartElement("StorageClassAnalysis", "http://s3.amazonaws.com/doc/2006-03-01/");
						if (storageClassAnalysis.IsSetDataExport())
						{
							xmlWriter.WriteStartElement("DataExport", "http://s3.amazonaws.com/doc/2006-03-01/");
							StorageClassAnalysisDataExport dataExport = storageClassAnalysis.DataExport;
							if (dataExport.IsSetOutputSchemaVersion())
							{
								StorageClassAnalysisSchemaVersion outputSchemaVersion = dataExport.OutputSchemaVersion;
								if (outputSchemaVersion != null)
								{
									xmlWriter.WriteElementString("OutputSchemaVersion", "http://s3.amazonaws.com/doc/2006-03-01/", ConstantClass.op_Implicit(outputSchemaVersion));
								}
							}
							if (dataExport.IsSetDestination())
							{
								xmlWriter.WriteStartElement("Destination", "http://s3.amazonaws.com/doc/2006-03-01/");
								AnalyticsExportDestination destination = dataExport.Destination;
								if (destination.IsSetS3BucketDestination())
								{
									xmlWriter.WriteStartElement("S3BucketDestination", "http://s3.amazonaws.com/doc/2006-03-01/");
									AnalyticsS3BucketDestination s3BucketDestination = destination.S3BucketDestination;
									if (s3BucketDestination.IsSetFormat())
									{
										xmlWriter.WriteElementString("Format", "http://s3.amazonaws.com/doc/2006-03-01/", s3BucketDestination.Format);
									}
									if (s3BucketDestination.IsSetBucketAccountId())
									{
										xmlWriter.WriteElementString("BucketAccountId", "http://s3.amazonaws.com/doc/2006-03-01/", s3BucketDestination.BucketAccountId);
									}
									if (s3BucketDestination.IsSetBucketName())
									{
										xmlWriter.WriteElementString("Bucket", "http://s3.amazonaws.com/doc/2006-03-01/", s3BucketDestination.BucketName);
									}
									if (s3BucketDestination.IsSetPrefix())
									{
										xmlWriter.WriteElementString("Prefix", "http://s3.amazonaws.com/doc/2006-03-01/", s3BucketDestination.Prefix);
									}
									xmlWriter.WriteEndElement();
								}
								xmlWriter.WriteEndElement();
							}
							xmlWriter.WriteEndElement();
						}
						xmlWriter.WriteEndElement();
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
