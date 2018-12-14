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
	public class PutBucketInventoryConfigurationRequestMarshaller : IMarshaller<IRequest, PutBucketInventoryConfigurationRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((PutBucketInventoryConfigurationRequest)input);
		}

		public IRequest Marshall(PutBucketInventoryConfigurationRequest putBucketInventoryConfigurationRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			//IL_0360: Unknown result type (might be due to invalid IL or missing references)
			IRequest val = new DefaultRequest(putBucketInventoryConfigurationRequest, "AmazonS3");
			val.set_HttpMethod("PUT");
			val.set_ResourcePath("/" + S3Transforms.ToStringValue(putBucketInventoryConfigurationRequest.BucketName));
			val.AddSubResource("inventory");
			if (putBucketInventoryConfigurationRequest.IsSetInventoryId())
			{
				val.AddSubResource("id", S3Transforms.ToStringValue(putBucketInventoryConfigurationRequest.InventoryId));
			}
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
			{
				Encoding = Encoding.UTF8,
				OmitXmlDeclaration = true
			}))
			{
				if (putBucketInventoryConfigurationRequest.IsSetInventoryConfiguration())
				{
					InventoryConfiguration inventoryConfiguration = putBucketInventoryConfigurationRequest.InventoryConfiguration;
					xmlWriter.WriteStartElement("InventoryConfiguration", "http://s3.amazonaws.com/doc/2006-03-01/");
					if (inventoryConfiguration != null)
					{
						if (inventoryConfiguration.IsSetDestination())
						{
							InventoryDestination destination = inventoryConfiguration.Destination;
							xmlWriter.WriteStartElement("Destination", "http://s3.amazonaws.com/doc/2006-03-01/");
							if (destination.isSetS3BucketDestination())
							{
								InventoryS3BucketDestination s3BucketDestination = destination.S3BucketDestination;
								xmlWriter.WriteStartElement("S3BucketDestination", "http://s3.amazonaws.com/doc/2006-03-01/");
								if (s3BucketDestination.IsSetAccountId())
								{
									xmlWriter.WriteElementString("AccountId", "http://s3.amazonaws.com/doc/2006-03-01/", S3Transforms.ToXmlStringValue(s3BucketDestination.AccountId));
								}
								if (s3BucketDestination.IsSetBucketName())
								{
									xmlWriter.WriteElementString("Bucket", "http://s3.amazonaws.com/doc/2006-03-01/", S3Transforms.ToXmlStringValue(s3BucketDestination.BucketName));
								}
								if (s3BucketDestination.IsSetInventoryFormat())
								{
									xmlWriter.WriteElementString("Format", "http://s3.amazonaws.com/doc/2006-03-01/", S3Transforms.ToXmlStringValue(ConstantClass.op_Implicit(s3BucketDestination.InventoryFormat)));
								}
								if (s3BucketDestination.IsSetPrefix())
								{
									xmlWriter.WriteElementString("Prefix", "http://s3.amazonaws.com/doc/2006-03-01/", S3Transforms.ToXmlStringValue(s3BucketDestination.Prefix));
								}
								xmlWriter.WriteEndElement();
							}
							xmlWriter.WriteEndElement();
						}
						xmlWriter.WriteElementString("IsEnabled", "http://s3.amazonaws.com/doc/2006-03-01/", inventoryConfiguration.IsEnabled.ToString().ToLowerInvariant());
						if (inventoryConfiguration.IsSetInventoryFilter())
						{
							xmlWriter.WriteStartElement("Filter", "http://s3.amazonaws.com/doc/2006-03-01/");
							inventoryConfiguration.InventoryFilter.InventoryFilterPredicate.Accept(new InventoryPredicateVisitor(xmlWriter));
							xmlWriter.WriteEndElement();
						}
						if (inventoryConfiguration.IsSetInventoryId())
						{
							xmlWriter.WriteElementString("Id", "http://s3.amazonaws.com/doc/2006-03-01/", S3Transforms.ToXmlStringValue(inventoryConfiguration.InventoryId));
						}
						if (inventoryConfiguration.IsSetIncludedObjectVersions())
						{
							xmlWriter.WriteElementString("IncludedObjectVersions", "http://s3.amazonaws.com/doc/2006-03-01/", S3Transforms.ToXmlStringValue(ConstantClass.op_Implicit(inventoryConfiguration.IncludedObjectVersions)));
						}
						if (inventoryConfiguration.IsSetInventoryOptionalFields())
						{
							xmlWriter.WriteStartElement("OptionalFields", "http://s3.amazonaws.com/doc/2006-03-01/");
							foreach (InventoryOptionalField inventoryOptionalField in inventoryConfiguration.InventoryOptionalFields)
							{
								xmlWriter.WriteElementString("Field", "http://s3.amazonaws.com/doc/2006-03-01/", S3Transforms.ToXmlStringValue(ConstantClass.op_Implicit(inventoryOptionalField)));
							}
							xmlWriter.WriteEndElement();
						}
						if (inventoryConfiguration.IsSetSchedule())
						{
							xmlWriter.WriteStartElement("Schedule", "http://s3.amazonaws.com/doc/2006-03-01/");
							InventorySchedule schedule = inventoryConfiguration.Schedule;
							if (schedule.IsFrequency())
							{
								xmlWriter.WriteElementString("Frequency", "http://s3.amazonaws.com/doc/2006-03-01/", S3Transforms.ToXmlStringValue(ConstantClass.op_Implicit(schedule.Frequency)));
							}
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
