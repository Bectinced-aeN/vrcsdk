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
	public class PutBucketReplicationRequestMarshaller : IMarshaller<IRequest, PutBucketReplicationRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((PutBucketReplicationRequest)input);
		}

		public IRequest Marshall(PutBucketReplicationRequest putBucketreplicationRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			IRequest val = new DefaultRequest(putBucketreplicationRequest, "AmazonS3");
			val.set_HttpMethod("PUT");
			val.set_ResourcePath("/" + S3Transforms.ToStringValue(putBucketreplicationRequest.BucketName));
			val.AddSubResource("replication");
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
			{
				Encoding = Encoding.UTF8,
				OmitXmlDeclaration = true
			}))
			{
				ReplicationConfiguration configuration = putBucketreplicationRequest.Configuration;
				if (configuration != null)
				{
					xmlWriter.WriteStartElement("ReplicationConfiguration", "");
					if (configuration.Role != null)
					{
						xmlWriter.WriteElementString("Role", "", S3Transforms.ToXmlStringValue(configuration.Role));
					}
					if (configuration.Rules != null)
					{
						foreach (ReplicationRule rule in configuration.Rules)
						{
							xmlWriter.WriteStartElement("Rule");
							if (rule.IsSetId())
							{
								xmlWriter.WriteElementString("ID", "", S3Transforms.ToXmlStringValue(rule.Id));
							}
							if (rule.IsSetPrefix())
							{
								xmlWriter.WriteElementString("Prefix", "", S3Transforms.ToXmlStringValue(rule.Prefix));
							}
							else
							{
								xmlWriter.WriteElementString("Prefix", "", S3Transforms.ToXmlStringValue(""));
							}
							if (rule.IsSetStatus())
							{
								xmlWriter.WriteElementString("Status", "", S3Transforms.ToXmlStringValue(((object)rule.Status).ToString()));
							}
							if (rule.IsSetDestination())
							{
								xmlWriter.WriteStartElement("Destination", "");
								if (rule.Destination.IsSetBucketArn())
								{
									xmlWriter.WriteElementString("Bucket", "", rule.Destination.BucketArn);
								}
								if (rule.Destination.IsSetStorageClass())
								{
									xmlWriter.WriteElementString("StorageClass", "", ConstantClass.op_Implicit(rule.Destination.StorageClass));
								}
								xmlWriter.WriteEndElement();
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
