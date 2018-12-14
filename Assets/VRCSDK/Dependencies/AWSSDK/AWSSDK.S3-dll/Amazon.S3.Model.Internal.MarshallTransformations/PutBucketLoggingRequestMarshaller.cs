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
	public class PutBucketLoggingRequestMarshaller : IMarshaller<IRequest, PutBucketLoggingRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((PutBucketLoggingRequest)input);
		}

		public IRequest Marshall(PutBucketLoggingRequest putBucketLoggingRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			//IL_0303: Unknown result type (might be due to invalid IL or missing references)
			IRequest val = new DefaultRequest(putBucketLoggingRequest, "AmazonS3");
			val.set_HttpMethod("PUT");
			val.set_ResourcePath("/" + S3Transforms.ToStringValue(putBucketLoggingRequest.BucketName));
			val.AddSubResource("logging");
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
			{
				Encoding = Encoding.UTF8,
				OmitXmlDeclaration = true
			}))
			{
				xmlWriter.WriteStartElement("BucketLoggingStatus", "");
				S3BucketLoggingConfig loggingConfig = putBucketLoggingRequest.LoggingConfig;
				if (loggingConfig != null && loggingConfig != null)
				{
					S3BucketLoggingConfig s3BucketLoggingConfig = loggingConfig;
					if (s3BucketLoggingConfig != null && s3BucketLoggingConfig.IsSetTargetBucket())
					{
						xmlWriter.WriteStartElement("LoggingEnabled", "");
						xmlWriter.WriteElementString("TargetBucket", "", S3Transforms.ToXmlStringValue(s3BucketLoggingConfig.TargetBucketName));
						List<S3Grant> grants = s3BucketLoggingConfig.Grants;
						if (grants != null && grants.Count > 0)
						{
							xmlWriter.WriteStartElement("TargetGrants", "");
							foreach (S3Grant item in grants)
							{
								xmlWriter.WriteStartElement("Grant", "");
								if (item != null)
								{
									S3Grantee grantee = item.Grantee;
									if (grantee != null)
									{
										xmlWriter.WriteStartElement("Grantee", "");
										if (grantee.IsSetType())
										{
											xmlWriter.WriteAttributeString("xsi", "type", "http://www.w3.org/2001/XMLSchema-instance", ((object)grantee.Type).ToString());
										}
										if (grantee.IsSetDisplayName())
										{
											xmlWriter.WriteElementString("DisplayName", "", S3Transforms.ToXmlStringValue(grantee.DisplayName));
										}
										if (grantee.IsSetEmailAddress())
										{
											xmlWriter.WriteElementString("EmailAddress", "", S3Transforms.ToXmlStringValue(grantee.EmailAddress));
										}
										if (grantee.IsSetCanonicalUser())
										{
											xmlWriter.WriteElementString("ID", "", S3Transforms.ToXmlStringValue(grantee.CanonicalUser));
										}
										if (grantee.IsSetURI())
										{
											xmlWriter.WriteElementString("URI", "", S3Transforms.ToXmlStringValue(grantee.URI));
										}
										xmlWriter.WriteEndElement();
									}
									if (item.IsSetPermission())
									{
										xmlWriter.WriteElementString("Permission", "", S3Transforms.ToXmlStringValue(ConstantClass.op_Implicit(item.Permission)));
									}
								}
								xmlWriter.WriteEndElement();
							}
							xmlWriter.WriteEndElement();
						}
						if (s3BucketLoggingConfig.IsSetTargetPrefix())
						{
							xmlWriter.WriteElementString("TargetPrefix", "", S3Transforms.ToXmlStringValue(s3BucketLoggingConfig.TargetPrefix));
						}
						else
						{
							xmlWriter.WriteStartElement("TargetPrefix");
							xmlWriter.WriteEndElement();
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
