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
	public class PutLifecycleConfigurationRequestMarshaller : IMarshaller<IRequest, PutLifecycleConfigurationRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((PutLifecycleConfigurationRequest)input);
		}

		public IRequest Marshall(PutLifecycleConfigurationRequest putLifecycleConfigurationRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_050c: Unknown result type (might be due to invalid IL or missing references)
			IRequest val = new DefaultRequest(putLifecycleConfigurationRequest, "AmazonS3");
			val.set_HttpMethod("PUT");
			val.set_ResourcePath("/" + S3Transforms.ToStringValue(putLifecycleConfigurationRequest.BucketName));
			val.AddSubResource("lifecycle");
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
			{
				Encoding = Encoding.UTF8,
				OmitXmlDeclaration = true
			}))
			{
				LifecycleConfiguration configuration = putLifecycleConfigurationRequest.Configuration;
				if (configuration != null)
				{
					xmlWriter.WriteStartElement("LifecycleConfiguration", "");
					if (configuration != null)
					{
						List<LifecycleRule> rules = configuration.Rules;
						if (rules != null && rules.Count > 0)
						{
							foreach (LifecycleRule item in rules)
							{
								xmlWriter.WriteStartElement("Rule", "");
								if (item != null)
								{
									LifecycleRuleExpiration expiration = item.Expiration;
									if (expiration != null)
									{
										xmlWriter.WriteStartElement("Expiration", "");
										if (expiration.IsSetDate())
										{
											xmlWriter.WriteElementString("Date", "", S3Transforms.ToXmlStringValue(expiration.Date));
										}
										if (expiration.IsSetDays())
										{
											xmlWriter.WriteElementString("Days", "", S3Transforms.ToXmlStringValue(expiration.Days));
										}
										if (expiration.IsSetExpiredObjectDeleteMarker())
										{
											xmlWriter.WriteElementString("ExpiredObjectDeleteMarker", "", expiration.ExpiredObjectDeleteMarker.ToString().ToLowerInvariant());
										}
										xmlWriter.WriteEndElement();
									}
									List<LifecycleTransition> transitions = item.Transitions;
									if (transitions != null && transitions.Count > 0)
									{
										foreach (LifecycleTransition item2 in transitions)
										{
											if (item2 != null)
											{
												xmlWriter.WriteStartElement("Transition", "");
												if (item2.IsSetDate())
												{
													xmlWriter.WriteElementString("Date", "", S3Transforms.ToXmlStringValue(item2.Date));
												}
												if (item2.IsSetDays())
												{
													xmlWriter.WriteElementString("Days", "", S3Transforms.ToXmlStringValue(item2.Days));
												}
												if (item2.IsSetStorageClass())
												{
													xmlWriter.WriteElementString("StorageClass", "", S3Transforms.ToXmlStringValue(ConstantClass.op_Implicit(item2.StorageClass)));
												}
												xmlWriter.WriteEndElement();
											}
										}
									}
									LifecycleRuleNoncurrentVersionExpiration noncurrentVersionExpiration = item.NoncurrentVersionExpiration;
									if (noncurrentVersionExpiration != null)
									{
										xmlWriter.WriteStartElement("NoncurrentVersionExpiration", "");
										if (noncurrentVersionExpiration.IsSetNoncurrentDays())
										{
											xmlWriter.WriteElementString("NoncurrentDays", "", S3Transforms.ToXmlStringValue(noncurrentVersionExpiration.NoncurrentDays));
										}
										xmlWriter.WriteEndElement();
									}
									List<LifecycleRuleNoncurrentVersionTransition> noncurrentVersionTransitions = item.NoncurrentVersionTransitions;
									if (noncurrentVersionTransitions != null && noncurrentVersionTransitions.Count > 0)
									{
										foreach (LifecycleRuleNoncurrentVersionTransition item3 in noncurrentVersionTransitions)
										{
											if (item3 != null)
											{
												xmlWriter.WriteStartElement("NoncurrentVersionTransition", "");
												if (item3.IsSetNoncurrentDays())
												{
													xmlWriter.WriteElementString("NoncurrentDays", "", S3Transforms.ToXmlStringValue(item3.NoncurrentDays));
												}
												if (item3.IsSetStorageClass())
												{
													xmlWriter.WriteElementString("StorageClass", "", S3Transforms.ToXmlStringValue(ConstantClass.op_Implicit(item3.StorageClass)));
												}
												xmlWriter.WriteEndElement();
											}
										}
									}
									LifecycleRuleAbortIncompleteMultipartUpload abortIncompleteMultipartUpload = item.AbortIncompleteMultipartUpload;
									if (abortIncompleteMultipartUpload != null)
									{
										xmlWriter.WriteStartElement("AbortIncompleteMultipartUpload", "");
										if (abortIncompleteMultipartUpload.IsSetDaysAfterInitiation())
										{
											xmlWriter.WriteElementString("DaysAfterInitiation", "", S3Transforms.ToXmlStringValue(abortIncompleteMultipartUpload.DaysAfterInitiation));
										}
										xmlWriter.WriteEndElement();
									}
								}
								if (item.IsSetId())
								{
									xmlWriter.WriteElementString("ID", "", S3Transforms.ToXmlStringValue(item.Id));
								}
								if (item.IsSetPrefix() && item.IsSetFilter())
								{
									throw new AmazonClientException("LifecycleRule.Prefix is deprecated.  Please only use LifecycleRule.Filter.");
								}
								if (item.IsSetPrefix())
								{
									xmlWriter.WriteElementString("Prefix", "", S3Transforms.ToXmlStringValue(item.Prefix));
								}
								if (item.IsSetFilter())
								{
									xmlWriter.WriteStartElement("Filter", "");
									if (item.Filter.IsSetLifecycleFilterPredicate())
									{
										item.Filter.LifecycleFilterPredicate.Accept(new LifecycleFilterPredicateMarshallVisitor(xmlWriter));
									}
									xmlWriter.WriteEndElement();
								}
								if (item.IsSetStatus())
								{
									xmlWriter.WriteElementString("Status", "", S3Transforms.ToXmlStringValue(ConstantClass.op_Implicit(item.Status)));
								}
								else
								{
									xmlWriter.WriteElementString("Status", "", "Disabled");
								}
								xmlWriter.WriteEndElement();
							}
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
